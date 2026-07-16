import { configureStore } from "@reduxjs/toolkit";
import authReducer from "../../features/auth/authSlice";
import seasonHubReducer from "../../features/competitionSeasons/seasonHubSlice";

export const store = configureStore({
  reducer: {
    auth: authReducer,
    seasonHub: seasonHubReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
