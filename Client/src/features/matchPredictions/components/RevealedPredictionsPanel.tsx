import { useEffect, useState } from "react";
import type { MatchPreviewDto } from "../../matches/matchTypes";
import { getRevealedPredictionsByMatchId } from "../matchPredictionApi";
import type { MatchPredictionRevealItemDto } from "../matchPredictionTypes";
import { arePredictionsRevealed } from "../myPredictionHelpers";

type RevealedPredictionsPanelProps = {
  match: MatchPreviewDto;
};

export function RevealedPredictionsPanel({ match }: RevealedPredictionsPanelProps) {
  const [expanded, setExpanded] = useState(false);
  const [status, setStatus] = useState<"idle" | "loading" | "ready" | "error">("idle");
  const [areRevealed, setAreRevealed] = useState(arePredictionsRevealed(match));
  const [predictionCount, setPredictionCount] = useState(0);
  const [predictions, setPredictions] = useState<MatchPredictionRevealItemDto[]>([]);

  useEffect(() => {
    setAreRevealed(arePredictionsRevealed(match));
    setExpanded(false);
    setStatus("idle");
    setPredictions([]);
    setPredictionCount(0);
  }, [match.id, match.kickoffTime, match.status]);

  useEffect(() => {
    if (!expanded) {
      return;
    }

    let cancelled = false;

    async function load() {
      setStatus("loading");
      const result = await getRevealedPredictionsByMatchId(match.id);

      if (cancelled) {
        return;
      }

      if (!result.success || !result.data) {
        setStatus("error");
        return;
      }

      setAreRevealed(result.data.areRevealed);
      setPredictionCount(result.data.predictionCount);
      setPredictions(result.data.predictions);
      setStatus("ready");
    }

    void load();

    return () => {
      cancelled = true;
    };
  }, [expanded, match.id]);

  function resolveAdvancerName(predictedAdvancingTeamId: string | null): string {
    if (!predictedAdvancingTeamId) {
      return "—";
    }

    if (predictedAdvancingTeamId === match.homeTeamId) {
      return match.homeTeam.shortName || match.homeTeam.name;
    }

    if (predictedAdvancingTeamId === match.awayTeamId) {
      return match.awayTeam.shortName || match.awayTeam.name;
    }

    return "—";
  }

  function formatPoints(points: number | null): string {
    if (points == null) {
      return "—";
    }

    if (points > 0) {
      return `+${points}`;
    }

    return String(points);
  }

  return (
    <div className="mt-3 w-full border-t border-outline-variant/15 pt-3">
      <button
        type="button"
        className="flex w-full items-center justify-between gap-2 rounded-[0.5rem] px-2 py-2 text-left transition-colors hover:bg-surface-container-high/60"
        onClick={() => setExpanded((value) => !value)}
        aria-expanded={expanded}
      >
        <span className="flex items-center gap-2 font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase">
          <span className="material-symbols-outlined text-[18px] text-secondary">groups</span>
          Diğer tahminler
        </span>
        <span className="material-symbols-outlined text-on-surface-variant">
          {expanded ? "expand_less" : "expand_more"}
        </span>
      </button>

      {expanded && (
        <div className="mt-2 rounded-[0.5rem] border border-outline-variant/15 bg-surface-container-lowest/60 p-3">
          {status === "loading" && (
            <p className="text-sm text-on-surface-variant">Tahminler yükleniyor...</p>
          )}

          {status === "error" && (
            <p className="text-sm text-error">Tahminler yüklenemedi.</p>
          )}

          {status === "ready" && !areRevealed && (
            <div className="flex items-start gap-2 text-sm text-on-surface-variant">
              <span className="material-symbols-outlined text-[18px] text-secondary">lock</span>
              <p>
                Tahminler maç başlayınca açılır.
                {predictionCount > 0 ? (
                  <>
                    {" "}
                    Şu an <span className="font-semibold text-on-surface">{predictionCount}</span>{" "}
                    kişi tahmin yaptı.
                  </>
                ) : (
                  <> Henüz tahmin yok.</>
                )}
              </p>
            </div>
          )}

          {status === "ready" && areRevealed && predictions.length === 0 && (
            <p className="text-sm text-on-surface-variant">Bu maç için henüz tahmin yok.</p>
          )}

          {status === "ready" && areRevealed && predictions.length > 0 && (
            <ul className="divide-y divide-outline-variant/10">
              {predictions.map((item) => (
                <li
                  key={item.id}
                  className="flex flex-wrap items-center justify-between gap-2 py-2 text-sm"
                >
                  <span className="font-semibold text-on-surface">{item.user.username}</span>
                  <span className="font-mono text-on-surface-variant">
                    {item.predictedHomeScore} - {item.predictedAwayScore}
                  </span>
                  <span className="text-on-surface-variant">
                    {resolveAdvancerName(item.predictedAdvancingTeamId)}
                  </span>
                  <span
                    className={[
                      "font-semibold",
                      item.pointsEarned != null && item.pointsEarned > 0
                        ? "text-secondary"
                        : "text-on-surface-variant/60",
                    ].join(" ")}
                  >
                    {formatPoints(item.pointsEarned)}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}
    </div>
  );
}
