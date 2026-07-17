import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import * as competitionTeamApi from "../competitionTeams/competitionTeamApi";
import type { CompetitionTeamPreviewDto } from "../competitionTeams/competitionTeamTypes";
import * as groupApi from "../competitionGroups/competitionGroupApi";
import type { CompetitionGroupPreviewDto } from "../competitionGroups/competitionGroupTypes";
import * as seasonApi from "../competitionSeasons/competitionSeasonApi";
import type { CompetitionSeasonResponseDto } from "../competitionSeasons/competitionSeasonTypes";
import * as stageApi from "../competitionStages/competitionStageApi";
import type { CompetitionStagePreviewDto } from "../competitionStages/competitionStageTypes";
import { COMPETITION_STAGE_TYPE } from "../competitionStages/competitionStageTypes";
import * as teamApi from "../teams/teamApi";
import type { TeamResponseDto } from "../teams/teamTypes";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";
type SortMode = "stage" | "name" | "recent";

type SeasonTeamsState = {
  status: LoadStatus;
  actionStatus: LoadStatus;
  seasonId: string | null;
  season: CompetitionSeasonResponseDto | null;
  catalogTeams: TeamResponseDto[];
  assignedTeams: CompetitionTeamPreviewDto[];
  stages: CompetitionStagePreviewDto[];
  groupsByStageId: Record<string, CompetitionGroupPreviewDto[]>;
  poolSearch: string;
  countryFilter: string | null;
  sortMode: SortMode;
};

const initialState: SeasonTeamsState = {
  status: "idle",
  actionStatus: "idle",
  seasonId: null,
  season: null,
  catalogTeams: [],
  assignedTeams: [],
  stages: [],
  groupsByStageId: {},
  poolSearch: "",
  countryFilter: null,
  sortMode: "stage",
};

async function loadAllCatalogTeams(): Promise<TeamResponseDto[]> {
  const first = await teamApi.getAllTeams(1, 50);

  if (!first.success || !first.data) {
    throw new Error(first.message ?? "Takımlar yüklenemedi.");
  }

  const teams = [...first.data.items];
  const totalPages = first.data.totalPages;

  for (let page = 2; page <= totalPages; page += 1) {
    const next = await teamApi.getAllTeams(page, 50);

    if (next.success && next.data) {
      teams.push(...next.data.items);
    }
  }

  return teams.filter((team) => team.isActive);
}

export const loadSeasonTeamsPage = createAsyncThunk(
  "seasonTeams/load",
  async (seasonId: string) => {
    const [seasonResult, assignedResult, stagesResult, catalogTeams] = await Promise.all([
      seasonApi.getSeasonById(seasonId),
      competitionTeamApi.getCompetitionTeamsBySeasonId(seasonId),
      stageApi.getStagesBySeasonId(seasonId),
      loadAllCatalogTeams(),
    ]);

    if (!seasonResult.success || !seasonResult.data) {
      throw new Error(seasonResult.message ?? "Sezon yüklenemedi.");
    }

    if (!assignedResult.success || !assignedResult.data) {
      throw new Error(assignedResult.message ?? "Sezon takımları yüklenemedi.");
    }

    if (!stagesResult.success || !stagesResult.data) {
      throw new Error(stagesResult.message ?? "Aşamalar yüklenemedi.");
    }

    const stages = stagesResult.data;
    const groupStageIds = stages
      .filter((stage) => stage.type === COMPETITION_STAGE_TYPE.GroupStage)
      .map((stage) => stage.id);

    const groupEntries = await Promise.all(
      groupStageIds.map(async (stageId) => {
        const groupsResult = await groupApi.getGroupsByStageId(stageId);
        return [
          stageId,
          groupsResult.success && groupsResult.data ? groupsResult.data : [],
        ] as const;
      }),
    );

    const groupsByStageId = Object.fromEntries(groupEntries);

    return {
      seasonId,
      season: seasonResult.data,
      assignedTeams: assignedResult.data,
      stages,
      catalogTeams,
      groupsByStageId,
    };
  },
);

