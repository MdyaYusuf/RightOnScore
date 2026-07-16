import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import type { MySeasonStandingDto } from "../competitionSeasons/competitionSeasonTypes";
import * as seasonStandingApi from "./seasonStandingApi";
import type { SeasonStandingPreviewDto } from "./seasonStandingTypes";

type LeaderboardStatus = "idle" | "loading" | "ready" | "error";

type LeaderboardState = {
  status: LeaderboardStatus;
  seasonId: string | null;
  standings: SeasonStandingPreviewDto[];
  myStanding: MySeasonStandingDto | null;
};

const initialState: LeaderboardState = {
  status: "idle",
  seasonId: null,
  standings: [],
  myStanding: null,
};

export const loadLeaderboardForSeason = createAsyncThunk(
  "leaderboard/loadLeaderboardForSeason",
  async (competitionSeasonId: string, { rejectWithValue }) => {
    const [topResult, mineResult] = await Promise.all([
      seasonStandingApi.getTopSeasonStandings(competitionSeasonId, 50),
      seasonStandingApi.getMySeasonStanding(competitionSeasonId),
    ]);

    if (!topResult.success || !topResult.data) {
      return rejectWithValue(topResult.message ?? "Sıralama yüklenemedi.");
    }

    return {
      seasonId: competitionSeasonId,
      standings: topResult.data,
      myStanding: mineResult.success ? mineResult.data : null,
    };
  },
);

const leaderboardSlice = createSlice({
  name: "leaderboard",
  initialState,
  reducers: {
    clearLeaderboard(state) {
      state.status = "idle";
      state.seasonId = null;
      state.standings = [];
      state.myStanding = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadLeaderboardForSeason.pending, (state) => {
        state.status = "loading";
      })
      .addCase(loadLeaderboardForSeason.fulfilled, (state, action) => {
        state.status = "ready";
        state.seasonId = action.payload.seasonId;
        state.standings = action.payload.standings;
        state.myStanding = action.payload.myStanding;
      })
      .addCase(loadLeaderboardForSeason.rejected, (state) => {
        state.status = "error";
        state.standings = [];
        state.myStanding = null;
      });
  },
});

export const { clearLeaderboard } = leaderboardSlice.actions;
export default leaderboardSlice.reducer;
