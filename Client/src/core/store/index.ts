import { configureStore } from "@reduxjs/toolkit";
import authReducer from "../../features/auth/authSlice";
import seasonHubReducer from "../../features/competitionSeasons/seasonHubSlice";
import fixturesReducer from "../../features/matches/fixturesSlice";

export const store = configureStore({
  reducer: {
    auth: authReducer,
    seasonHub: seasonHubReducer,
    fixtures: fixturesReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
