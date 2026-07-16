import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type { MatchPreviewDto } from "./matchTypes";

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
