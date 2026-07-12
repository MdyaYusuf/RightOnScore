import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import type { UserResponseDto } from "../../core/types/api";
import * as authApi from "./authApi";
import type { AuthStatus, LoginRequest, RegisterUserRequest } from "./authTypes";

type AuthState = {
  user: UserResponseDto | null;
  status: AuthStatus;
};

const initialState: AuthState = {
  user: null,
  status: "idle",
};

export const restoreSession = createAsyncThunk(
  "auth/restoreSession",
  async () => {
    const result = await authApi.getCurrentUser();

    if (!result.success || !result.data) {
      return null;
    }

    return result.data;
  },
);

export const login = createAsyncThunk(
  "auth/login",
  async (request: LoginRequest, { rejectWithValue }) => {
    const result = await authApi.login(request);

    if (!result.success || !result.data) {
      return rejectWithValue(result.message ?? "Giriş başarısız.");
    }

    return result.data;
  },
);

export const register = createAsyncThunk(
  "auth/register",
  async (request: RegisterUserRequest, { rejectWithValue }) => {
    const result = await authApi.register(request);

    if (!result.success || !result.data) {
      return rejectWithValue(result.message ?? "Kayıt başarısız.");
    }

    return result.data;
  },
);

export const logout = createAsyncThunk("auth/logout", async () => {
  await authApi.logout();
});

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    clearAuthState(state) {
      state.user = null;
      state.status = "anonymous";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(restoreSession.pending, (state) => {
        state.status = "loading";
      })
      .addCase(restoreSession.fulfilled, (state, action) => {
        state.user = action.payload;
        state.status = action.payload ? "authenticated" : "anonymous";
      })
      .addCase(restoreSession.rejected, (state) => {
        state.user = null;
        state.status = "anonymous";
      })
      .addCase(login.fulfilled, (state, action) => {
        state.user = action.payload;
        state.status = "authenticated";
      })
      .addCase(logout.fulfilled, (state) => {
        state.user = null;
        state.status = "anonymous";
      })
      .addCase(logout.rejected, (state) => {
        state.user = null;
        state.status = "anonymous";
      });
  },
});

export const { clearAuthState } = authSlice.actions;
export default authSlice.reducer;
