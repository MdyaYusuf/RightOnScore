import {
  MATCH_STATUS,
  MATCH_STATUS_LABEL,
  type MatchPreviewDto,
  type MatchStatus,
} from "./matchTypes";

export function formatFixtureDateLabel(kickoffTime: string, now = new Date()): string {
  const kickoff = new Date(kickoffTime);
  const startOfToday = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  const startOfKickoffDay = new Date(
    kickoff.getFullYear(),
    kickoff.getMonth(),
    kickoff.getDate(),
  );
  const dayDiff = Math.round(
    (startOfKickoffDay.getTime() - startOfToday.getTime()) / (1000 * 60 * 60 * 24),
  );

  if (dayDiff === 0) {
    return "Bugün";
  }

  if (dayDiff === 1) {
    return "Yarın";
  }

  if (dayDiff === -1) {
    return "Dün";
  }

  return kickoff.toLocaleDateString("tr-TR", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}

export function formatFixtureTime(kickoffTime: string): string {
  return new Date(kickoffTime).toLocaleTimeString("tr-TR", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

export function fixtureStagePrimaryLabel(match: MatchPreviewDto): string {
  return match.competitionGroup?.name ?? match.competitionStage?.name ?? "—";
}

export function fixtureStageSecondaryLabel(match: MatchPreviewDto): string | null {
  if (match.competitionGroup?.name && match.competitionStage?.name) {
    if (match.round != null) {
      return `Hafta ${match.round}`;
    }

    return match.competitionStage.name;
  }

  if (match.round != null) {
    return `Hafta ${match.round}`;
  }

  return null;
}

export function fixtureContextLabel(match: MatchPreviewDto): string {
  const primary = fixtureStagePrimaryLabel(match);
  const secondary = fixtureStageSecondaryLabel(match);

  if (secondary && primary !== "—") {
    return `${primary} - ${secondary}`;
  }

  return primary;
}

export function matchStatusLabel(status: MatchStatus): string {
  return MATCH_STATUS_LABEL[status] ?? "Bilinmiyor";
}

export function canRecordResult(status: MatchStatus): boolean {
  return status === MATCH_STATUS.Scheduled || status === MATCH_STATUS.Live;
}

export function canCorrectResult(status: MatchStatus): boolean {
  return status === MATCH_STATUS.Finished;
}

export function canEnterOrCorrectResult(status: MatchStatus): boolean {
  return canRecordResult(status) || canCorrectResult(status);
}
