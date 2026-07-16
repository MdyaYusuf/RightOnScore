import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CompetitionSeasonPreviewDto,
  CompetitionSeasonResponseDto,
} from "./competitionSeasonTypes";

export function getActiveSeasons(): Promise<ReturnModel<CompetitionSeasonPreviewDto[]>> {
  return apiClient<CompetitionSeasonPreviewDto[]>("/api/competitionseasons/active", {
    silent: true,
  });
}

export function getSeasonById(
  competitionSeasonId: string,
): Promise<ReturnModel<CompetitionSeasonResponseDto>> {
  return apiClient<CompetitionSeasonResponseDto>(
    `/api/competitionseasons/${competitionSeasonId}`,
    { silent: true },
  );
}
