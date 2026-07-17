import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CompetitionSeasonPreviewDto,
  CompetitionSeasonResponseDto,
  CreateCompetitionSeasonRequest,
  CreatedCompetitionSeasonResponseDto,
} from "./competitionSeasonTypes";

export function getActiveSeasons(): Promise<ReturnModel<CompetitionSeasonPreviewDto[]>> {
  return apiClient<CompetitionSeasonPreviewDto[]>("/api/competitionseasons/active", {
    silent: true,
  });
}

export function getSeasonsByCompetitionId(
  competitionId: string,
): Promise<ReturnModel<CompetitionSeasonPreviewDto[]>> {
  return apiClient<CompetitionSeasonPreviewDto[]>(
    `/api/competitionseasons/competition/${competitionId}`,
    { silent: true },
  );
}

export function getSeasonById(
  competitionSeasonId: string,
): Promise<ReturnModel<CompetitionSeasonResponseDto>> {
  return apiClient<CompetitionSeasonResponseDto>(
    `/api/competitionseasons/${competitionSeasonId}`,
    { silent: true },
  );
}

export function createSeason(
  request: CreateCompetitionSeasonRequest,
): Promise<ReturnModel<CreatedCompetitionSeasonResponseDto>> {
  return apiClient<CreatedCompetitionSeasonResponseDto>("/api/competitionseasons", {
    method: "POST",
    body: request,
    successToast: true,
  });
}
