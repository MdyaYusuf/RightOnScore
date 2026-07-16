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

export type MatchPreviewDto = {
  id: string;
  competitionSeasonId: string;
  competitionStageId: string | null;
  competitionStage: CompetitionStagePreviewDto | null;
  homeTeamId: string;
  homeTeam: TeamPreviewDto;
  awayTeamId: string;
  awayTeam: TeamPreviewDto;
  kickoffTime: string;
  status: MatchStatus;
  homeScore: number | null;
  awayScore: number | null;
  advancingTeamId: string | null;
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
