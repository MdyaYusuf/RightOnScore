import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CompetitionStagePreviewDto,
  CreateCompetitionStageRequest,
  CreatedCompetitionStageResponseDto,
} from "./competitionStageTypes";

export function getStagesBySeasonId(
  competitionSeasonId: string,
): Promise<ReturnModel<CompetitionStagePreviewDto[]>> {
  return apiClient<CompetitionStagePreviewDto[]>(
    `/api/competitionstages/season/${competitionSeasonId}`,
    { silent: true },
  );
}

export function createStage(
  request: CreateCompetitionStageRequest,
): Promise<ReturnModel<CreatedCompetitionStageResponseDto>> {
  return apiClient<CreatedCompetitionStageResponseDto>("/api/competitionstages", {
    method: "POST",
    body: request,
    successToast: true,
  });
}
