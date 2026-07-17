import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CreatedTeamResponseDto,
  TeamPreviewDto,
  TeamResponseDto,
  TeamsPageResult,
} from "./teamTypes";

export type TeamWritePayload = {
  id?: string;
  name: string;
  shortName: string;
  country: string;
  isActive: boolean;
  crestFile?: File | null;
};

function buildTeamFormData(payload: TeamWritePayload): FormData {
  const formData = new FormData();

  if (payload.id) {
    formData.append("Id", payload.id);
  }

  formData.append("Name", payload.name);
  formData.append("ShortName", payload.shortName);
  formData.append("Country", payload.country);
  formData.append("IsActive", String(payload.isActive));

  if (payload.crestFile) {
    formData.append("CrestFile", payload.crestFile);
  }

  return formData;
}

export function getAllTeams(
  pageNumber = 1,
  pageSize = 10,
): Promise<ReturnModel<TeamsPageResult>> {
  const params = new URLSearchParams({
    pageNumber: String(pageNumber),
    pageSize: String(pageSize),
  });

  return apiClient<TeamsPageResult>(`/api/teams?${params.toString()}`, {
    silent: true,
  });
}

export function getActiveTeams(): Promise<ReturnModel<TeamPreviewDto[]>> {
  return apiClient<TeamPreviewDto[]>("/api/teams/active", { silent: true });
}

export function getTeamById(id: string): Promise<ReturnModel<TeamResponseDto>> {
  return apiClient<TeamResponseDto>(`/api/teams/${id}`, { silent: true });
}

export function searchTeams(searchTerm: string): Promise<ReturnModel<TeamPreviewDto[]>> {
  const params = new URLSearchParams({ searchTerm });

  return apiClient<TeamPreviewDto[]>(`/api/teams/search?${params.toString()}`, {
    silent: true,
  });
}

export function createTeam(
  payload: TeamWritePayload,
): Promise<ReturnModel<CreatedTeamResponseDto>> {
  return apiClient<CreatedTeamResponseDto>("/api/teams", {
    method: "POST",
    body: buildTeamFormData(payload),
    successToast: true,
  });
}

export function updateTeam(payload: TeamWritePayload): Promise<ReturnModel<null>> {
  return apiClient<null>("/api/teams", {
    method: "PUT",
    body: buildTeamFormData(payload),
    successToast: true,
  });
}

export function deleteTeam(id: string): Promise<ReturnModel<null>> {
  return apiClient<null>(`/api/teams/${id}`, {
    method: "DELETE",
    successToast: true,
  });
}
