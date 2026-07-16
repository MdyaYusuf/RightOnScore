import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import * as competitionSeasonApi from "./competitionSeasonApi";
import * as seasonStandingApi from "../seasonStandings/seasonStandingApi";
import {
  readStoredSelectedSeasonId,
  writeStoredSelectedSeasonId,
  type CompetitionSeasonPreviewDto,
  type CompetitionSeasonResponseDto,
  type MySeasonStandingDto,
} from "./competitionSeasonTypes";

type SeasonHubStatus = "idle" | "loading" | "ready" | "error";

type SeasonHubState = {
  status: SeasonHubStatus;
  seasons: CompetitionSeasonPreviewDto[];
  selectedSeasonId: string | null;
  selectedSeason: CompetitionSeasonResponseDto | null;
  myStanding: MySeasonStandingDto | null;
  detailStatus: "idle" | "loading" | "ready";
};

const initialState: SeasonHubState = {
  status: "idle",
  seasons: [],
  selectedSeasonId: readStoredSelectedSeasonId(),
  selectedSeason: null,
  myStanding: null,
  detailStatus: "idle",
};

export const loadActiveSeasons = createAsyncThunk(
  "seasonHub/loadActiveSeasons",
  async (_, { rejectWithValue }) => {
    const result = await competitionSeasonApi.getActiveSeasons();

    if (!result.success || !result.data) {
      return rejectWithValue(result.message ?? "Aktif sezonlar yüklenemedi.");
    }

    return result.data;
  },
);

export const loadSelectedSeasonDetails = createAsyncThunk(
  "seasonHub/loadSelectedSeasonDetails",
  async (competitionSeasonId: string, { rejectWithValue }) => {
    const [seasonResult, standingResult] = await Promise.all([
      competitionSeasonApi.getSeasonById(competitionSeasonId),
      seasonStandingApi.getMySeasonStanding(competitionSeasonId),
    ]);

    if (!seasonResult.success || !seasonResult.data) {
      return rejectWithValue(seasonResult.message ?? "Sezon detayı yüklenemedi.");
    }

    return {
      season: seasonResult.data,
      myStanding: standingResult.success ? standingResult.data : null,
    };
  },
);

const seasonHubSlice = createSlice({
  name: "seasonHub",
  initialState,
  reducers: {
    selectSeason(state, action: PayloadAction<string>) {
      state.selectedSeasonId = action.payload;
      writeStoredSelectedSeasonId(action.payload);
    },
    clearSeasonHub(state) {
      state.status = "idle";
      state.seasons = [];
      state.selectedSeasonId = null;
      state.selectedSeason = null;
      state.myStanding = null;
      state.detailStatus = "idle";
      writeStoredSelectedSeasonId(null);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadActiveSeasons.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadActiveSeasons.fulfilled, (state, action) => {
        state.status = "ready";
        state.seasons = action.payload;

        const storedStillValid = action.payload.some(
          (season) => season.id === state.selectedSeasonId,
        );

        if (!storedStillValid) {
          const firstSeasonId = action.payload[0]?.id ?? null;
          state.selectedSeasonId = firstSeasonId;
          writeStoredSelectedSeasonId(firstSeasonId);
        }
      })
      .addCase(loadActiveSeasons.rejected, (state) => {
        state.status = "error";
        state.seasons = [];
      })
      .addCase(loadSelectedSeasonDetails.pending, (state) => {
        state.detailStatus = "loading";
      })
      .addCase(loadSelectedSeasonDetails.fulfilled, (state, action) => {
        state.detailStatus = "ready";
        state.selectedSeason = action.payload.season;
        state.myStanding = action.payload.myStanding;
      })
      .addCase(loadSelectedSeasonDetails.rejected, (state) => {
        state.detailStatus = "ready";
        state.selectedSeason = null;
        state.myStanding = null;
      });
  },
});

export const { selectSeason, clearSeasonHub } = seasonHubSlice.actions;
export default seasonHubSlice.reducer;
