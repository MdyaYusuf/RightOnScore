import { configureStore } from "@reduxjs/toolkit";
import adminHomeReducer from "../../features/admin/adminHomeSlice";
import authReducer from "../../features/auth/authSlice";
import competitionsReducer from "../../features/competitions/competitionsSlice";
import seasonHubReducer from "../../features/competitionSeasons/seasonHubSlice";
import seasonStructureReducer from "../../features/competitionSeasons/seasonStructureSlice";
import seasonTeamsReducer from "../../features/competitionTeams/seasonTeamsSlice";
import adminFixturesReducer from "../../features/matches/adminFixturesSlice";
import fixturesReducer from "../../features/matches/fixturesSlice";
import adminPredictionsReducer from "../../features/matchPredictions/adminPredictionsSlice";
import myPredictionsReducer from "../../features/matchPredictions/myPredictionsSlice";
import leaderboardReducer from "../../features/seasonStandings/leaderboardSlice";
import teamsReducer from "../../features/teams/teamsSlice";

export const store = configureStore({
  reducer: {
    auth: authReducer,
    adminHome: adminHomeReducer,
    competitions: competitionsReducer,
    seasonStructure: seasonStructureReducer,
    seasonTeams: seasonTeamsReducer,
    seasonHub: seasonHubReducer,
    teams: teamsReducer,
    adminFixtures: adminFixturesReducer,
    adminPredictions: adminPredictionsReducer,
    fixtures: fixturesReducer,
    leaderboard: leaderboardReducer,
    myPredictions: myPredictionsReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
