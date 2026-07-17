import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CompetitionResponseDto,
  CreateCompetitionRequest,
  CreatedCompetitionResponseDto,
  CompetitionsPageResult,
  UpdateCompetitionRequest,
} from "./competitionTypes";

export function getAllCompetitions(
  pageNumber = 1,
  pageSize = 50,
): Promise<ReturnModel<CompetitionsPageResult>> {
  const params = new URLSearchParams({
    pageNumber: String(pageNumber),
    pageSize: String(pageSize),
  });

  return apiClient<CompetitionsPageResult>(`/api/competitions?${params.toString()}`, {
    silent: true,
  });
}

export function getCompetitionById(
  id: string,
): Promise<ReturnModel<CompetitionResponseDto>> {
  return apiClient<CompetitionResponseDto>(`/api/competitions/${id}`, {
    silent: true,
  });
}

export function createCompetition(
  request: CreateCompetitionRequest,
): Promise<ReturnModel<CreatedCompetitionResponseDto>> {
  return apiClient<CreatedCompetitionResponseDto>("/api/competitions", {
    method: "POST",
    body: request,
    successToast: true,
  });
}

export function updateCompetition(
  request: UpdateCompetitionRequest,
): Promise<ReturnModel<null>> {
  return apiClient<null>("/api/competitions", {
    method: "PUT",
    body: request,
    successToast: true,
  });
}
