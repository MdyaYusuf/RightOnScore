import {
  formatActualScore,
  getMatchStatusBadge,
  getPredictionOutcome,
  type PredictionOutcome,
} from "../myPredictionHelpers";
import type { MatchPredictionPreviewDto } from "../../matches/matchTypes";
import { RevealedPredictionsPanel } from "./RevealedPredictionsPanel";

type PredictionListCardProps = {
  prediction: MatchPredictionPreviewDto;
};

export function PredictionListCard({ prediction }: PredictionListCardProps) {
  const outcome = getPredictionOutcome(prediction);
  const statusBadge = getMatchStatusBadge(prediction);
  const actualScore = formatActualScore(prediction);
  const isLive = statusBadge.tone === "live";
  const homeName = prediction.match.homeTeam.shortName || prediction.match.homeTeam.name;
  const awayName = prediction.match.awayTeam.shortName || prediction.match.awayTeam.name;

  return (
    <div
      className={[
        "card-border inner-glow relative flex flex-col overflow-hidden rounded-[0.75rem] p-4 transition-colors",
        cardToneClass(outcome, isLive),
      ].join(" ")}
    >
      {isLive ? <div className="absolute top-0 left-0 h-full w-1 bg-secondary" /> : null}

      <div className="flex flex-col items-center gap-4 sm:flex-row">
        <div className="w-full flex-shrink-0 text-center sm:w-20">
          {statusBadge.tone === "time" ? (
            <div className="font-body text-[16px] font-semibold text-on-surface-variant">
              {statusBadge.label}
            </div>
          ) : (
            <>
              {statusBadge.tone === "live" ? (
                <div className="mb-1 text-xs text-on-surface-variant">
                  {formatKickoffOnly(prediction.match.kickoffTime)}
                </div>
              ) : null}
              <div
                className={[
                  "inline-block rounded px-2 py-0.5 text-[10px] font-bold uppercase",
                  statusBadge.tone === "live"
                    ? "animate-pulse bg-[#93000a] text-white"
                    : "bg-surface-variant text-on-surface-variant",
                ].join(" ")}
              >
                {statusBadge.label}
              </div>
            </>
          )}
        </div>

        <div className="flex w-full flex-1 items-center justify-between px-2 sm:px-4">
          <div className="flex-1 truncate pr-3 text-right font-body text-[16px] font-semibold text-on-surface sm:text-[18px]">
            {homeName}
          </div>

          <PredictedScoreBox prediction={prediction} outcome={outcome} />

          <div className="flex-1 truncate pl-3 text-left font-body text-[16px] font-semibold text-on-surface sm:text-[18px]">
            {awayName}
          </div>
        </div>

        <div className="mt-2 w-full flex-shrink-0 border-t border-outline-variant/10 pt-3 text-center sm:mt-0 sm:w-24 sm:border-0 sm:pt-0 sm:text-right">
          {outcome === "pending" ? (
            <>
              <div className="mb-1 font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase">
                Durum
              </div>
              <div
                className={[
                  "font-body text-[16px] font-semibold sm:text-[18px]",
                  isLive ? "text-secondary" : "text-outline",
                ].join(" ")}
              >
                Bekliyor
              </div>
              {actualScore ? (
                <div className="mt-1 text-[10px] text-on-surface-variant">Gerçek: {actualScore}</div>
              ) : null}
            </>
          ) : (
            <>
              <div className="mb-1 font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase">
                Puan
              </div>
              <div
                className={[
                  "font-display text-[28px] font-bold leading-none md:text-[36px]",
                  pointsClass(outcome),
                ].join(" ")}
              >
                {formatPoints(prediction.pointsEarned ?? 0)}
              </div>
              {actualScore ? (
                <div className="mt-1 text-[10px] text-on-surface-variant">Gerçek: {actualScore}</div>
              ) : null}
            </>
          )}
        </div>
      </div>

      <RevealedPredictionsPanel match={prediction.match} />
    </div>
  );
}

function PredictedScoreBox({
  prediction,
  outcome,
}: {
  prediction: MatchPredictionPreviewDto;
  outcome: PredictionOutcome;
}) {
  const showExactBadge = outcome === "exact";
  const showWrongBadge = outcome === "wrong";
  const scoreClass = scoreToneClass(outcome);

  return (
    <div
      className={[
        "relative flex flex-shrink-0 items-center justify-center rounded-[0.5rem] border px-3 py-1 sm:px-4 sm:py-2",
        scoreBoxBorderClass(outcome),
      ].join(" ")}
    >
      {showExactBadge ? (
        <span className="material-symbols-outlined absolute -top-2 -right-2 rounded-full bg-background text-[16px] text-primary">
          check_circle
        </span>
      ) : null}
      {showWrongBadge ? (
        <span className="material-symbols-outlined absolute -top-2 -right-2 rounded-full bg-background text-[16px] text-error">
          cancel
        </span>
      ) : null}
      <span className={["font-display text-[28px] font-bold leading-none md:text-[32px]", scoreClass].join(" ")}>
        {prediction.predictedHomeScore}
      </span>
      <span className="mx-2 text-outline-variant">-</span>
      <span className={["font-display text-[28px] font-bold leading-none md:text-[32px]", scoreClass].join(" ")}>
        {prediction.predictedAwayScore}
      </span>
    </div>
  );
}

function cardToneClass(outcome: PredictionOutcome, isLive: boolean): string {
  if (isLive) {
    return "bg-primary-container/20 hover:bg-primary-container/30";
  }

  if (outcome === "wrong") {
    return "border border-outline-variant/10 bg-surface-container-lowest opacity-60";
  }

  if (outcome === "exact" || outcome === "partial") {
    return "bg-surface-container-low opacity-80 hover:opacity-100";
  }

  return "bg-surface-container-low hover:bg-surface-container";
}

function scoreBoxBorderClass(outcome: PredictionOutcome): string {
  if (outcome === "exact") {
    return "border-primary/30 bg-surface-container";
  }

  if (outcome === "wrong") {
    return "border-error/30 bg-surface-container";
  }

  if (outcome === "pending") {
    return "border-outline-variant/30 bg-surface-container-low";
  }

  return "border-outline-variant/30 bg-surface-container";
}

function scoreToneClass(outcome: PredictionOutcome): string {
  if (outcome === "exact") {
    return "text-primary";
  }

  if (outcome === "wrong") {
    return "text-error line-through decoration-error/50";
  }

  if (outcome === "pending") {
    return "text-on-surface";
  }

  return "text-on-surface-variant";
}

function pointsClass(outcome: PredictionOutcome): string {
  if (outcome === "exact") {
    return "text-primary";
  }

  if (outcome === "wrong") {
    return "text-on-surface-variant";
  }

  return "text-on-surface";
}

function formatPoints(points: number): string {
  if (points > 0) {
    return `+${points}`;
  }

  return String(points);
}

function formatKickoffOnly(kickoffTime: string): string {
  return new Date(kickoffTime).toLocaleTimeString("tr-TR", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}
