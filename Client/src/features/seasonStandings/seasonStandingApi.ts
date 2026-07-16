import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type { MySeasonStandingDto } from "../competitionSeasons/competitionSeasonTypes";

export function getMySeasonStanding(
  competitionSeasonId: string,
): Promise<ReturnModel<MySeasonStandingDto>> {
  return apiClient<MySeasonStandingDto>(
    `/api/seasonstandings/season/${competitionSeasonId}/mine`,
    { silent: true },
  );
}
