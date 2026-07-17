import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type {
  CompetitionGroupPreviewDto,
  CreateCompetitionGroupRequest,
  CreatedCompetitionGroupResponseDto,
} from "./competitionGroupTypes";

export function getGroupsByStageId(
  competitionStageId: string,
): Promise<ReturnModel<CompetitionGroupPreviewDto[]>> {
  return apiClient<CompetitionGroupPreviewDto[]>(
    `/api/competitiongroups/stage/${competitionStageId}`,
    { silent: true },
  );
}

export function createGroup(
  request: CreateCompetitionGroupRequest,
): Promise<ReturnModel<CreatedCompetitionGroupResponseDto>> {
  return apiClient<CreatedCompetitionGroupResponseDto>("/api/competitiongroups", {
    method: "POST",
    body: request,
    successToast: true,
  });
}
