import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import * as competitionApi from "../competitions/competitionApi";
import type { CompetitionResponseDto } from "../competitions/competitionTypes";
import * as groupApi from "../competitionGroups/competitionGroupApi";
import type { CompetitionGroupPreviewDto } from "../competitionGroups/competitionGroupTypes";
import * as seasonApi from "../competitionSeasons/competitionSeasonApi";
import type {
  CompetitionSeasonResponseDto,
  CompetitionSeasonStatus,
  CreateCompetitionSeasonRequest,
} from "../competitionSeasons/competitionSeasonTypes";
import { COMPETITION_SEASON_STATUS } from "../competitionSeasons/competitionSeasonTypes";
import * as stageApi from "../competitionStages/competitionStageApi";
import type {
  CompetitionStagePreviewDto,
  CompetitionStageType,
  CreateCompetitionStageRequest,
} from "../competitionStages/competitionStageTypes";
import {
  COMPETITION_STAGE_STATUS,
  COMPETITION_STAGE_TYPE,
} from "../competitionStages/competitionStageTypes";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";
type DialogKind = "none" | "season" | "stage" | "group";

type SeasonForm = {
  name: string;
  startDate: string;
  endDate: string;
  status: CompetitionSeasonStatus;
  isActive: boolean;
};

type StageForm = {
  name: string;
  type: CompetitionStageType;
  startDate: string;
  endDate: string;
};

type GroupForm = {
  name: string;
};

type SeasonStructureState = {
  competitionId: string | null;
  competition: CompetitionResponseDto | null;
  status: LoadStatus;
  seasons: CompetitionSeasonResponseDto[];
  stages: CompetitionStagePreviewDto[];
  groups: CompetitionGroupPreviewDto[];
  selectedSeasonId: string | null;
  selectedStageId: string | null;
  stagesStatus: LoadStatus;
  groupsStatus: LoadStatus;
  saveStatus: LoadStatus;
  dialog: DialogKind;
  seasonForm: SeasonForm;
  stageForm: StageForm;
  groupForm: GroupForm;
};

function todayIsoDate(): string {
  return new Date().toISOString().slice(0, 10);
}

function plusMonthsIsoDate(months: number): string {
  const date = new Date();
  date.setMonth(date.getMonth() + months);
  return date.toISOString().slice(0, 10);
}

const emptySeasonForm = (): SeasonForm => ({
  name: "",
  startDate: todayIsoDate(),
  endDate: plusMonthsIsoDate(9),
  status: COMPETITION_SEASON_STATUS.Upcoming,
  isActive: false,
});

const emptyStageForm = (): StageForm => ({
  name: "",
  type: COMPETITION_STAGE_TYPE.LeagueTable,
  startDate: todayIsoDate(),
  endDate: plusMonthsIsoDate(6),
});

const emptyGroupForm = (): GroupForm => ({
  name: "",
});

const initialState: SeasonStructureState = {
  competitionId: null,
  competition: null,
  status: "idle",
  seasons: [],
  stages: [],
  groups: [],
  selectedSeasonId: null,
  selectedStageId: null,
  stagesStatus: "idle",
  groupsStatus: "idle",
  saveStatus: "idle",
  dialog: "none",
  seasonForm: emptySeasonForm(),
  stageForm: emptyStageForm(),
  groupForm: emptyGroupForm(),
};

export const loadSeasonStructure = createAsyncThunk(
  "seasonStructure/load",
  async (competitionId: string) => {
    const [competitionResult, seasonsResult] = await Promise.all([
      competitionApi.getCompetitionById(competitionId),
      seasonApi.getSeasonsByCompetitionId(competitionId),
    ]);

    if (!competitionResult.success || !competitionResult.data) {
      throw new Error(competitionResult.message ?? "Yarışma yüklenemedi.");
    }

    if (!seasonsResult.success || !seasonsResult.data) {
      throw new Error(seasonsResult.message ?? "Sezonlar yüklenemedi.");
    }

    const fullSeasons = await Promise.all(
      seasonsResult.data.map(async (preview) => {
        const detail = await seasonApi.getSeasonById(preview.id);
        return detail.success && detail.data ? detail.data : null;
      }),
    );

    const seasons = fullSeasons.filter(
      (season): season is CompetitionSeasonResponseDto => season !== null,
    );

    return {
      competitionId,
      competition: competitionResult.data,
      seasons,
    };
  },
);

