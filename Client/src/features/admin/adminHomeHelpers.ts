import { MATCH_STATUS, type MatchResponseDto } from "../matches/matchTypes";

export function isAwaitingResult(match: MatchResponseDto, now = new Date()): boolean {
  if (match.status === MATCH_STATUS.Live) {
    return true;
  }

  if (match.status !== MATCH_STATUS.Scheduled) {
    return false;
  }

  return new Date(match.kickoffTime).getTime() <= now.getTime();
}

export function isUpcomingScheduled(match: MatchResponseDto, now = new Date()): boolean {
  if (match.status !== MATCH_STATUS.Scheduled) {
    return false;
  }

  return new Date(match.kickoffTime).getTime() > now.getTime();
}

export function matchContextLabel(match: MatchResponseDto): string {
  return (
    match.competitionStage?.name ??
    match.competitionSeason.name ??
    "Müsabaka"
  );
}

export function awaitingStatusLabel(match: MatchResponseDto): string {
  if (match.status === MATCH_STATUS.Live) {
    return "Canlı";
  }

  return "Tamamlandı";
}

export function formatAwaitingKickoffLabel(kickoffTime: string, now = new Date()): string {
  const kickoff = new Date(kickoffTime);
  const time = kickoff.toLocaleTimeString("tr-TR", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });

  const startOfToday = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  const startOfKickoffDay = new Date(
    kickoff.getFullYear(),
    kickoff.getMonth(),
    kickoff.getDate(),
  );
  const dayDiff = Math.round(
    (startOfToday.getTime() - startOfKickoffDay.getTime()) / (1000 * 60 * 60 * 24),
  );

  if (dayDiff === 0) {
    return `Bugün, ${time}`;
  }

  if (dayDiff === 1) {
    return `Dün, ${time}`;
  }

  const date = kickoff.toLocaleDateString("tr-TR", {
    day: "numeric",
    month: "short",
  });

  return `${date}, ${time}`;
}

export function formatUpcomingTime(kickoffTime: string): string {
  return new Date(kickoffTime).toLocaleTimeString("tr-TR", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}
