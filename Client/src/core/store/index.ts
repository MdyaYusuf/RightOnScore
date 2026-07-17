import { configureStore } from "@reduxjs/toolkit";
import adminHomeReducer from "../../features/admin/adminHomeSlice";
import authReducer from "../../features/auth/authSlice";
import competitionsReducer from "../../features/competitions/competitionsSlice";
import seasonHubReducer from "../../features/competitionSeasons/seasonHubSlice";
import fixturesReducer from "../../features/matches/fixturesSlice";
import myPredictionsReducer from "../../features/matchPredictions/myPredictionsSlice";
import leaderboardReducer from "../../features/seasonStandings/leaderboardSlice";

export const store = configureStore({
  reducer: {
    auth: authReducer,
    adminHome: adminHomeReducer,
    competitions: competitionsReducer,
    seasonHub: seasonHubReducer,
    fixtures: fixturesReducer,
    leaderboard: leaderboardReducer,
    myPredictions: myPredictionsReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
