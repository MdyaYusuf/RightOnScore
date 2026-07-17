import type { TeamPreviewDto } from "../teams/teamTypes";

export type CompetitionTeamPreviewDto = {
  id: string;
  competitionSeasonId: string;
  teamId: string;
  team: TeamPreviewDto;
  competitionStageId: string | null;
  competitionGroupId: string | null;
  seed: number | null;
  isActive: boolean;
};

export type CreateCompetitionTeamRequest = {
  competitionSeasonId: string;
  teamId: string;
  competitionStageId?: string | null;
  competitionGroupId?: string | null;
  seed?: number | null;
  isActive?: boolean;
};

export type UpdateCompetitionTeamRequest = {
  id: string;
  competitionSeasonId: string;
  teamId: string;
  competitionStageId?: string | null;
  competitionGroupId?: string | null;
  seed?: number | null;
  isActive: boolean;
};

export type CreatedCompetitionTeamResponseDto = {
  id: string;
  teamId: string;
};
