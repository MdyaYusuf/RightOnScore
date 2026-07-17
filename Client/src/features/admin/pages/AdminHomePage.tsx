import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  formatAwaitingKickoffLabel,
  formatUpcomingTime,
  awaitingStatusLabel,
  matchContextLabel,
} from "../adminHomeHelpers";
import { loadAdminHome } from "../adminHomeSlice";

export function AdminHomePage() {
  const dispatch = useAppDispatch();
  const { status, awaitingResults, upcomingMatches } = useAppSelector(
    (state) => state.adminHome,
  );

  useEffect(() => {
    void dispatch(loadAdminHome());
  }, [dispatch]);

  const pendingCount = awaitingResults.length;
  const isLoading = status === "idle" || status === "loading";

  return (
    <div className="mx-auto flex w-full max-w-[1440px] flex-col gap-8">
      <div className="flex flex-col gap-1">
        <h2 className="font-display text-[36px] leading-tight font-bold tracking-tight text-on-surface md:text-[48px] md:leading-[56px]">
          Operasyon Merkezi
        </h2>
        <p className="font-body text-base text-on-surface-variant">
          Eylem gerektiren operasyonlar ve anlık maç takibi.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 xl:grid-cols-12">
        <section className="flex flex-col gap-4 xl:col-span-7">
          <div className="flex items-center justify-between border-b border-outline-variant/20 pb-2">
            <div className="flex items-center gap-2">
              <span
                className="material-symbols-outlined text-secondary"
                style={{ fontVariationSettings: "'FILL' 1" }}
              >
                warning
              </span>
              <h3 className="font-display text-2xl leading-8 font-semibold tracking-[0.02em] text-on-surface">
                Sonuç Bekleyen Maçlar
              </h3>
            </div>
            {!isLoading && (
              <span className="rounded bg-secondary/10 px-2 py-1 font-label text-[12px] font-medium text-secondary">
                {pendingCount} İşlem Bekliyor
              </span>
            )}
          </div>

          <div className="flex flex-col gap-2">
            {isLoading && (
              <p className="py-8 text-center text-on-surface-variant">Yükleniyor…</p>
            )}

            {!isLoading && awaitingResults.length === 0 && (
              <p className="rounded border border-outline-variant/20 bg-[#101e1b] px-4 py-8 text-center text-on-surface-variant">
                Sonuç bekleyen maç yok.
              </p>
            )}

            {awaitingResults.map((match) => (
              <div
                key={match.id}
                className="admin-surface-texture flex flex-col gap-4 rounded border border-outline-variant/20 bg-[#101e1b] p-4 transition-colors hover:border-outline-variant/50 sm:flex-row sm:items-center sm:justify-between"
              >
                <div className="flex items-center gap-6">
                  <div className="flex min-w-[80px] flex-col gap-0.5">
                    <span className="font-label text-[12px] font-medium text-secondary">
                      {awaitingStatusLabel(match)}
                    </span>
                    <span className="text-sm text-on-surface-variant">
                      {formatAwaitingKickoffLabel(match.kickoffTime)}
                    </span>
                  </div>
                  <div className="hidden h-8 w-px bg-outline-variant/30 sm:block" />
                  <div className="flex flex-col">
                    <div className="flex flex-wrap items-center gap-3">
                      <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface">
                        {match.homeTeam.name}
                      </span>
                      <span className="font-display text-2xl text-outline-variant">-</span>
                      <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface">
                        {match.awayTeam.name}
                      </span>
                    </div>
                    <span className="font-label text-[12px] font-medium text-outline">
                      {matchContextLabel(match)}
                    </span>
                  </div>
                </div>
                <Link
                  to={`/admin/seasons/${match.competitionSeasonId}/fixtures`}
                  className="inline-flex items-center justify-center gap-2 rounded bg-secondary px-5 py-2.5 font-label text-[14px] font-semibold tracking-[0.05em] text-on-secondary shadow-[0_0_15px_rgba(233,195,73,0.1)] transition-colors hover:bg-secondary/90"
                >
                  <span className="material-symbols-outlined text-[18px]">edit_document</span>
                  Sonuç Gir
                </Link>
              </div>
            ))}
          </div>
        </section>

        <section className="flex flex-col gap-4 xl:col-span-5">
          <div className="flex items-center gap-2 border-b border-outline-variant/20 pb-2">
            <span className="material-symbols-outlined text-outline-variant">schedule</span>
            <h3 className="font-display text-2xl leading-8 font-semibold tracking-[0.02em] text-on-surface">
              Yaklaşan Maçlar
            </h3>
          </div>

          <div className="overflow-hidden rounded border border-outline-variant/10 bg-[#04110e]">
            {isLoading && (
              <p className="px-4 py-8 text-center text-on-surface-variant">Yükleniyor…</p>
            )}

            {!isLoading && upcomingMatches.length === 0 && (
              <p className="px-4 py-8 text-center text-on-surface-variant">
                Yaklaşan maç yok.
              </p>
            )}

            {upcomingMatches.map((match, index) => {
              const isFirst = index === 0;
              const timeClass = isFirst
                ? "border-primary-container bg-primary-container/30 text-primary"
                : "border-outline-variant/20 bg-surface-container-high text-on-surface";

              return (
                <div
                  key={match.id}
                  className={[
                    "flex items-center justify-between gap-3 border-outline-variant/10 p-4",
                    index < upcomingMatches.length - 1 ? "border-b" : "",
                    isFirst ? "bg-[#101e1b]/50" : "bg-[#04110e]",
                  ].join(" ")}
                >
                  <div className="flex min-w-0 items-center gap-4">
                    <div
                      className={[
                        "flex min-w-[64px] items-center justify-center rounded border px-3 py-1",
                        timeClass,
                      ].join(" ")}
                    >
                      <span className="font-display text-xl font-semibold tracking-widest md:text-2xl">
                        {formatUpcomingTime(match.kickoffTime)}
                      </span>
                    </div>
                    <div className="flex min-w-0 flex-col">
                      <div className="flex flex-wrap items-center gap-2">
                        <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface">
                          {match.homeTeam.name}
                        </span>
                        <span className="text-xs text-outline-variant">vs</span>
                        <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface">
                          {match.awayTeam.name}
                        </span>
                      </div>
                      <span className="font-label text-[12px] font-medium text-on-surface-variant">
                        {matchContextLabel(match)}
                      </span>
                    </div>
                  </div>
                  <span className="shrink-0 rounded border border-outline/20 px-2 font-label text-[12px] font-medium text-outline">
                    Beklemede
                  </span>
                </div>
              );
            })}
          </div>

          <Link
            to="/admin/fixtures"
            className="mt-2 inline-flex items-center gap-1 self-start font-label text-[14px] font-semibold tracking-[0.05em] text-secondary transition-opacity hover:opacity-80"
          >
            Tüm Fikstürü Gör
            <span className="material-symbols-outlined text-[16px]">arrow_forward</span>
          </Link>
        </section>
      </div>
    </div>
  );
}
