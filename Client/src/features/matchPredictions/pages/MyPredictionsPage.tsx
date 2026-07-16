import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  loadActiveSeasons,
  loadSelectedSeasonDetails,
} from "../../competitionSeasons/seasonHubSlice";
import { PredictionListCard } from "../components/PredictionListCard";
import {
  filterPredictions,
  groupPredictionsByDate,
  sortPredictionsByKickoffDesc,
  type PredictionFilter,
} from "../myPredictionHelpers";
import { loadMyPredictionsForSeason } from "../myPredictionsSlice";

const FILTERS: Array<{ id: PredictionFilter; label: string }> = [
  { id: "all", label: "Tümü" },
  { id: "pending", label: "Bekleyen" },
  { id: "scored", label: "Puanlanan" },
];

export function MyPredictionsPage() {
  const dispatch = useAppDispatch();
  const [filter, setFilter] = useState<PredictionFilter>("all");
  const {
    selectedSeasonId,
    selectedSeason,
    myStanding,
    status: seasonHubStatus,
  } = useAppSelector((state) => state.seasonHub);
  const { status, predictions } = useAppSelector((state) => state.myPredictions);

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

    void dispatch(loadMyPredictionsForSeason(selectedSeasonId));
  }, [dispatch, selectedSeasonId]);

  const seasonName =
    selectedSeason?.name ?? (selectedSeasonId ? "Sezon yükleniyor..." : "Sezon seçilmedi");
  const totalPoints = myStanding?.totalPoints ?? 0;

  const groupedPredictions = useMemo(() => {
    const filtered = filterPredictions(predictions, filter);
    const sorted = sortPredictionsByKickoffDesc(filtered);
    return groupPredictionsByDate(sorted);
  }, [predictions, filter]);

  if (!selectedSeasonId && seasonHubStatus === "ready") {
    return (
      <div className="mx-auto w-full max-w-[800px] animate-[authFadeIn_480ms_ease-out]">
        <h1 className="font-display text-[32px] font-bold text-on-surface">Tahminlerim</h1>
        <p className="mt-2 text-on-surface-variant">
          Tahminlerini görmek için önce bir sezon seçmelisin.
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
    <div className="mx-auto w-full max-w-[800px] animate-[authFadeIn_480ms_ease-out]">
      <div className="mb-8 flex flex-col gap-4 md:mb-10 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 className="mb-1 font-display text-[32px] font-bold leading-[1.1] tracking-[-0.02em] text-on-surface md:text-[48px]">
            Tahminlerim
          </h1>
          <div className="flex items-center gap-2 font-body text-[16px] font-semibold text-on-surface-variant md:text-[18px]">
            <span className="text-secondary">{seasonName}</span>
            <span>•</span>
            <span>
              Toplam: <span className="font-bold text-on-surface">{totalPoints} puan</span>
            </span>
          </div>
        </div>

        <div className="inline-flex gap-2 rounded-[0.5rem] border border-outline-variant/20 bg-surface-container-low p-1">
          {FILTERS.map((item) => {
            const isActive = filter === item.id;

            return (
              <button
                key={item.id}
                type="button"
                onClick={() => setFilter(item.id)}
                className={[
                  "rounded-md px-4 py-2 font-label text-[12px] font-medium tracking-[0.05em] uppercase transition-colors",
                  isActive
                    ? "bg-surface-variant text-on-surface"
                    : "text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface",
                ].join(" ")}
              >
                {item.label}
              </button>
            );
          })}
        </div>
      </div>

      {status === "loading" && (
        <p className="text-on-surface-variant">Tahminler yükleniyor...</p>
      )}

      {status === "error" && (
        <p className="text-error">Tahminler yüklenemedi. Daha sonra tekrar dene.</p>
      )}

      {status === "ready" && groupedPredictions.length === 0 && (
        <section className="card-level-1 rounded-[0.5rem] p-6">
          <p className="text-on-surface-variant">
            {filter === "all"
              ? "Bu sezon için henüz tahminin yok. Maçlar sayfasından tahmin yapabilirsin."
              : "Bu filtreye uyan tahmin bulunamadı."}
          </p>
          {filter === "all" ? (
            <Link
              to="/fixtures"
              className="mt-4 inline-block rounded-[0.5rem] bg-secondary px-4 py-3 font-semibold text-black transition-colors hover:bg-secondary-fixed"
            >
              Maçlara git
            </Link>
          ) : null}
        </section>
      )}

      {status === "ready" && groupedPredictions.length > 0 && (
        <div className="space-y-4">
          {groupedPredictions.map((group, index) => (
            <section key={group.key}>
              <div
                className={[
                  "mb-2 border-b border-outline-variant/20 pb-2 font-label text-[12px] font-medium tracking-wider text-on-surface-variant uppercase",
                  index === 0 ? "mt-0" : "mt-6",
                ].join(" ")}
              >
                {group.label}
              </div>
              <div className="space-y-4">
                {group.items.map((prediction) => (
                  <PredictionListCard key={prediction.id} prediction={prediction} />
                ))}
              </div>
            </section>
          ))}
        </div>
      )}
    </div>
  );
}
