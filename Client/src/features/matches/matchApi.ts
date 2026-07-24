import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CreateMatchRequest,
  CreatedMatchResponseDto,
  MatchPreviewDto,
  MatchResponseDto,
  PagedResponse,
  RecordMatchResultRequest,
} from "./matchTypes";

export function getAllMatches(
  pageNumber = 1,
  pageSize = 50,
): Promise<ReturnModel<PagedResponse<MatchResponseDto>>> {
  const params = new URLSearchParams({
    pageNumber: String(pageNumber),
    pageSize: String(pageSize),
  });

  return apiClient<PagedResponse<MatchResponseDto>>(
    `/api/matches?${params.toString()}`,
    { silent: true },
  );
}

export function getUpcomingMatchesBySeason(
  competitionSeasonId: string,
): Promise<ReturnModel<MatchPreviewDto[]>> {
  return apiClient<MatchPreviewDto[]>(
    `/api/matches/season/${competitionSeasonId}/upcoming`,
    { silent: true },
  );
}

export function getMatchesBySeason(
  competitionSeasonId: string,
): Promise<ReturnModel<MatchPreviewDto[]>> {
  return apiClient<MatchPreviewDto[]>(
    `/api/matches/season/${competitionSeasonId}`,
    { silent: true },
  );
}

export function getMatchById(id: string): Promise<ReturnModel<MatchResponseDto>> {
  return apiClient<MatchResponseDto>(`/api/matches/${id}`, { silent: true });
}

export function createMatch(
  request: CreateMatchRequest,
): Promise<ReturnModel<CreatedMatchResponseDto>> {
  return apiClient<CreatedMatchResponseDto>("/api/matches", {
    method: "POST",
    body: request,
    successToast: true,
  });
}

export function recordMatchResult(
  request: RecordMatchResultRequest,
): Promise<ReturnModel<null>> {
  return apiClient<null>("/api/matches/result", {
    method: "PATCH",
    body: request,
    successToast: true,
  });
}

export function correctMatchResult(
  request: RecordMatchResultRequest,
): Promise<ReturnModel<null>> {
  return apiClient<null>("/api/matches/result/correct", {
    method: "PATCH",
    body: request,
    successToast: true,
  });
}

export function deleteMatch(id: string): Promise<ReturnModel<null>> {
  return apiClient<null>(`/api/matches/${id}`, {
    method: "DELETE",
    successToast: true,
  });
}