export const loadStagesForSeason = createAsyncThunk(
  "seasonStructure/loadStages",
  async (seasonId: string) => {
    const result = await stageApi.getStagesBySeasonId(seasonId);

    if (!result.success || !result.data) {
      throw new Error(result.message ?? "Aşamalar yüklenemedi.");
    }

    return { seasonId, stages: result.data };
  },
);

export const loadGroupsForStage = createAsyncThunk(
  "seasonStructure/loadGroups",
  async (stageId: string) => {
    const result = await groupApi.getGroupsByStageId(stageId);

    if (!result.success || !result.data) {
      throw new Error(result.message ?? "Gruplar yüklenemedi.");
    }

    return { stageId, groups: result.data };
  },
);

export const createSeasonForCompetition = createAsyncThunk(
  "seasonStructure/createSeason",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { seasonStructure: SeasonStructureState };
    const { competitionId, seasonForm } = state.seasonStructure;

    if (!competitionId) {
      return rejectWithValue("Yarışma seçili değil.");
    }

    if (!seasonForm.name.trim()) {
      return rejectWithValue("Sezon adı zorunludur.");
    }

    const request: CreateCompetitionSeasonRequest = {
      competitionId,
      name: seasonForm.name.trim(),
      startDate: `${seasonForm.startDate}T12:00:00.000Z`,
      endDate: `${seasonForm.endDate}T12:00:00.000Z`,
      status: seasonForm.status,
      isActive: seasonForm.isActive,
    };

    const result = await seasonApi.createSeason(request);

    if (!result.success) {
      return rejectWithValue(result.message ?? "Sezon oluşturulamadı.");
    }

    return competitionId;
  },
);

export const createStageForSeason = createAsyncThunk(
  "seasonStructure/createStage",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { seasonStructure: SeasonStructureState };
    const { selectedSeasonId, stages, stageForm } = state.seasonStructure;

    if (!selectedSeasonId) {
      return rejectWithValue("Sezon seçili değil.");
    }

    if (!stageForm.name.trim()) {
      return rejectWithValue("Aşama adı zorunludur.");
    }

    const request: CreateCompetitionStageRequest = {
      competitionSeasonId: selectedSeasonId,
      name: stageForm.name.trim(),
      type: stageForm.type,
      displayOrder: stages.length + 1,
      startDate: `${stageForm.startDate}T12:00:00.000Z`,
      endDate: `${stageForm.endDate}T12:00:00.000Z`,
      status: COMPETITION_STAGE_STATUS.Upcoming,
      isActive: true,
    };

    const result = await stageApi.createStage(request);

    if (!result.success) {
      return rejectWithValue(result.message ?? "Aşama oluşturulamadı.");
    }

    return selectedSeasonId;
  },
);

export const createGroupForStage = createAsyncThunk(
  "seasonStructure/createGroup",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { seasonStructure: SeasonStructureState };
    const { selectedStageId, groups, groupForm, stages } = state.seasonStructure;

    if (!selectedStageId) {
      return rejectWithValue("Aşama seçili değil.");
    }

    const stage = stages.find((item) => item.id === selectedStageId);

    if (stage && stage.type !== COMPETITION_STAGE_TYPE.GroupStage) {
      return rejectWithValue("Gruplar yalnızca grup aşamasına eklenebilir.");
    }

    if (!groupForm.name.trim()) {
      return rejectWithValue("Grup adı zorunludur.");
    }

    const result = await groupApi.createGroup({
      competitionStageId: selectedStageId,
      name: groupForm.name.trim(),
      displayOrder: groups.length + 1,
      isActive: true,
    });

    if (!result.success) {
      return rejectWithValue(result.message ?? "Grup oluşturulamadı.");
    }

    return selectedStageId;
  },
);

