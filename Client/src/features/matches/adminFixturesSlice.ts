import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import * as competitionTeamApi from "../competitionTeams/competitionTeamApi";
import type { CompetitionTeamPreviewDto } from "../competitionTeams/competitionTeamTypes";
import * as groupApi from "../competitionGroups/competitionGroupApi";
import type { CompetitionGroupPreviewDto } from "../competitionGroups/competitionGroupTypes";
import * as seasonApi from "../competitionSeasons/competitionSeasonApi";
import type {
  CompetitionSeasonPreviewDto,
  CompetitionSeasonResponseDto,
} from "../competitionSeasons/competitionSeasonTypes";
import * as stageApi from "../competitionStages/competitionStageApi";
import type { CompetitionStagePreviewDto } from "../competitionStages/competitionStageTypes";
import { COMPETITION_STAGE_TYPE } from "../competitionStages/competitionStageTypes";
import * as matchApi from "./matchApi";
import type {
  CreateMatchRequest,
  MatchPreviewDto,
  MatchStatus,
  RecordMatchResultRequest,
} from "./matchTypes";
import { MATCH_STATUS } from "./matchTypes";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";
type DialogKind = "none" | "result" | "create" | "menu";

type CreateForm = {
  competitionStageId: string;
  competitionGroupId: string;
  homeTeamId: string;
  awayTeamId: string;
  kickoffTime: string;
  round: string;
  venue: string;
};

type ResultForm = {
  homeScore: string;
  awayScore: string;
  advancingTeamId: string;
};

type AdminFixturesState = {
  status: LoadStatus;
  actionStatus: LoadStatus;
  seasonId: string | null;
  season: CompetitionSeasonResponseDto | null;
  activeSeasons: CompetitionSeasonPreviewDto[];
  matches: MatchPreviewDto[];
  stages: CompetitionStagePreviewDto[];
  groupsByStageId: Record<string, CompetitionGroupPreviewDto[]>;
  seasonTeams: CompetitionTeamPreviewDto[];
  stageFilter: string;
  statusFilter: MatchStatus | "all";
  dialog: DialogKind;
  selectedMatchId: string | null;
  createForm: CreateForm;
  resultForm: ResultForm;
};

const emptyCreateForm = (): CreateForm => ({
  competitionStageId: "",
  competitionGroupId: "",
  homeTeamId: "",
  awayTeamId: "",
  kickoffTime: "",
  round: "",
  venue: "",
});

const emptyResultForm = (): ResultForm => ({
  homeScore: "0",
  awayScore: "0",
  advancingTeamId: "",
});

const initialState: AdminFixturesState = {
  status: "idle",
  actionStatus: "idle",
  seasonId: null,
  season: null,
  activeSeasons: [],
  matches: [],
  stages: [],
  groupsByStageId: {},
  seasonTeams: [],
  stageFilter: "all",
  statusFilter: "all",
  dialog: "none",
  selectedMatchId: null,
  createForm: emptyCreateForm(),
  resultForm: emptyResultForm(),
};

export const loadActiveSeasonsForFixtures = createAsyncThunk(
  "adminFixtures/loadActiveSeasons",
  async () => {
    const result = await seasonApi.getActiveSeasons();

    if (!result.success || !result.data) {
      throw new Error(result.message ?? "Aktif sezonlar yüklenemedi.");
    }

    return result.data;
  },
);