export const assignTeamToSeason = createAsyncThunk(
  "seasonTeams/assign",
  async (teamId: string, { getState, rejectWithValue }) => {
    const state = getState() as { seasonTeams: SeasonTeamsState };
    const { seasonId, stages } = state.seasonTeams;

    if (!seasonId) {
      return rejectWithValue("Sezon seçili değil.");
    }

    const defaultStage =
      stages.find((stage) => stage.type === COMPETITION_STAGE_TYPE.LeagueTable) ??
      stages[0] ??
      null;

    const result = await competitionTeamApi.createCompetitionTeam({
      competitionSeasonId: seasonId,
      teamId,
      competitionStageId: defaultStage?.id ?? null,
      competitionGroupId: null,
      isActive: true,
    });

    if (!result.success) {
      return rejectWithValue(result.message ?? "Takım atanamadı.");
    }

    return seasonId;
  },
);

export const unassignTeamFromSeason = createAsyncThunk(
  "seasonTeams/unassign",
  async (competitionTeamId: string, { getState, rejectWithValue }) => {
    const state = getState() as { seasonTeams: SeasonTeamsState };
    const { seasonId } = state.seasonTeams;

    if (!seasonId) {
      return rejectWithValue("Sezon seçili değil.");
    }

    const result = await competitionTeamApi.deleteCompetitionTeam(competitionTeamId);

    if (!result.success) {
      return rejectWithValue(result.message ?? "Takım kaldırılamadı.");
    }

    return seasonId;
  },
);

export const updateAssignedTeamPlacement = createAsyncThunk(
  "seasonTeams/updatePlacement",
  async (
    payload: {
      competitionTeamId: string;
      competitionStageId: string | null;
      competitionGroupId: string | null;
    },
    { getState, rejectWithValue },
  ) => {
    const state = getState() as { seasonTeams: SeasonTeamsState };
    const { seasonId, assignedTeams } = state.seasonTeams;
    const existing = assignedTeams.find((item) => item.id === payload.competitionTeamId);

    if (!seasonId || !existing) {
      return rejectWithValue("Atama bulunamadı.");
    }

    const result = await competitionTeamApi.updateCompetitionTeam({
      id: existing.id,
      competitionSeasonId: seasonId,
      teamId: existing.teamId,
      competitionStageId: payload.competitionStageId,
      competitionGroupId: payload.competitionGroupId,
      seed: existing.seed,
      isActive: existing.isActive,
    });

    if (!result.success) {
      return rejectWithValue(result.message ?? "Atama güncellenemedi.");
    }

    return seasonId;
  },
);

const seasonTeamsSlice = createSlice({
  name: "seasonTeams",
  initialState,
  reducers: {
    clearSeasonTeams() {
      return initialState;
    },
    setPoolSearch(state, action: PayloadAction<string>) {
      state.poolSearch = action.payload;
    },
    setCountryFilter(state, action: PayloadAction<string | null>) {
      state.countryFilter = action.payload;
    },
    setSortMode(state, action: PayloadAction<SortMode>) {
      state.sortMode = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadSeasonTeamsPage.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadSeasonTeamsPage.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.seasonId = action.payload.seasonId;
        state.season = action.payload.season;
        state.assignedTeams = action.payload.assignedTeams;
        state.stages = action.payload.stages;
        state.catalogTeams = action.payload.catalogTeams;
        state.groupsByStageId = action.payload.groupsByStageId;
      })
      .addCase(loadSeasonTeamsPage.rejected, (state) => {
        state.status = "failed";
      })
      .addCase(assignTeamToSeason.pending, (state) => {
        state.actionStatus = "loading";
      })
      .addCase(assignTeamToSeason.fulfilled, (state) => {
        state.actionStatus = "succeeded";
      })
      .addCase(assignTeamToSeason.rejected, (state) => {
        state.actionStatus = "failed";
      })
      .addCase(unassignTeamFromSeason.pending, (state) => {
        state.actionStatus = "loading";
      })
      .addCase(unassignTeamFromSeason.fulfilled, (state) => {
        state.actionStatus = "succeeded";
      })
      .addCase(unassignTeamFromSeason.rejected, (state) => {
        state.actionStatus = "failed";
      })
      .addCase(updateAssignedTeamPlacement.pending, (state) => {
        state.actionStatus = "loading";
      })
      .addCase(updateAssignedTeamPlacement.fulfilled, (state) => {
        state.actionStatus = "succeeded";
      })
      .addCase(updateAssignedTeamPlacement.rejected, (state) => {
        state.actionStatus = "failed";
      });
  },
});

export const { clearSeasonTeams, setPoolSearch, setCountryFilter, setSortMode } =
  seasonTeamsSlice.actions;

export default seasonTeamsSlice.reducer;
