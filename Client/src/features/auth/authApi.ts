import { apiClient } from "../../core/api/client";
import type { ReturnModel, UserResponseDto } from "../../core/types/api";
import type { LoginRequest, RegisterUserRequest } from "./authTypes";

export function login(request: LoginRequest): Promise<ReturnModel<UserResponseDto>> {
  return apiClient<UserResponseDto>("/api/authentication/login", {
    method: "POST",
    body: request,
    successToast: true,
  });
}

export function register(request: RegisterUserRequest): Promise<ReturnModel<{ id: string; username: string }>> {
  return apiClient("/api/authentication/register", {
    method: "POST",
    body: request,
    successToast: true,
  });
}

export function refreshToken(): Promise<ReturnModel<UserResponseDto>> {
  return apiClient<UserResponseDto>("/api/authentication/refresh-token", {
    method: "POST",
    body: {},
    skipAuthRefresh: true,
    silent: true,
  });
}

export function logout(): Promise<ReturnModel<null>> {
  return apiClient<null>("/api/authentication/revoke-refresh-token", {
    method: "POST",
    body: {},
    skipAuthRefresh: true,
    successToast: true,
  });
}

export function getCurrentUser(): Promise<ReturnModel<UserResponseDto>> {
  return apiClient<UserResponseDto>("/api/users/me", {
    silent: true,
  });
}
