import { useEffect, useMemo, useState } from "react";
import {
  formatKickoffBadge,
  isKnockoutStage,
  isMatchPredictable,
  MATCH_STATUS,
  type MatchPredictionPreviewDto,
  type MatchPreviewDto,
} from "../matchTypes";

type MatchFixtureCardProps = {
  match: MatchPreviewDto;
  prediction: MatchPredictionPreviewDto | null;
  isSaving: boolean;
  onSave: (input: {
    predictedHomeScore: number;
    predictedAwayScore: number;
    predictedAdvancingTeamId: string | null;
  }) => Promise<void>;
};

function scoreToInputValue(score: number | null | undefined): string {
  if (score === null || score === undefined) {
    return "";
  }

  return String(score);
}

export function MatchFixtureCard({
  match,
  prediction,
  isSaving,
  onSave,
}: MatchFixtureCardProps) {
  const locked = !isMatchPredictable(match);
  const isPostponed = match.status === MATCH_STATUS.Postponed;
  const knockout = isKnockoutStage(match.competitionStage);

  const [homeScore, setHomeScore] = useState(scoreToInputValue(prediction?.predictedHomeScore));
  const [awayScore, setAwayScore] = useState(scoreToInputValue(prediction?.predictedAwayScore));
  const [advancingTeamId, setAdvancingTeamId] = useState<string | null>(
    prediction?.predictedAdvancingTeamId ?? null,
  );

  useEffect(() => {
    setHomeScore(scoreToInputValue(prediction?.predictedHomeScore));
    setAwayScore(scoreToInputValue(prediction?.predictedAwayScore));
    setAdvancingTeamId(prediction?.predictedAdvancingTeamId ?? null);
  }, [prediction]);

  const homeScoreNumber = homeScore === "" ? null : Number(homeScore);
  const awayScoreNumber = awayScore === "" ? null : Number(awayScore);
  const isDrawPredicted =
    homeScoreNumber !== null &&
    awayScoreNumber !== null &&
    !Number.isNaN(homeScoreNumber) &&
    !Number.isNaN(awayScoreNumber) &&
    homeScoreNumber === awayScoreNumber;

  const showAdvancePicker = knockout && isDrawPredicted && !locked;

  useEffect(() => {
    if (!knockout || !isDrawPredicted) {
      setAdvancingTeamId(null);
    }
  }, [knockout, isDrawPredicted]);

  const hasExistingPrediction = Boolean(prediction);
  const canSubmit = useMemo(() => {
    if (locked || isSaving) {
      return false;
    }

    if (homeScore === "" || awayScore === "") {
      return false;
    }

    const home = Number(homeScore);
    const away = Number(awayScore);

    if (Number.isNaN(home) || Number.isNaN(away) || home < 0 || away < 0) {
      return false;
    }

    if (knockout && home === away && !advancingTeamId) {
      return false;
    }

    return true;
  }, [locked, isSaving, homeScore, awayScore, knockout, advancingTeamId]);

  async function handleSubmit() {
    if (!canSubmit) {
      return;
    }

    await onSave({
      predictedHomeScore: Number(homeScore),
      predictedAwayScore: Number(awayScore),
      predictedAdvancingTeamId:
        knockout && Number(homeScore) === Number(awayScore) ? advancingTeamId : null,
    });
  }

  const badgeLabel = isPostponed
    ? "ERTELENDİ"
    : knockout
      ? match.competitionStage?.name.toUpperCase() ?? "ELEME"
      : formatKickoffBadge(match.kickoffTime);

  const badgeClass = isPostponed
    ? "bg-[#00201a] text-[#accec3]"
    : knockout
      ? "flex items-center gap-1 border-b border-l border-[#204f42] bg-[#002c22] text-[#689687]"
      : "bg-surface-variant text-on-surface-variant";

  return (
    <div
      className={[
        "match-card relative overflow-hidden rounded-[0.75rem] p-4 shadow-lg md:p-6",
        locked ? "opacity-60" : "group",
      ].join(" ")}
    >
      <div
        className={[
          "absolute top-0 right-0 rounded-bl-lg px-3 py-1 font-label text-[12px] font-medium tracking-[0.05em] uppercase",
          badgeClass,
        ].join(" ")}
      >
        {knockout && !isPostponed ? (
          <span className="material-symbols-outlined text-[14px]">emoji_events</span>
        ) : null}
        {badgeLabel}
      </div>

      <div className="mt-4 flex flex-col items-center">
        <div
          className={[
            "mb-4 flex w-full items-center justify-between",
            locked && !isPostponed ? "mb-2" : "md:mb-6",
          ].join(" ")}
        >
          <TeamBlock team={match.homeTeam} grayscale={locked} />

          {locked ? (
            <div className="px-4 font-display text-[36px] font-bold leading-none text-outline-variant">
              {prediction
                ? `${prediction.predictedHomeScore} : ${prediction.predictedAwayScore}`
                : "- : -"}
            </div>
          ) : (
            <div className="flex items-center gap-4 px-4">
              <input
                type="number"
                min={0}
                max={99}
                inputMode="numeric"
                value={homeScore}
                placeholder="-"
                onChange={(event) => setHomeScore(event.target.value)}
                className="score-input h-20 w-16 rounded-[0.5rem] font-display text-[36px] font-bold text-on-surface transition-colors focus:text-secondary md:h-24 md:w-20"
              />
              <span className="font-display text-[36px] font-bold leading-none text-outline-variant">
                :
              </span>
              <input
                type="number"
                min={0}
                max={99}
                inputMode="numeric"
                value={awayScore}
                placeholder="-"
                onChange={(event) => setAwayScore(event.target.value)}
                className="score-input h-20 w-16 rounded-[0.5rem] font-display text-[36px] font-bold text-on-surface transition-colors focus:text-secondary md:h-24 md:w-20"
              />
            </div>
          )}

          <TeamBlock team={match.awayTeam} grayscale={locked} />
        </div>

        {showAdvancePicker ? (
          <div className="mb-6 flex w-full flex-col items-center rounded-[0.5rem] border border-outline-variant/30 bg-surface-container-low p-3">
            <span className="mb-2 font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase">
              Beraberlik tahmini — kim tur atlar?
            </span>
            <div className="flex w-full gap-2 rounded-md bg-background p-1">
              <button
                type="button"
                onClick={() => setAdvancingTeamId(match.homeTeamId)}
                className={[
                  "flex-1 rounded py-2 font-body text-[16px] transition-colors",
                  advancingTeamId === match.homeTeamId
                    ? "border border-primary/20 bg-primary-container font-semibold text-primary shadow-sm"
                    : "text-on-surface-variant hover:text-on-surface",
                ].join(" ")}
              >
                {match.homeTeam.shortName || match.homeTeam.name}
              </button>
              <button
                type="button"
                onClick={() => setAdvancingTeamId(match.awayTeamId)}
                className={[
                  "flex-1 rounded py-2 font-body text-[16px] transition-colors",
                  advancingTeamId === match.awayTeamId
                    ? "border border-primary/20 bg-primary-container font-semibold text-primary shadow-sm"
                    : "text-on-surface-variant hover:text-on-surface",
                ].join(" ")}
              >
                {match.awayTeam.shortName || match.awayTeam.name}
              </button>
            </div>
          </div>
        ) : null}

        {!locked ? (
          <button
            type="button"
            disabled={!canSubmit}
            onClick={() => void handleSubmit()}
            className={[
              "w-full rounded-[0.5rem] py-3 font-body text-[18px] font-semibold transition-all duration-300",
              hasExistingPrediction
                ? "border-2 border-secondary bg-transparent text-secondary hover:bg-secondary/10"
                : "bg-secondary text-black opacity-100 hover:bg-secondary-fixed md:opacity-0 md:group-hover:opacity-100 md:focus:opacity-100",
              !canSubmit ? "cursor-not-allowed !opacity-50" : "",
            ].join(" ")}
          >
            {isSaving
              ? "Kaydediliyor..."
              : hasExistingPrediction
                ? "Tahmini Güncelle"
                : "Tahmini kaydet"}
          </button>
        ) : null}
      </div>
    </div>
  );
}

type TeamBlockProps = {
  team: MatchPreviewDto["homeTeam"];
  grayscale?: boolean;
};

function TeamBlock({ team, grayscale = false }: TeamBlockProps) {
  return (
    <div
      className={[
        "flex flex-1 flex-col items-center text-center",
        grayscale ? "grayscale" : "",
      ].join(" ")}
    >
      <div className="mb-2 flex h-16 w-16 items-center justify-center rounded-full border border-outline-variant/20 bg-surface-container p-2 shadow-inner md:h-20 md:w-20">
        {team.crestUrl ? (
          <img src={team.crestUrl} alt="" className="h-full w-full object-contain" />
        ) : (
          <span className="material-symbols-outlined text-3xl text-primary">shield</span>
        )}
      </div>
      <span className="font-body text-[16px] font-semibold text-on-surface md:text-[18px]">
        {team.shortName || team.name}
      </span>
    </div>
  );
}
