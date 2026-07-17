import type { MatchResponseDto, MatchStatus } from "../matches/matchTypes";
import { MATCH_STATUS, MATCH_STATUS_LABEL } from "../matches/matchTypes";
import type { MatchPredictionResponseDto } from "./matchPredictionTypes";

export type PredictionSortKey = "username" | "score" | "advancer" | "points";
export type SortDirection = "asc" | "desc";

export const ADMIN_PREDICTIONS_PAGE_SIZE = 20;

export function matchStatusBadgeLabel(status: MatchStatus): string {
  if (status === MATCH_STATUS.Finished) {
    return "Tamamlandı";
  }

  return MATCH_STATUS_LABEL[status] ?? "Bilinmiyor";
}

export function formatMatchDateTime(kickoffTime: string): string {
  const date = new Date(kickoffTime);

  if (Number.isNaN(date.getTime())) {
    return "—";
  }

  return date.toLocaleString("tr-TR", {
    day: "numeric",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

export function formatPredictedScore(home: number, away: number): string {
  return `${home} - ${away}`;
}

export function formatPointsEarned(points: number | null): string {
  if (points == null) {
    return "—";
  }

  if (points > 0) {
    return `+${points}`;
  }

  return String(points);
}

export function usernameInitials(username: string): string {
  const cleaned = username.trim();

  if (!cleaned) {
    return "?";
  }

  const parts = cleaned.split(/[_\s.-]+/).filter(Boolean);

  if (parts.length >= 2) {
    return `${parts[0]![0] ?? ""}${parts[1]![0] ?? ""}`.toUpperCase();
  }

  return cleaned.slice(0, 2).toUpperCase();
}

export function resolveAdvancerName(
  match: MatchResponseDto,
  predictedAdvancingTeamId: string | null,
): string {
  if (!predictedAdvancingTeamId) {
    return "—";
  }

  if (predictedAdvancingTeamId === match.homeTeamId) {
    return match.homeTeam.name;
  }

  if (predictedAdvancingTeamId === match.awayTeamId) {
    return match.awayTeam.name;
  }

  return "—";
}

export function isExactScorePrediction(
  match: MatchResponseDto,
  prediction: MatchPredictionResponseDto,
): boolean {
  if (match.status !== MATCH_STATUS.Finished) {
    return false;
  }

  if (match.homeScore == null || match.awayScore == null) {
    return false;
  }

  return (
    prediction.predictedHomeScore === match.homeScore &&
    prediction.predictedAwayScore === match.awayScore
  );
}

export function sortPredictions(
  items: MatchPredictionResponseDto[],
  match: MatchResponseDto,
  sortKey: PredictionSortKey,
  direction: SortDirection,
): MatchPredictionResponseDto[] {
  const sorted = [...items];
  const factor = direction === "asc" ? 1 : -1;

  sorted.sort((a, b) => {
    let comparison = 0;

    if (sortKey === "username") {
      comparison = a.user.username.localeCompare(b.user.username, "tr");
    } else if (sortKey === "score") {
      comparison =
        a.predictedHomeScore - b.predictedHomeScore ||
        a.predictedAwayScore - b.predictedAwayScore;
    } else if (sortKey === "advancer") {
      comparison = resolveAdvancerName(match, a.predictedAdvancingTeamId).localeCompare(
        resolveAdvancerName(match, b.predictedAdvancingTeamId),
        "tr",
      );
    } else {
      comparison = (a.pointsEarned ?? -1) - (b.pointsEarned ?? -1);
    }

    return comparison * factor;
  });

  return sorted;
}

export function buildPredictionsCsv(
  match: MatchResponseDto,
  predictions: MatchPredictionResponseDto[],
): string {
  const header = [
    "Username",
    "PredictedHomeScore",
    "PredictedAwayScore",
    "PredictedAdvancer",
    "PointsEarned",
  ];

  const rows = predictions.map((prediction) => [
    csvEscape(prediction.user.username),
    String(prediction.predictedHomeScore),
    String(prediction.predictedAwayScore),
    csvEscape(resolveAdvancerName(match, prediction.predictedAdvancingTeamId)),
    prediction.pointsEarned == null ? "" : String(prediction.pointsEarned),
  ]);

  return [header.join(","), ...rows.map((row) => row.join(","))].join("\r\n");
}

function csvEscape(value: string): string {
  if (/[",\n\r]/.test(value)) {
    return `"${value.replaceAll('"', '""')}"`;
  }

  return value;
}

export function downloadCsv(filename: string, content: string): void {
  const blob = new Blob(["\uFEFF" + content], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = filename;
  anchor.click();
  URL.revokeObjectURL(url);
}
