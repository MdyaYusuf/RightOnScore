import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CreateMatchPredictionRequest,
  MatchPredictionPreviewDto,
  UpdateMatchPredictionRequest,
} from "../matches/matchTypes";

export function getMyPredictionsBySeason(
  competitionSeasonId: string,
): Promise<ReturnModel<MatchPredictionPreviewDto[]>> {
  return apiClient<MatchPredictionPreviewDto[]>(
    `/api/matchpredictions/season/${competitionSeasonId}/mine`,
    { silent: true },
  );
}

export function createPrediction(
  request: CreateMatchPredictionRequest,
): Promise<ReturnModel<{ id: string; matchId: string }>> {
  return apiClient("/api/matchpredictions", {
    method: "POST",
    body: request,
    successToast: true,
  });
}

export function updatePrediction(
  request: UpdateMatchPredictionRequest,
): Promise<ReturnModel<null>> {
  return apiClient("/api/matchpredictions", {
    method: "PUT",
    body: request,
    successToast: true,
  });
}