export const loadAdminFixturesPage = createAsyncThunk(
  "adminFixtures/load",
  async (seasonId: string) => {
    const [seasonResult, matchesResult, stagesResult, teamsResult] = await Promise.all([
      seasonApi.getSeasonById(seasonId),
      matchApi.getMatchesBySeason(seasonId),
      stageApi.getStagesBySeasonId(seasonId),
      competitionTeamApi.getCompetitionTeamsBySeasonId(seasonId),
    ]);

    if (!seasonResult.success || !seasonResult.data) {
      throw new Error(seasonResult.message ?? "Sezon yüklenemedi.");
    }

    if (!matchesResult.success || !matchesResult.data) {
      throw new Error(matchesResult.message ?? "Maçlar yüklenemedi.");
    }

    if (!stagesResult.success || !stagesResult.data) {
      throw new Error(stagesResult.message ?? "Aşamalar yüklenemedi.");
    }

    if (!teamsResult.success || !teamsResult.data) {
      throw new Error(teamsResult.message ?? "Sezon takımları yüklenemedi.");
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

    return {
      seasonId,
      season: seasonResult.data,
      matches: matchesResult.data,
      stages,
      seasonTeams: teamsResult.data.filter((item) => item.isActive),
      groupsByStageId: Object.fromEntries(groupEntries),
    };
  },
);

export const createFixture = createAsyncThunk(
  "adminFixtures/create",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { adminFixtures: AdminFixturesState };
    const { seasonId, createForm } = state.adminFixtures;

    if (!seasonId) {
      return rejectWithValue("Sezon seçilmedi.");
    }

    if (!createForm.homeTeamId || !createForm.awayTeamId) {
      return rejectWithValue("Ev sahibi ve deplasman takımları zorunludur.");
    }

    if (createForm.homeTeamId === createForm.awayTeamId) {
      return rejectWithValue("Ev sahibi ve deplasman takımları farklı olmalıdır.");
    }

    if (!createForm.kickoffTime) {
      return rejectWithValue("Başlama saati zorunludur.");
    }

    const roundValue = createForm.round.trim()
      ? Number.parseInt(createForm.round, 10)
      : null;

    if (createForm.round.trim() && (!roundValue || roundValue < 1)) {
      return rejectWithValue("Hafta numarası 1 veya daha büyük olmalıdır.");
    }

    const request: CreateMatchRequest = {
      competitionSeasonId: seasonId,
      competitionStageId: createForm.competitionStageId || null,
      competitionGroupId: createForm.competitionGroupId || null,
      homeTeamId: createForm.homeTeamId,
      awayTeamId: createForm.awayTeamId,
      kickoffTime: new Date(createForm.kickoffTime).toISOString(),
      round: roundValue,
      venue: createForm.venue.trim() || null,
      status: MATCH_STATUS.Scheduled,
    };

    const result = await matchApi.createMatch(request);

    if (!result.success) {
      return rejectWithValue(result.message ?? "Maç oluşturulamadı.");
    }

    return result.data;
  },
);

export const saveMatchResult = createAsyncThunk(
  "adminFixtures/recordResult",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { adminFixtures: AdminFixturesState };
    const { selectedMatchId, resultForm, matches } = state.adminFixtures;
    const match = matches.find((item) => item.id === selectedMatchId);

    if (!match) {
      return rejectWithValue("Maç bulunamadı.");
    }

    const homeScore = Number.parseInt(resultForm.homeScore, 10);
    const awayScore = Number.parseInt(resultForm.awayScore, 10);

    if (Number.isNaN(homeScore) || homeScore < 0 || Number.isNaN(awayScore) || awayScore < 0) {
      return rejectWithValue("Skorlar 0 veya daha büyük olmalıdır.");
    }

    const isKnockout =
      match.competitionStage?.type === COMPETITION_STAGE_TYPE.Knockout ||
      match.competitionStage?.type === COMPETITION_STAGE_TYPE.Final;

    let advancingTeamId: string | null = null;

    if (isKnockout && homeScore === awayScore) {
      if (!resultForm.advancingTeamId) {
        return rejectWithValue("Beraberlikte tur atlayan takımı seçin.");
      }

      advancingTeamId = resultForm.advancingTeamId;
    }

    const request: RecordMatchResultRequest = {
      id: match.id,
      homeScore,
      awayScore,
      advancingTeamId,
    };

    const result = await matchApi.recordMatchResult(request);

    if (!result.success) {
      return rejectWithValue(result.message ?? "Sonuç kaydedilemedi.");
    }

    return match.id;
  },
);

export const removeFixture = createAsyncThunk(
  "adminFixtures/delete",
  async (matchId: string, { rejectWithValue }) => {
    const result = await matchApi.deleteMatch(matchId);

    if (!result.success) {
      return rejectWithValue(result.message ?? "Maç silinemedi.");
    }

    return matchId;
  },
);

