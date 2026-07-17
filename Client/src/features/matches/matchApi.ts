import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type { MatchPreviewDto, MatchResponseDto, PagedResponse } from "./matchTypes";

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
