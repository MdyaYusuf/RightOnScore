import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import * as matchApi from "../matches/matchApi";
import type { MatchResponseDto } from "../matches/matchTypes";
import {
  ADMIN_PREDICTIONS_PAGE_SIZE,
  type PredictionSortKey,
  type SortDirection,
} from "./adminPredictionsHelpers";
import * as predictionApi from "./matchPredictionApi";
import type { MatchPredictionResponseDto } from "./matchPredictionTypes";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";

type AdminPredictionsState = {
  status: LoadStatus;
  matchId: string | null;
  match: MatchResponseDto | null;
  predictions: MatchPredictionResponseDto[];
  sortKey: PredictionSortKey;
  sortDirection: SortDirection;
  pageNumber: number;
  pageSize: number;
};

const initialState: AdminPredictionsState = {
  status: "idle",
  matchId: null,
  match: null,
  predictions: [],
  sortKey: "points",
  sortDirection: "desc",
  pageNumber: 1,
  pageSize: ADMIN_PREDICTIONS_PAGE_SIZE,
};

export const loadAdminPredictionsPage = createAsyncThunk(
  "adminPredictions/load",
  async (matchId: string) => {
    const [matchResult, predictionsResult] = await Promise.all([
      matchApi.getMatchById(matchId),
      predictionApi.getPredictionsByMatchId(matchId),
    ]);

    if (!matchResult.success || !matchResult.data) {
      throw new Error(matchResult.message ?? "Maç yüklenemedi.");
    }

    if (!predictionsResult.success || !predictionsResult.data) {
      throw new Error(predictionsResult.message ?? "Tahminler yüklenemedi.");
    }

    return {
      matchId,
      match: matchResult.data,
      predictions: predictionsResult.data,
    };
  },
);

const adminPredictionsSlice = createSlice({
  name: "adminPredictions",
  initialState,
  reducers: {
    clearAdminPredictions() {
      return initialState;
    },
    setSort(state, action: PayloadAction<PredictionSortKey>) {
      if (state.sortKey === action.payload) {
        state.sortDirection = state.sortDirection === "asc" ? "desc" : "asc";
      } else {
        state.sortKey = action.payload;
        state.sortDirection = action.payload === "points" ? "desc" : "asc";
      }

      state.pageNumber = 1;
    },
    setPageNumber(state, action: PayloadAction<number>) {
      state.pageNumber = Math.max(1, action.payload);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadAdminPredictionsPage.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadAdminPredictionsPage.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.matchId = action.payload.matchId;
        state.match = action.payload.match;
        state.predictions = action.payload.predictions;
        state.pageNumber = 1;
      })
      .addCase(loadAdminPredictionsPage.rejected, (state) => {
        state.status = "failed";
        state.match = null;
        state.predictions = [];
      });
  },
});

export const { clearAdminPredictions, setSort, setPageNumber } =
  adminPredictionsSlice.actions;

export default adminPredictionsSlice.reducer;