const adminFixturesSlice = createSlice({
  name: "adminFixtures",
  initialState,
  reducers: {
    clearAdminFixtures() {
      return initialState;
    },
    setStageFilter(state, action: PayloadAction<string>) {
      state.stageFilter = action.payload;
    },
    setStatusFilter(state, action: PayloadAction<MatchStatus | "all">) {
      state.statusFilter = action.payload;
    },
    openCreateDialog(state) {
      const firstStage = state.stages[0];
      state.dialog = "create";
      state.selectedMatchId = null;
      state.createForm = {
        ...emptyCreateForm(),
        competitionStageId: firstStage?.id ?? "",
        kickoffTime: "",
      };
    },
    openResultDialog(state, action: PayloadAction<string>) {
      const match = state.matches.find((item) => item.id === action.payload);

      if (!match) {
        return;
      }

      state.dialog = "result";
      state.selectedMatchId = match.id;
      state.resultForm = {
        homeScore: String(match.homeScore ?? 0),
        awayScore: String(match.awayScore ?? 0),
        advancingTeamId: match.advancingTeamId ?? match.homeTeamId,
      };
    },
    openMatchMenu(state, action: PayloadAction<string>) {
      state.dialog = "menu";
      state.selectedMatchId = action.payload;
    },
    closeDialog(state) {
      state.dialog = "none";
      state.selectedMatchId = null;
      state.createForm = emptyCreateForm();
      state.resultForm = emptyResultForm();
    },
    updateCreateForm(state, action: PayloadAction<Partial<CreateForm>>) {
      state.createForm = { ...state.createForm, ...action.payload };

      if (action.payload.competitionStageId !== undefined) {
        state.createForm.competitionGroupId = "";
      }
    },
    updateResultForm(state, action: PayloadAction<Partial<ResultForm>>) {
      state.resultForm = { ...state.resultForm, ...action.payload };
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadActiveSeasonsForFixtures.fulfilled, (state, action) => {
        state.activeSeasons = action.payload;
      })
      .addCase(loadAdminFixturesPage.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadAdminFixturesPage.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.seasonId = action.payload.seasonId;
        state.season = action.payload.season;
        state.matches = action.payload.matches;
        state.stages = action.payload.stages;
        state.seasonTeams = action.payload.seasonTeams;
        state.groupsByStageId = action.payload.groupsByStageId;
      })
      .addCase(loadAdminFixturesPage.rejected, (state) => {
        state.status = "failed";
      })
      .addCase(createFixture.pending, (state) => {
        state.actionStatus = "loading";
      })
      .addCase(createFixture.fulfilled, (state) => {
        state.actionStatus = "succeeded";
        state.dialog = "none";
        state.createForm = emptyCreateForm();
      })
      .addCase(createFixture.rejected, (state) => {
        state.actionStatus = "failed";
      })
      .addCase(saveMatchResult.pending, (state) => {
        state.actionStatus = "loading";
      })
      .addCase(saveMatchResult.fulfilled, (state) => {
        state.actionStatus = "succeeded";
        state.dialog = "none";
        state.selectedMatchId = null;
        state.resultForm = emptyResultForm();
      })
      .addCase(saveMatchResult.rejected, (state) => {
        state.actionStatus = "failed";
      })
      .addCase(removeFixture.pending, (state) => {
        state.actionStatus = "loading";
      })
      .addCase(removeFixture.fulfilled, (state, action) => {
        state.actionStatus = "succeeded";
        state.matches = state.matches.filter((match) => match.id !== action.payload);
        state.dialog = "none";
        state.selectedMatchId = null;
      })
      .addCase(removeFixture.rejected, (state) => {
        state.actionStatus = "failed";
      });
  },
});

export const {
  clearAdminFixtures,
  setStageFilter,
  setStatusFilter,
  openCreateDialog,
  openResultDialog,
  openMatchMenu,
  closeDialog,
  updateCreateForm,
  updateResultForm,
} = adminFixturesSlice.actions;

export default adminFixturesSlice.reducer;
