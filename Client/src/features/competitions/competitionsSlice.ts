import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import * as competitionApi from "./competitionApi";
import type {
  CompetitionResponseDto,
  CompetitionType,
  CreateCompetitionRequest,
  UpdateCompetitionRequest,
} from "./competitionTypes";
import { COMPETITION_TYPE } from "./competitionTypes";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";
type SaveStatus = "idle" | "saving" | "succeeded" | "failed";
type PanelMode = "closed" | "create" | "edit";

export type CompetitionFormState = {
  name: string;
  country: string;
  type: CompetitionType;
  isActive: boolean;
  logoUrl: string | null;
};

type CompetitionsState = {
  status: LoadStatus;
  saveStatus: SaveStatus;
  items: CompetitionResponseDto[];
  selectedId: string | null;
  panelMode: PanelMode;
  form: CompetitionFormState;
};

const emptyForm: CompetitionFormState = {
  name: "",
  country: "Türkiye",
  type: COMPETITION_TYPE.League,
  isActive: true,
  logoUrl: null,
};

const initialState: CompetitionsState = {
  status: "idle",
  saveStatus: "idle",
  items: [],
  selectedId: null,
  panelMode: "closed",
  form: emptyForm,
};

function formFromCompetition(competition: CompetitionResponseDto): CompetitionFormState {
  return {
    name: competition.name,
    country: competition.country,
    type: competition.type,
    isActive: competition.isActive,
    logoUrl: competition.logoUrl,
  };
}

export const loadCompetitions = createAsyncThunk("competitions/load", async () => {
  const result = await competitionApi.getAllCompetitions(1, 50);

  if (!result.success || !result.data) {
    throw new Error(result.message ?? "Yarışmalar yüklenemedi.");
  }

  return result.data.items;
});

export const saveCompetition = createAsyncThunk(
  "competitions/save",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { competitions: CompetitionsState };
    const { panelMode, selectedId, form } = state.competitions;

    if (!form.name.trim() || !form.country.trim()) {
      return rejectWithValue("Yarışma adı ve ülke zorunludur.");
    }

    if (panelMode === "create") {
      const request: CreateCompetitionRequest = {
        name: form.name.trim(),
        country: form.country.trim(),
        type: form.type,
        logoUrl: form.logoUrl,
        isActive: form.isActive,
      };

      const result = await competitionApi.createCompetition(request);

      if (!result.success) {
        return rejectWithValue(result.message ?? "Yarışma oluşturulamadı.");
      }

      return { mode: "create" as const };
    }

    if (panelMode === "edit" && selectedId) {
      const request: UpdateCompetitionRequest = {
        id: selectedId,
        name: form.name.trim(),
        country: form.country.trim(),
        type: form.type,
        logoUrl: form.logoUrl,
        isActive: form.isActive,
      };

      const result = await competitionApi.updateCompetition(request);

      if (!result.success) {
        return rejectWithValue(result.message ?? "Yarışma güncellenemedi.");
      }

      return { mode: "edit" as const };
    }

    return rejectWithValue("Kaydedilecek bir form yok.");
  },
);

const competitionsSlice = createSlice({
  name: "competitions",
  initialState,
  reducers: {
    clearCompetitions() {
      return initialState;
    },
    openCreatePanel(state) {
      state.panelMode = "create";
      state.selectedId = null;
      state.form = { ...emptyForm };
      state.saveStatus = "idle";
    },
    openEditPanel(state, action: PayloadAction<string>) {
      const competition = state.items.find((item) => item.id === action.payload);

      if (!competition) {
        return;
      }

      state.panelMode = "edit";
      state.selectedId = competition.id;
      state.form = formFromCompetition(competition);
      state.saveStatus = "idle";
    },
    selectCompetition(state, action: PayloadAction<string>) {
      const competition = state.items.find((item) => item.id === action.payload);

      if (!competition) {
        return;
      }

      state.selectedId = competition.id;
      state.panelMode = "edit";
      state.form = formFromCompetition(competition);
      state.saveStatus = "idle";
    },
    closePanel(state) {
      state.panelMode = "closed";
      state.selectedId = null;
      state.form = { ...emptyForm };
      state.saveStatus = "idle";
    },
    updateForm(state, action: PayloadAction<Partial<CompetitionFormState>>) {
      state.form = { ...state.form, ...action.payload };
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadCompetitions.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadCompetitions.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.items = action.payload;
      })
      .addCase(loadCompetitions.rejected, (state) => {
        state.status = "failed";
        state.items = [];
      })
      .addCase(saveCompetition.pending, (state) => {
        state.saveStatus = "saving";
      })
      .addCase(saveCompetition.fulfilled, (state) => {
        state.saveStatus = "succeeded";
        state.panelMode = "closed";
        state.selectedId = null;
        state.form = { ...emptyForm };
      })
      .addCase(saveCompetition.rejected, (state) => {
        state.saveStatus = "failed";
      });
  },
});

export const {
  clearCompetitions,
  openCreatePanel,
  openEditPanel,
  selectCompetition,
  closePanel,
  updateForm,
} = competitionsSlice.actions;

export default competitionsSlice.reducer;
