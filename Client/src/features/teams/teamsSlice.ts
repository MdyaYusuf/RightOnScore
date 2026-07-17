import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import * as teamApi from "./teamApi";
import type { TeamResponseDto } from "./teamTypes";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";
type SaveStatus = "idle" | "saving" | "succeeded" | "failed";
type PanelMode = "closed" | "create" | "edit";

export type TeamFormState = {
  name: string;
  shortName: string;
  country: string;
  isActive: boolean;
  crestUrl: string | null;
  crestFile: File | null;
  crestPreviewUrl: string | null;
};

type TeamsState = {
  status: LoadStatus;
  saveStatus: SaveStatus;
  items: TeamResponseDto[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  searchQuery: string;
  isSearchMode: boolean;
  selectedId: string | null;
  panelMode: PanelMode;
  form: TeamFormState;
};

const emptyForm = (): TeamFormState => ({
  name: "",
  shortName: "",
  country: "Türkiye",
  isActive: true,
  crestUrl: null,
  crestFile: null,
  crestPreviewUrl: null,
});

const initialState: TeamsState = {
  status: "idle",
  saveStatus: "idle",
  items: [],
  pageNumber: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 1,
  searchQuery: "",
  isSearchMode: false,
  selectedId: null,
  panelMode: "closed",
  form: emptyForm(),
};

function formFromTeam(team: TeamResponseDto): TeamFormState {
  return {
    name: team.name,
    shortName: team.shortName,
    country: team.country,
    isActive: team.isActive,
    crestUrl: team.crestUrl,
    crestFile: null,
    crestPreviewUrl: null,
  };
}

function revokePreview(url: string | null) {
  if (url) {
    URL.revokeObjectURL(url);
  }
}

export const loadTeams = createAsyncThunk(
  "teams/load",
  async (pageNumber: number, { getState }) => {
    const state = getState() as { teams: TeamsState };
    const result = await teamApi.getAllTeams(pageNumber, state.teams.pageSize);

    if (!result.success || !result.data) {
      throw new Error(result.message ?? "Takımlar yüklenemedi.");
    }

    return result.data;
  },
);

export const searchTeams = createAsyncThunk(
  "teams/search",
  async (searchTerm: string) => {
    const trimmed = searchTerm.trim();

    if (trimmed.length < 2) {
      throw new Error("Arama için en az 2 karakter girin.");
    }

    const result = await teamApi.searchTeams(trimmed);

    if (!result.success || !result.data) {
      throw new Error(result.message ?? "Arama başarısız.");
    }

    const detailed = await Promise.all(
      result.data.map(async (preview) => {
        const detail = await teamApi.getTeamById(preview.id);
        return detail.success && detail.data ? detail.data : null;
      }),
    );

    return detailed.filter((team): team is TeamResponseDto => team !== null);
  },
);

export const saveTeam = createAsyncThunk(
  "teams/save",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { teams: TeamsState };
    const { panelMode, selectedId, form } = state.teams;

    if (!form.name.trim() || !form.shortName.trim() || !form.country.trim()) {
      return rejectWithValue("Takım adı, kısa ad ve ülke zorunludur.");
    }

    if (panelMode === "create") {
      const result = await teamApi.createTeam({
        name: form.name.trim(),
        shortName: form.shortName.trim(),
        country: form.country.trim(),
        isActive: form.isActive,
        crestFile: form.crestFile,
      });

      if (!result.success) {
        return rejectWithValue(result.message ?? "Takım oluşturulamadı.");
      }

      return { mode: "create" as const };
    }

    if (panelMode === "edit" && selectedId) {
      const result = await teamApi.updateTeam({
        id: selectedId,
        name: form.name.trim(),
        shortName: form.shortName.trim(),
        country: form.country.trim(),
        isActive: form.isActive,
        crestFile: form.crestFile,
      });

      if (!result.success) {
        return rejectWithValue(result.message ?? "Takım güncellenemedi.");
      }

      return { mode: "edit" as const };
    }

    return rejectWithValue("Kaydedilecek bir form yok.");
  },
);

export const removeTeam = createAsyncThunk(
  "teams/remove",
  async (_, { getState, rejectWithValue }) => {
    const state = getState() as { teams: TeamsState };
    const { selectedId, panelMode } = state.teams;

    if (panelMode !== "edit" || !selectedId) {
      return rejectWithValue("Silinecek takım seçili değil.");
    }

    const result = await teamApi.deleteTeam(selectedId);

    if (!result.success) {
      return rejectWithValue(result.message ?? "Takım silinemedi.");
    }

    return selectedId;
  },
);

