export type MatchStatus = 1 | 2 | 3 | 4 | 5;

export type CompetitionStageType = 1 | 2 | 3 | 4;

export type TeamPreviewDto = {
  id: string;
  name: string;
  shortName: string;
  crestUrl: string | null;
};

export type CompetitionStagePreviewDto = {
  id: string;
  competitionSeasonId: string;
  name: string;
  type: CompetitionStageType;
  displayOrder: number;
  status: number;
  isActive: boolean;
};

export type CompetitionGroupMatchPreviewDto = {
  id: string;
  competitionStageId: string;
  name: string;
  displayOrder: number;
  isActive: boolean;
};

export type MatchPreviewDto = {
  id: string;
  competitionSeasonId: string;
  competitionStageId: string | null;
  competitionStage: CompetitionStagePreviewDto | null;
  competitionGroupId: string | null;
  competitionGroup: CompetitionGroupMatchPreviewDto | null;
  homeTeamId: string;
  homeTeam: TeamPreviewDto;
  awayTeamId: string;
  awayTeam: TeamPreviewDto;
  kickoffTime: string;
  status: MatchStatus;
  round: number | null;
  homeScore: number | null;
  awayScore: number | null;
  advancingTeamId: string | null;
};

export type CreateMatchRequest = {
  competitionSeasonId: string;
  competitionStageId?: string | null;
  competitionGroupId?: string | null;
  homeTeamId: string;
  awayTeamId: string;
  kickoffTime: string;
  round?: number | null;
  venue?: string | null;
  status?: MatchStatus;
};

export type RecordMatchResultRequest = {
  id: string;
  homeScore: number;
  awayScore: number;
  advancingTeamId?: string | null;
};

export type CreatedMatchResponseDto = {
  id: string;
  kickoffTime: string;
};

export const MATCH_STATUS_LABEL: Record<MatchStatus, string> = {
  1: "Planlandı",
  2: "Canlı",
  3: "Bitti",
  4: "Ertelendi",
  5: "İptal",
};

export type CompetitionSeasonMatchPreviewDto = {
  id: string;
  competitionId: string;
  name: string;
  status: number;
  isActive: boolean;
};

export type MatchResponseDto = {
  id: string;
  competitionSeasonId: string;
  competitionSeason: CompetitionSeasonMatchPreviewDto;
  competitionStageId: string | null;
  competitionStage: CompetitionStagePreviewDto | null;
  competitionGroupId: string | null;
  homeTeamId: string;
  homeTeam: TeamPreviewDto;
  awayTeamId: string;
  awayTeam: TeamPreviewDto;
  kickoffTime: string;
  status: MatchStatus;
  round: number | null;
  venue: string | null;
  homeScore: number | null;
  awayScore: number | null;
  advancingTeamId: string | null;
};

export type PagedResponse<T> = {
  items: T[];
  currentPage: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};

export type MatchPredictionPreviewDto = {
  id: string;
  matchId: string;
  match: MatchPreviewDto;
  predictedHomeScore: number;
  predictedAwayScore: number;
  predictedAdvancingTeamId: string | null;
  pointsEarned: number | null;
};

export type CreateMatchPredictionRequest = {
  matchId: string;
  predictedHomeScore: number;
  predictedAwayScore: number;
  predictedAdvancingTeamId?: string | null;
};

export type UpdateMatchPredictionRequest = {
  id: string;
  predictedHomeScore: number;
  predictedAwayScore: number;
  predictedAdvancingTeamId?: string | null;
};

export const MATCH_STATUS = {
  Scheduled: 1,
  Live: 2,
  Finished: 3,
  Postponed: 4,
  Cancelled: 5,
} as const;

export const COMPETITION_STAGE_TYPE = {
  LeagueTable: 1,
  GroupStage: 2,
  Knockout: 3,
  Final: 4,
} as const;

export function isKnockoutStage(stage: CompetitionStagePreviewDto | null | undefined): boolean {
  return (
    stage?.type === COMPETITION_STAGE_TYPE.Knockout ||
    stage?.type === COMPETITION_STAGE_TYPE.Final
  );
}

export function isMatchPredictable(match: MatchPreviewDto, now = new Date()): boolean {
  if (match.status !== MATCH_STATUS.Scheduled) {
    return false;
  }

  return new Date(match.kickoffTime).getTime() > now.getTime();
}

export function formatKickoffBadge(kickoffTime: string): string {
  const date = new Date(kickoffTime);
  const weekday = date
    .toLocaleDateString("tr-TR", { weekday: "short" })
    .replace(".", "")
    .toUpperCase();
  const time = date.toLocaleTimeString("tr-TR", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });

  return `${weekday} ${time}`;
}
