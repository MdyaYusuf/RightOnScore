import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import type { MatchPredictionPreviewDto } from "../matches/matchTypes";
import * as matchPredictionApi from "./matchPredictionApi";

type MyPredictionsStatus = "idle" | "loading" | "ready" | "error";

type MyPredictionsState = {
  status: MyPredictionsStatus;
  seasonId: string | null;
  predictions: MatchPredictionPreviewDto[];
};

const initialState: MyPredictionsState = {
  status: "idle",
  seasonId: null,
  predictions: [],
};

export const loadMyPredictionsForSeason = createAsyncThunk(
  "myPredictions/loadMyPredictionsForSeason",
  async (competitionSeasonId: string, { rejectWithValue }) => {
    const result = await matchPredictionApi.getMyPredictionsBySeason(competitionSeasonId);

    if (!result.success || !result.data) {
      return rejectWithValue(result.message ?? "Tahminler yüklenemedi.");
    }

    return {
      seasonId: competitionSeasonId,
      predictions: result.data,
    };
  },
);

const myPredictionsSlice = createSlice({
  name: "myPredictions",
  initialState,
  reducers: {
    clearMyPredictions(state) {
      state.status = "idle";
      state.seasonId = null;
      state.predictions = [];
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadMyPredictionsForSeason.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadMyPredictionsForSeason.fulfilled, (state, action) => {
        state.status = "ready";
        state.seasonId = action.payload.seasonId;
        state.predictions = action.payload.predictions;
      })
      .addCase(loadMyPredictionsForSeason.rejected, (state) => {
        state.status = "error";
        state.predictions = [];
      });
  },
});

export const { clearMyPredictions } = myPredictionsSlice.actions;
export default myPredictionsSlice.reducer;
