import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CompetitionTeamPreviewDto,
  CreateCompetitionTeamRequest,
  CreatedCompetitionTeamResponseDto,
  UpdateCompetitionTeamRequest,
} from "./competitionTeamTypes";

export function getCompetitionTeamsBySeasonId(
  competitionSeasonId: string,
): Promise<ReturnModel<CompetitionTeamPreviewDto[]>> {
  return apiClient<CompetitionTeamPreviewDto[]>(
    `/api/competitionteams/season/${competitionSeasonId}`,
    { silent: true },
  );
}

export function createCompetitionTeam(
  request: CreateCompetitionTeamRequest,
): Promise<ReturnModel<CreatedCompetitionTeamResponseDto>> {
  return apiClient<CreatedCompetitionTeamResponseDto>("/api/competitionteams", {
    method: "POST",
    body: request,
    successToast: true,
  });
}

export function updateCompetitionTeam(
  request: UpdateCompetitionTeamRequest,
): Promise<ReturnModel<null>> {
  return apiClient<null>("/api/competitionteams", {
    method: "PUT",
    body: request,
    successToast: true,
  });
}

export function deleteCompetitionTeam(id: string): Promise<ReturnModel<null>> {
  return apiClient<null>(`/api/competitionteams/${id}`, {
    method: "DELETE",
    successToast: true,
  });
}
