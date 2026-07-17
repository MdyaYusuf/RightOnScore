import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import * as matchApi from "../matches/matchApi";
import type { MatchResponseDto } from "../matches/matchTypes";
import { isAwaitingResult, isUpcomingScheduled } from "./adminHomeHelpers";

type LoadStatus = "idle" | "loading" | "succeeded" | "failed";

type AdminHomeState = {
  status: LoadStatus;
  awaitingResults: MatchResponseDto[];
  upcomingMatches: MatchResponseDto[];
};

const initialState: AdminHomeState = {
  status: "idle",
  awaitingResults: [],
  upcomingMatches: [],
};

export const loadAdminHome = createAsyncThunk("adminHome/load", async () => {
  const result = await matchApi.getAllMatches(1, 50);

  if (!result.success || !result.data) {
    throw new Error(result.message ?? "Maçlar yüklenemedi.");
  }

  const now = new Date();
  const items = result.data.items;

  const awaitingResults = items
    .filter((match) => isAwaitingResult(match, now))
    .sort(
      (a, b) => new Date(b.kickoffTime).getTime() - new Date(a.kickoffTime).getTime(),
    );

  const upcomingMatches = items
    .filter((match) => isUpcomingScheduled(match, now))
    .sort(
      (a, b) => new Date(a.kickoffTime).getTime() - new Date(b.kickoffTime).getTime(),
    )
    .slice(0, 6);

  return { awaitingResults, upcomingMatches };
});

const adminHomeSlice = createSlice({
  name: "adminHome",
  initialState,
  reducers: {
    clearAdminHome() {
      return initialState;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadAdminHome.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadAdminHome.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.awaitingResults = action.payload.awaitingResults;
        state.upcomingMatches = action.payload.upcomingMatches;
      })
      .addCase(loadAdminHome.rejected, (state) => {
        state.status = "failed";
        state.awaitingResults = [];
        state.upcomingMatches = [];
      });
  },
});

export const { clearAdminHome } = adminHomeSlice.actions;
export default adminHomeSlice.reducer;