const teamsSlice = createSlice({
  name: "teams",
  initialState,
  reducers: {
    clearTeams(state) {
      revokePreview(state.form.crestPreviewUrl);
      return initialState;
    },
    setSearchQuery(state, action: PayloadAction<string>) {
      state.searchQuery = action.payload;
    },
    clearSearchMode(state) {
      state.isSearchMode = false;
      state.searchQuery = "";
    },
    openCreatePanel(state) {
      revokePreview(state.form.crestPreviewUrl);
      state.panelMode = "create";
      state.selectedId = null;
      state.form = emptyForm();
      state.saveStatus = "idle";
    },
    openEditPanel(state, action: PayloadAction<string>) {
      const team = state.items.find((item) => item.id === action.payload);

      if (!team) {
        return;
      }

      revokePreview(state.form.crestPreviewUrl);
      state.panelMode = "edit";
      state.selectedId = team.id;
      state.form = formFromTeam(team);
      state.saveStatus = "idle";
    },
    selectTeam(state, action: PayloadAction<string>) {
      const team = state.items.find((item) => item.id === action.payload);

      if (!team) {
        return;
      }

      revokePreview(state.form.crestPreviewUrl);
      state.selectedId = team.id;
      state.panelMode = "edit";
      state.form = formFromTeam(team);
      state.saveStatus = "idle";
    },
    closePanel(state) {
      revokePreview(state.form.crestPreviewUrl);
      state.panelMode = "closed";
      state.selectedId = null;
      state.form = emptyForm();
      state.saveStatus = "idle";
    },
    updateForm(state, action: PayloadAction<Partial<TeamFormState>>) {
      if (
        "crestPreviewUrl" in action.payload &&
        action.payload.crestPreviewUrl !== state.form.crestPreviewUrl
      ) {
        revokePreview(state.form.crestPreviewUrl);
      }

      state.form = { ...state.form, ...action.payload };
    },
    setCrestFile(state, action: PayloadAction<File | null>) {
      revokePreview(state.form.crestPreviewUrl);

      if (!action.payload) {
        state.form.crestFile = null;
        state.form.crestPreviewUrl = null;
        return;
      }

      state.form.crestFile = action.payload;
      state.form.crestPreviewUrl = URL.createObjectURL(action.payload);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadTeams.pending, (state) => {
        state.status = "loading";
        state.isSearchMode = false;
      })
      .addCase(loadTeams.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.items = action.payload.items;
        state.pageNumber = action.payload.currentPage;
        state.pageSize = action.payload.pageSize;
        state.totalCount = action.payload.totalCount;
        state.totalPages = Math.max(action.payload.totalPages, 1);
      })
      .addCase(loadTeams.rejected, (state) => {
        state.status = "failed";
        state.items = [];
      })
      .addCase(searchTeams.pending, (state) => {
        state.status = "loading";
        state.isSearchMode = true;
      })
      .addCase(searchTeams.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.items = action.payload;
        state.totalCount = action.payload.length;
        state.totalPages = 1;
        state.pageNumber = 1;
      })
      .addCase(searchTeams.rejected, (state) => {
        state.status = "failed";
        state.items = [];
      })
      .addCase(saveTeam.pending, (state) => {
        state.saveStatus = "saving";
      })
      .addCase(saveTeam.fulfilled, (state) => {
        state.saveStatus = "succeeded";
        revokePreview(state.form.crestPreviewUrl);
        state.panelMode = "closed";
        state.selectedId = null;
        state.form = emptyForm();
      })
      .addCase(saveTeam.rejected, (state) => {
        state.saveStatus = "failed";
      })
      .addCase(removeTeam.pending, (state) => {
        state.saveStatus = "saving";
      })
      .addCase(removeTeam.fulfilled, (state) => {
        state.saveStatus = "succeeded";
        revokePreview(state.form.crestPreviewUrl);
        state.panelMode = "closed";
        state.selectedId = null;
        state.form = emptyForm();
      })
      .addCase(removeTeam.rejected, (state) => {
        state.saveStatus = "failed";
      });
  },
});

export const {
  clearTeams,
  setSearchQuery,
  clearSearchMode,
  openCreatePanel,
  openEditPanel,
  selectTeam,
  closePanel,
  updateForm,
  setCrestFile,
} = teamsSlice.actions;

export default teamsSlice.reducer;
