import { useEffect, useMemo } from "react";
import { Link, useParams } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import { MATCH_STATUS } from "../../matches/matchTypes";
import {
  buildPredictionsCsv,
  downloadCsv,
  formatMatchDateTime,
  formatPointsEarned,
  formatPredictedScore,
  isExactScorePrediction,
  matchStatusBadgeLabel,
  resolveAdvancerName,
  sortPredictions,
  usernameInitials,
  type PredictionSortKey,
} from "../adminPredictionsHelpers";
import {
  loadAdminPredictionsPage,
  setPageNumber,
  setSort,
} from "../adminPredictionsSlice";

export function AdminPredictionsPage() {
  const { matchId = "" } = useParams();
  const dispatch = useAppDispatch();
  const {
    status,
    match,
    predictions,
    sortKey,
    sortDirection,
    pageNumber,
    pageSize,
  } = useAppSelector((state) => state.adminPredictions);

  useEffect(() => {
    if (!matchId) {
      return;
    }

    void dispatch(loadAdminPredictionsPage(matchId));
  }, [matchId, dispatch]);

  const sortedPredictions = useMemo(() => {
    if (!match) {
      return [];
    }

    return sortPredictions(predictions, match, sortKey, sortDirection);
  }, [match, predictions, sortKey, sortDirection]);

  const totalCount = sortedPredictions.length;
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const safePage = Math.min(pageNumber, totalPages);
  const pageStart = totalCount === 0 ? 0 : (safePage - 1) * pageSize + 1;
  const pageEnd = Math.min(safePage * pageSize, totalCount);
  const pageItems = sortedPredictions.slice((safePage - 1) * pageSize, safePage * pageSize);

  const isLoading = status === "idle" || status === "loading";
  const fixturesBackPath = match
    ? `/admin/seasons/${match.competitionSeasonId}/fixtures`
    : "/admin/fixtures";

  const scoreHome = match?.homeScore;
  const scoreAway = match?.awayScore;
  const hasFinalScore =
    match?.status === MATCH_STATUS.Finished && scoreHome != null && scoreAway != null;

  function handleExportCsv() {
    if (!match) {
      return;
    }

    const csv = buildPredictionsCsv(match, sortedPredictions);
    const slug = `${match.homeTeam.shortName}-vs-${match.awayTeam.shortName}`
      .toLowerCase()
      .replace(/[^a-z0-9-]+/g, "-");
    downloadCsv(`predictions-${slug}.csv`, csv);
  }

  function handleSort(key: PredictionSortKey) {
    dispatch(setSort(key));
  }

  return (
    <div className="relative mx-auto flex w-full max-w-[1440px] flex-col gap-8 overflow-hidden">
      <div
        className="pointer-events-none absolute inset-0 opacity-20"
        style={{
          background:
            "radial-gradient(circle at 100% 0%, rgba(11, 43, 36, 0.9) 0%, transparent 40%)",
        }}
      />

      <div className="relative z-10">
        <nav className="mb-3 flex font-label text-[12px] font-medium tracking-wider text-on-surface-variant/60 uppercase">
          <Link
            to={fixturesBackPath}
            className="transition-colors hover:text-secondary"
          >
            Maçlar
          </Link>
          <span className="mx-2">/</span>
          <span className="text-on-surface-variant">Tahminler</span>
        </nav>
        <div className="flex flex-col justify-between gap-4 md:flex-row md:items-end">
          <div>
            <h2 className="font-display text-[36px] leading-tight font-bold tracking-tight text-on-surface md:text-[48px] md:leading-[56px]">
              Tahmin Görünümü
            </h2>
            <p className="mt-2 font-body text-lg text-on-surface-variant">
              Maç bütünlüğü için kullanıcı tahminlerinin salt okunur özeti.
            </p>
          </div>
        </div>
      </div>

      {isLoading && (
        <p className="relative z-10 py-10 text-center text-on-surface-variant">Yükleniyor…</p>
      )}

      {!isLoading && status === "failed" && (
        <p className="relative z-10 py-10 text-center text-error">
          Tahminler yüklenemedi. Maç bağlantısını kontrol edin.
        </p>
      )}

      {!isLoading && match && (
        <>
          <section className="admin-surface-texture relative z-10 overflow-hidden rounded-xl border border-outline-variant/20 bg-[#101e1b] p-6 md:p-8">
            <div className="absolute -top-20 -left-20 h-64 w-64 rounded-full bg-secondary-container opacity-10 blur-3xl" />
            <div className="absolute -right-20 -bottom-20 h-64 w-64 rounded-full bg-primary-container opacity-20 blur-3xl" />

            <div className="relative z-10 flex flex-col items-center justify-center text-center">
              <div className="mb-6 inline-flex items-center gap-2 rounded-full border border-outline-variant/30 bg-[#2a3834] px-3 py-1">
                <span
                  className={[
                    "h-2 w-2 rounded-full",
                    match.status === MATCH_STATUS.Live
                      ? "animate-pulse bg-primary"
                      : "bg-secondary",
                  ].join(" ")}
                />
                <span className="font-label text-[12px] font-medium tracking-widest text-secondary uppercase">
                  {matchStatusBadgeLabel(match.status)}
                </span>
              </div>

              <div className="flex w-full max-w-3xl items-center justify-center gap-6 md:gap-10">
                <div className="flex-1 text-right">
                  <h3 className="font-display text-2xl font-semibold text-on-surface md:text-3xl">
                    {match.homeTeam.name}
                  </h3>
                  <p className="mt-1 font-label text-[12px] font-medium tracking-wider text-on-surface-variant uppercase">
                    Ev Sahibi
                  </p>
                </div>

                <div className="flex flex-col items-center justify-center border-x border-outline-variant/10 px-6 md:px-10">
                  <div className="font-display text-[48px] leading-none font-bold tracking-tighter text-on-surface md:text-[64px]">
                    {hasFinalScore ? scoreHome : "—"}
                    <span className="mx-2 text-outline-variant">-</span>
                    {hasFinalScore ? scoreAway : "—"}
                  </div>
                  <p className="mt-3 rounded-full border border-outline-variant/20 bg-[#1f2d29] px-3 py-1 font-label text-[11px] font-medium text-on-surface-variant">
                    {hasFinalScore ? "Final Skor" : "Skor Bekleniyor"}
                  </p>
                </div>

                <div className="flex-1 text-left">
                  <h3 className="font-display text-2xl font-semibold text-on-surface md:text-3xl">
                    {match.awayTeam.name}
                  </h3>
                  <p className="mt-1 font-label text-[12px] font-medium tracking-wider text-on-surface-variant uppercase">
                    Deplasman
                  </p>
                </div>
              </div>

              <div className="mt-8 flex w-full max-w-2xl flex-wrap justify-center gap-6 border-t border-outline-variant/10 pt-6">
                <div className="flex items-center gap-2 text-on-surface-variant">
                  <span className="material-symbols-outlined text-xl">calendar_today</span>
                  <span className="font-body text-base">
                    {formatMatchDateTime(match.kickoffTime)}
                  </span>
                </div>
                {match.venue && (
                  <div className="flex items-center gap-2 text-on-surface-variant">
                    <span className="material-symbols-outlined text-xl">stadium</span>
                    <span className="font-body text-base">{match.venue}</span>
                  </div>
                )}
              </div>
            </div>
          </section>

          <section className="relative z-10 flex min-h-[400px] flex-col overflow-hidden rounded-xl border border-outline-variant/20 bg-[#101e1b]">
            <div className="flex items-center justify-between border-b border-outline-variant/10 bg-[#04110e] p-4">
              <h3 className="flex items-center gap-2 font-display text-xl font-semibold text-on-surface">
                <span className="material-symbols-outlined text-secondary">analytics</span>
                Kullanıcı Tahminleri
              </h3>
              <button
                type="button"
                className="inline-flex items-center gap-2 rounded-lg border border-outline-variant/20 bg-[#15221f] px-4 py-2 font-label text-[14px] font-semibold text-on-surface transition-colors hover:border-secondary hover:text-secondary disabled:opacity-40"
                disabled={totalCount === 0}
                onClick={handleExportCsv}
              >
                <span className="material-symbols-outlined text-sm">download</span>
                CSV Dışa Aktar
              </button>
            </div>

            {totalCount === 0 ? (
              <div className="flex flex-1 flex-col items-center justify-center border-t border-dashed border-outline-variant/10 p-10">
                <span className="material-symbols-outlined mb-4 text-[64px] text-outline-variant/30">
                  search_off
                </span>
                <p className="font-display text-xl text-on-surface-variant">
                  Bu maç için tahmin yok.
                </p>
                <p className="mt-2 font-body text-base text-on-surface-variant/60">
                  Henüz hiçbir kullanıcı bu karşılaşma için tahminde bulunmadı.
                </p>
              </div>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full border-collapse text-left">
                    <thead>
                      <tr className="border-b border-outline-variant/20 bg-[#04110e]">
                        <SortableHeader
                          label="Kullanıcı Adı"
                          active={sortKey === "username"}
                          direction={sortDirection}
                          onClick={() => handleSort("username")}
                        />
                        <SortableHeader
                          label="Tahmin Skoru"
                          active={sortKey === "score"}
                          direction={sortDirection}
                          onClick={() => handleSort("score")}
                        />
                        <SortableHeader
                          label="Tur Atlayan"
                          active={sortKey === "advancer"}
                          direction={sortDirection}
                          onClick={() => handleSort("advancer")}
                        />
                        <SortableHeader
                          label="Kazanılan Puan"
                          active={sortKey === "points"}
                          direction={sortDirection}
                          align="right"
                          onClick={() => handleSort("points")}
                        />
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-outline-variant/10 font-body text-base text-on-surface">
                      {pageItems.map((prediction) => {
                        const exact = isExactScorePrediction(match, prediction);
                        const points = prediction.pointsEarned;
                        const pointsLabel = formatPointsEarned(points);

                        return (
                          <tr
                            key={prediction.id}
                            className="transition-colors hover:bg-[#1f2d29]"
                          >
                            <td className="p-4">
                              <div className="flex items-center gap-3">
                                {prediction.user.profileImageUrl ? (
                                  <img
                                    src={prediction.user.profileImageUrl}
                                    alt=""
                                    className="h-8 w-8 rounded-full border border-outline-variant/20 object-cover"
                                  />
                                ) : (
                                  <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary-container text-sm font-bold text-on-primary-container">
                                    {usernameInitials(prediction.user.username)}
                                  </div>
                                )}
                                <span className="font-semibold text-on-surface">
                                  {prediction.user.username}
                                </span>
                              </div>
                            </td>
                            <td className="p-4">
                              <span
                                className={[
                                  "inline-block rounded border px-3 py-1 font-mono",
                                  exact
                                    ? "border-secondary/30 bg-[#2a3834] text-secondary"
                                    : "border-outline-variant/20 bg-[#15221f] text-on-surface-variant",
                                ].join(" ")}
                              >
                                {formatPredictedScore(
                                  prediction.predictedHomeScore,
                                  prediction.predictedAwayScore,
                                )}
                              </span>
                            </td>
                            <td className="p-4 text-on-surface-variant">
                              {resolveAdvancerName(
                                match,
                                prediction.predictedAdvancingTeamId,
                              )}
                            </td>
                            <td className="p-4 text-right">
                              <span
                                className={[
                                  "font-bold",
                                  points != null && points > 0
                                    ? points >= 10
                                      ? "text-secondary"
                                      : "text-primary"
                                    : "text-on-surface-variant/50",
                                ].join(" ")}
                              >
                                {pointsLabel}
                              </span>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>

                <div className="mt-auto flex items-center justify-between border-t border-outline-variant/10 bg-[#04110e] p-4 font-label text-[12px] text-on-surface-variant">
                  <span>
                    {pageStart}–{pageEnd} / {totalCount.toLocaleString("tr-TR")} kayıt
                  </span>
                  <div className="flex gap-1">
                    <button
                      type="button"
                      className="rounded p-1 transition-colors hover:bg-[#15221f] hover:text-on-surface disabled:opacity-30"
                      disabled={safePage <= 1}
                      onClick={() => dispatch(setPageNumber(safePage - 1))}
                      aria-label="Önceki sayfa"
                    >
                      <span className="material-symbols-outlined">chevron_left</span>
                    </button>
                    <button
                      type="button"
                      className="rounded p-1 transition-colors hover:bg-[#15221f] hover:text-on-surface disabled:opacity-30"
                      disabled={safePage >= totalPages}
                      onClick={() => dispatch(setPageNumber(safePage + 1))}
                      aria-label="Sonraki sayfa"
                    >
                      <span className="material-symbols-outlined">chevron_right</span>
                    </button>
                  </div>
                </div>
              </>
            )}
          </section>
        </>
      )}
    </div>
  );
}

function SortableHeader({
  label,
  active,
  direction,
  align = "left",
  onClick,
}: {
  label: string;
  active: boolean;
  direction: "asc" | "desc";
  align?: "left" | "right";
  onClick: () => void;
}) {
  return (
    <th
      className={[
        "cursor-pointer p-4 font-display text-[14px] tracking-widest text-tertiary uppercase opacity-80 transition-colors hover:text-secondary",
        align === "right" ? "text-right" : "text-left",
      ].join(" ")}
    >
      <button
        type="button"
        className={[
          "group flex items-center gap-1",
          align === "right" ? "ml-auto justify-end" : "",
        ].join(" ")}
        onClick={onClick}
      >
        {label}
        <span
          className={[
            "material-symbols-outlined text-[16px] transition-opacity",
            active ? "opacity-100 text-secondary" : "opacity-0 group-hover:opacity-100",
          ].join(" ")}
        >
          {active && direction === "asc" ? "arrow_upward" : "arrow_downward"}
        </span>
      </button>
    </th>
  );
}
