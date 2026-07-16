import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  loadActiveSeasons,
  loadSelectedSeasonDetails,
} from "../../competitionSeasons/seasonHubSlice";
import { MatchFixtureCard } from "../components/MatchFixtureCard";
import { loadFixturesForSeason, saveMatchPrediction } from "../fixturesSlice";

export function FixturesPage() {
  const dispatch = useAppDispatch();
  const {
    selectedSeasonId,
    selectedSeason,
    myStanding,
    status: seasonHubStatus,
  } = useAppSelector((state) => state.seasonHub);
  const { status, matches, predictionsByMatchId, savingMatchIds } = useAppSelector(
    (state) => state.fixtures,
  );

  useEffect(() => {
    if (seasonHubStatus === "idle") {
      void dispatch(loadActiveSeasons());
    }
  }, [dispatch, seasonHubStatus]);

  useEffect(() => {
    if (!selectedSeasonId) {
      return;
    }

    if (!selectedSeason || selectedSeason.id !== selectedSeasonId) {
      void dispatch(loadSelectedSeasonDetails(selectedSeasonId));
    }
  }, [dispatch, selectedSeason, selectedSeasonId]);

  useEffect(() => {
    if (!selectedSeasonId) {
      return;
    }

    void dispatch(loadFixturesForSeason(selectedSeasonId));
  }, [dispatch, selectedSeasonId]);

  const seasonName =
    selectedSeason?.name ??
    (selectedSeasonId
      ? "Sezon yükleniyor..."
      : "Sezon seçilmedi");

  const rankLabel = myStanding ? `#${myStanding.rank}` : "—";
  const pointsLabel = myStanding ? `${myStanding.totalPoints} puan` : "0 puan";

  if (!selectedSeasonId && seasonHubStatus === "ready") {
    return (
      <div className="mx-auto w-full max-w-[600px] animate-[authFadeIn_480ms_ease-out]">
        <h1 className="font-display text-[32px] font-bold text-on-surface">Maçlar</h1>
        <p className="mt-2 text-on-surface-variant">
          Tahmin yapmak için önce bir sezon seçmelisin.
        </p>
        <Link
          to="/seasons"
          className="mt-6 inline-block rounded-[0.5rem] bg-secondary px-4 py-3 font-semibold text-black transition-colors hover:bg-secondary-fixed"
        >
          Sezonlara git
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto w-full max-w-[600px] animate-[authFadeIn_480ms_ease-out]">
      <div className="sticky top-16 z-10 mb-6 border-b border-outline-variant/30 bg-[#05120F]/90 pt-4 pb-4 backdrop-blur-xl md:top-0 md:pt-8">
        <div className="flex items-end justify-between gap-4">
          <div>
            <span className="mb-1 block font-label text-[12px] font-medium tracking-[0.05em] text-secondary uppercase">
              Güncel Sezon
            </span>
            <h2 className="m-0 font-display text-[24px] font-semibold leading-[1.2] text-on-surface md:text-[32px]">
              {seasonName}
            </h2>
          </div>
          <div className="text-right">
            <div className="mb-1 font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase">
              Sıram
            </div>
            <div className="font-body text-[16px] font-semibold text-secondary md:text-[18px]">
              {rankLabel} ({pointsLabel})
            </div>
          </div>
        </div>
      </div>

      {status === "loading" && (
        <p className="text-on-surface-variant">Maçlar yükleniyor...</p>
      )}

      {status === "error" && (
        <p className="text-error">Maçlar yüklenemedi. Daha sonra tekrar dene.</p>
      )}

      {status === "ready" && matches.length === 0 && (
        <section className="card-level-1 rounded-[0.5rem] p-6">
          <p className="text-on-surface-variant">
            Bu sezon için tahmin edilecek yaklaşan maç yok.
          </p>
        </section>
      )}

      {status === "ready" && matches.length > 0 && (
        <div className="space-y-6">
          {matches.map((match) => (
            <MatchFixtureCard
              key={match.id}
              match={match}
              prediction={predictionsByMatchId[match.id] ?? null}
              isSaving={savingMatchIds.includes(match.id)}
              onSave={async (input) => {
                await dispatch(
                  saveMatchPrediction({
                    matchId: match.id,
                    predictionId: predictionsByMatchId[match.id]?.id ?? null,
                    predictedHomeScore: input.predictedHomeScore,
                    predictedAwayScore: input.predictedAwayScore,
                    predictedAdvancingTeamId: input.predictedAdvancingTeamId,
                  }),
                ).unwrap();
              }}
            />
          ))}
        </div>
      )}
    </div>
  );
}
