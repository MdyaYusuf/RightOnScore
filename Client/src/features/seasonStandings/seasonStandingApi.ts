import { apiClient } from "../../core/api/client";
import type { ReturnModel } from "../../core/types/api";
import type { MySeasonStandingDto } from "../competitionSeasons/competitionSeasonTypes";
import type { SeasonStandingPreviewDto } from "./seasonStandingTypes";

export function getMySeasonStanding(
  competitionSeasonId: string,
): Promise<ReturnModel<MySeasonStandingDto>> {
  return apiClient<MySeasonStandingDto>(
    `/api/seasonstandings/season/${competitionSeasonId}/mine`,
    { silent: true },
  );
}

export function getTopSeasonStandings(
  competitionSeasonId: string,
  topCount = 50,
): Promise<ReturnModel<SeasonStandingPreviewDto[]>> {
  return apiClient<SeasonStandingPreviewDto[]>(
    `/api/seasonstandings/season/${competitionSeasonId}?topCount=${topCount}`,
    { silent: true },
  );
}