const seasonStructureSlice = createSlice({
  name: "seasonStructure",
  initialState,
  reducers: {
    clearSeasonStructure() {
      return initialState;
    },
    selectSeason(state, action: PayloadAction<string>) {
      state.selectedSeasonId = action.payload;
      state.selectedStageId = null;
      state.stages = [];
      state.groups = [];
      state.stagesStatus = "idle";
      state.groupsStatus = "idle";
    },
    selectStage(state, action: PayloadAction<string>) {
      state.selectedStageId = action.payload;
      state.groups = [];
      state.groupsStatus = "idle";
    },
    openDialog(state, action: PayloadAction<DialogKind>) {
      state.dialog = action.payload;
      state.seasonForm = emptySeasonForm();
      state.stageForm = emptyStageForm();
      state.groupForm = emptyGroupForm();
    },
    closeDialog(state) {
      state.dialog = "none";
    },
    updateSeasonForm(state, action: PayloadAction<Partial<SeasonForm>>) {
      state.seasonForm = { ...state.seasonForm, ...action.payload };
    },
    updateStageForm(state, action: PayloadAction<Partial<StageForm>>) {
      state.stageForm = { ...state.stageForm, ...action.payload };
    },
    updateGroupForm(state, action: PayloadAction<Partial<GroupForm>>) {
      state.groupForm = { ...state.groupForm, ...action.payload };
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadSeasonStructure.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadSeasonStructure.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.competitionId = action.payload.competitionId;
        state.competition = action.payload.competition;
        state.seasons = action.payload.seasons;

        const preferred =
          action.payload.seasons.find((season) => season.isActive) ??
          action.payload.seasons[0] ??
          null;

        state.selectedSeasonId = preferred?.id ?? null;
        state.selectedStageId = null;
        state.stages = [];
        state.groups = [];
      })
      .addCase(loadSeasonStructure.rejected, (state) => {
        state.status = "failed";
        state.seasons = [];
      })
      .addCase(loadStagesForSeason.pending, (state) => {
        state.stagesStatus = "loading";
      })
      .addCase(loadStagesForSeason.fulfilled, (state, action) => {
        state.stagesStatus = "succeeded";
        state.stages = action.payload.stages;
        state.selectedStageId = action.payload.stages[0]?.id ?? null;
        state.groups = [];
      })
      .addCase(loadStagesForSeason.rejected, (state) => {
        state.stagesStatus = "failed";
        state.stages = [];
      })
      .addCase(loadGroupsForStage.pending, (state) => {
        state.groupsStatus = "loading";
      })
      .addCase(loadGroupsForStage.fulfilled, (state, action) => {
        state.groupsStatus = "succeeded";
        state.groups = action.payload.groups;
      })
      .addCase(loadGroupsForStage.rejected, (state) => {
        state.groupsStatus = "failed";
        state.groups = [];
      })
      .addCase(createSeasonForCompetition.pending, (state) => {
        state.saveStatus = "loading";
      })
      .addCase(createSeasonForCompetition.fulfilled, (state) => {
        state.saveStatus = "succeeded";
        state.dialog = "none";
      })
      .addCase(createSeasonForCompetition.rejected, (state) => {
        state.saveStatus = "failed";
      })
      .addCase(createStageForSeason.pending, (state) => {
        state.saveStatus = "loading";
      })
      .addCase(createStageForSeason.fulfilled, (state) => {
        state.saveStatus = "succeeded";
        state.dialog = "none";
      })
      .addCase(createStageForSeason.rejected, (state) => {
        state.saveStatus = "failed";
      })
      .addCase(createGroupForStage.pending, (state) => {
        state.saveStatus = "loading";
      })
      .addCase(createGroupForStage.fulfilled, (state) => {
        state.saveStatus = "succeeded";
        state.dialog = "none";
      })
      .addCase(createGroupForStage.rejected, (state) => {
        state.saveStatus = "failed";
      });
  },
});

export const {
  clearSeasonStructure,
  selectSeason,
  selectStage,
  openDialog,
  closeDialog,
  updateSeasonForm,
  updateStageForm,
  updateGroupForm,
} = seasonStructureSlice.actions;

export default seasonStructureSlice.reducer;
