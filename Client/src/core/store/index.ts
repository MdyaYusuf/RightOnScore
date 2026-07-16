import { configureStore } from "@reduxjs/toolkit";
import authReducer from "../../features/auth/authSlice";
import seasonHubReducer from "../../features/competitionSeasons/seasonHubSlice";
import fixturesReducer from "../../features/matches/fixturesSlice";
import myPredictionsReducer from "../../features/matchPredictions/myPredictionsSlice";
import leaderboardReducer from "../../features/seasonStandings/leaderboardSlice";

export const store = configureStore({
  reducer: {
    auth: authReducer,
    seasonHub: seasonHubReducer,
    fixtures: fixturesReducer,
    leaderboard: leaderboardReducer,
    myPredictions: myPredictionsReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
