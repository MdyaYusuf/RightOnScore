import { MATCH_STATUS, type MatchPredictionPreviewDto } from "../matches/matchTypes";

export type PredictionFilter = "all" | "pending" | "scored";

export type PredictionOutcome = "pending" | "exact" | "partial" | "wrong";

export function isPredictionPending(prediction: MatchPredictionPreviewDto): boolean {
  return prediction.pointsEarned === null || prediction.pointsEarned === undefined;
}

export function isPredictionExact(prediction: MatchPredictionPreviewDto): boolean {
  const { match } = prediction;

  if (match.homeScore === null || match.awayScore === null) {
    return false;
  }

  return (
    prediction.predictedHomeScore === match.homeScore &&
    prediction.predictedAwayScore === match.awayScore
  );
}

export function getPredictionOutcome(prediction: MatchPredictionPreviewDto): PredictionOutcome {
  if (isPredictionPending(prediction)) {
    return "pending";
  }

  if (isPredictionExact(prediction)) {
    return "exact";
  }

  if ((prediction.pointsEarned ?? 0) > 0) {
    return "partial";
  }

  return "wrong";
}

export function filterPredictions(
  predictions: MatchPredictionPreviewDto[],
  filter: PredictionFilter,
): MatchPredictionPreviewDto[] {
  if (filter === "pending") {
    return predictions.filter(isPredictionPending);
  }

  if (filter === "scored") {
    return predictions.filter((prediction) => !isPredictionPending(prediction));
  }

  return predictions;
}

export function sortPredictionsByKickoffDesc(
  predictions: MatchPredictionPreviewDto[],
): MatchPredictionPreviewDto[] {
  return [...predictions].sort(
    (left, right) =>
      new Date(right.match.kickoffTime).getTime() - new Date(left.match.kickoffTime).getTime(),
  );
}

export function groupPredictionsByDate(
  predictions: MatchPredictionPreviewDto[],
): Array<{ key: string; label: string; items: MatchPredictionPreviewDto[] }> {
  const groups = new Map<string, MatchPredictionPreviewDto[]>();

  for (const prediction of predictions) {
    const key = toDateKey(prediction.match.kickoffTime);
    const existing = groups.get(key);

    if (existing) {
      existing.push(prediction);
    } else {
      groups.set(key, [prediction]);
    }
  }

  return [...groups.entries()].map(([key, items]) => ({
    key,
    label: formatDateGroupLabel(key),
    items,
  }));
}

export function formatKickoffTime(kickoffTime: string): string {
  return new Date(kickoffTime).toLocaleTimeString("tr-TR", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

export function formatActualScore(prediction: MatchPredictionPreviewDto): string | null {
  const { homeScore, awayScore } = prediction.match;

  if (homeScore === null || awayScore === null) {
    return null;
  }

  return `${homeScore}-${awayScore}`;
}

export function getMatchStatusBadge(prediction: MatchPredictionPreviewDto): {
  label: string;
  tone: "live" | "finished" | "time" | "other";
} {
  const { status, kickoffTime } = prediction.match;

  if (status === MATCH_STATUS.Live) {
    return { label: "CANLI", tone: "live" };
  }

  if (status === MATCH_STATUS.Finished) {
    return { label: "MS", tone: "finished" };
  }

  if (status === MATCH_STATUS.Postponed) {
    return { label: "ERTELENDİ", tone: "other" };
  }

  if (status === MATCH_STATUS.Cancelled) {
    return { label: "İPTAL", tone: "other" };
  }

  return { label: formatKickoffTime(kickoffTime), tone: "time" };
}

function toDateKey(kickoffTime: string): string {
  const date = new Date(kickoffTime);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function formatDateGroupLabel(dateKey: string): string {
  const [year, month, day] = dateKey.split("-").map(Number);
  const date = new Date(year, month - 1, day);
  const today = startOfLocalDay(new Date());
  const yesterday = new Date(today);
  yesterday.setDate(today.getDate() - 1);

  const target = startOfLocalDay(date);

  if (target.getTime() === today.getTime()) {
    return `Bugün, ${formatShortDate(date)}`;
  }

  if (target.getTime() === yesterday.getTime()) {
    return `Dün, ${formatShortDate(date)}`;
  }

  return date
    .toLocaleDateString("tr-TR", {
      weekday: "long",
      day: "numeric",
      month: "short",
    })
    .toLocaleUpperCase("tr-TR");
}

function formatShortDate(date: Date): string {
  return date.toLocaleDateString("tr-TR", {
    day: "numeric",
    month: "short",
  });
}

function startOfLocalDay(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}
