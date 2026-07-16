import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  loadActiveSeasons,
  loadSelectedSeasonDetails,
} from "../../competitionSeasons/seasonHubSlice";
import { loadLeaderboardForSeason } from "../leaderboardSlice";
import {
  formatAccuracy,
  formatTopPercent,
  getInitials,
  type SeasonStandingPreviewDto,
} from "../seasonStandingTypes";

export function LeaderboardPage() {
  const dispatch = useAppDispatch();
  const currentUser = useAppSelector((state) => state.auth.user);
  const {
    selectedSeasonId,
    selectedSeason,
    status: seasonHubStatus,
  } = useAppSelector((state) => state.seasonHub);
  const { status, standings, myStanding } = useAppSelector((state) => state.leaderboard);

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

    void dispatch(loadLeaderboardForSeason(selectedSeasonId));
  }, [dispatch, selectedSeasonId]);

  const seasonName =
    selectedSeason?.name ?? (selectedSeasonId ? "Sezon yükleniyor..." : "Sezon seçilmedi");

  const currentUserId = currentUser?.id ?? null;
  const isCurrentUserInTop = standings.some((standing) => standing.userId === currentUserId);
  const showPinnedCurrentUser = Boolean(myStanding && currentUserId && !isCurrentUserInTop);

  const topPercent = myStanding
    ? formatTopPercent(myStanding.rank, myStanding.totalParticipants)
    : null;
  const accuracy = myStanding
    ? formatAccuracy(myStanding.exactScoreCount, myStanding.scoredPredictionCount)
    : null;

  if (!selectedSeasonId && seasonHubStatus === "ready") {
    return (
      <div className="mx-auto w-full max-w-[600px] animate-[authFadeIn_480ms_ease-out]">
        <h1 className="font-display text-[32px] font-bold text-on-surface">Sıralama</h1>
        <p className="mt-2 text-on-surface-variant">
          Sıralamayı görmek için önce bir sezon seçmelisin.
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
    <div className="mx-auto flex w-full max-w-[600px] animate-[authFadeIn_480ms_ease-out] flex-col gap-6">
      <div className="flex items-center justify-between border-b border-[#1E4D40] pb-4">
        <h1 className="font-display text-[24px] font-semibold leading-[1.2] text-on-surface md:text-[32px]">
          {seasonName}
        </h1>
        <div className="rounded bg-surface-container-high px-3 py-1 font-label text-[12px] font-medium tracking-wider text-on-surface-variant uppercase">
          Genel
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <StatCard
          label="Sıram"
          value={myStanding ? `#${myStanding.rank}` : "—"}
          valueClassName="text-secondary"
          subtitle={topPercent ?? "Henüz sıralama yok"}
        />
        <StatCard
          label="Toplam Puan"
          value={myStanding ? String(myStanding.totalPoints) : "0"}
          subtitle={
            myStanding
              ? `${myStanding.scoredPredictionCount} puanlanan tahmin`
              : "Puanlanan tahmin yok"
          }
        />
        <StatCard
          label="Tam İskor"
          value={myStanding ? String(myStanding.exactScoreCount) : "0"}
          subtitle={accuracy ?? "İsabet oranı yok"}
        />
        <StatCard
          label="Katılımcı"
          value={
            myStanding
              ? myStanding.totalParticipants.toLocaleString("tr-TR")
              : standings.length > 0
                ? String(standings.length)
                : "0"
          }
          subtitle="Bu sezonda aktif"
        />
      </div>

      <div className="mt-2">
        <div className="mb-4 flex items-center justify-between px-2">
          <h2 className="font-body text-[18px] font-semibold text-on-surface">En İyi Oyuncular</h2>
        </div>

        {status === "loading" && (
          <p className="px-2 text-on-surface-variant">Sıralama yükleniyor...</p>
        )}

        {status === "error" && (
          <p className="px-2 text-error">Sıralama yüklenemedi. Daha sonra tekrar dene.</p>
        )}

        {status === "ready" && standings.length === 0 && !myStanding && (
          <section className="card-level-1 rounded-[0.5rem] p-6">
            <p className="text-on-surface-variant">
              Bu sezon için henüz sıralama kaydı yok. Maç sonuçları girildikçe burada görünecek.
            </p>
          </section>
        )}

        {status === "ready" && (standings.length > 0 || myStanding) && (
          <div className="flex flex-col gap-2">
            <div className="mb-2 flex items-center border-b border-[#1E4D40] px-4 py-2 font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase">
              <div className="w-12 text-center">Sıra</div>
              <div className="flex-1">Oyuncu</div>
              <div className="w-16 text-center">Tam</div>
              <div className="w-20 text-right">Puan</div>
            </div>

            {standings.map((standing) => (
              <StandingRow
                key={standing.userId}
                standing={standing}
                isCurrentUser={standing.userId === currentUserId}
              />
            ))}

            {showPinnedCurrentUser && myStanding && currentUser ? (
              <>
                <div className="flex justify-center py-2">
                  <span className="material-symbols-outlined text-outline-variant">more_vert</span>
                </div>
                <StandingRow
                  standing={{
                    rank: myStanding.rank,
                    userId: currentUser.id,
                    user: {
                      id: currentUser.id,
                      username: currentUser.username,
                      profileImageUrl: currentUser.profileImageUrl,
                      roleName: currentUser.roleName,
                    },
                    totalPoints: myStanding.totalPoints,
                    exactScoreCount: myStanding.exactScoreCount,
                    scoredPredictionCount: myStanding.scoredPredictionCount,
                  }}
                  isCurrentUser
                  showYouLabel
                />
              </>
            ) : null}
          </div>
        )}
      </div>
    </div>
  );
}

type StatCardProps = {
  label: string;
  value: string;
  subtitle: string;
  valueClassName?: string;
};

function StatCard({ label, value, subtitle, valueClassName = "text-on-surface" }: StatCardProps) {
  return (
    <div className="card-level-1 flex flex-col justify-between rounded-[0.75rem] p-4">
      <div className="mb-2 font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase">
        {label}
      </div>
      <div className={`font-display text-[36px] font-bold leading-none ${valueClassName}`}>
        {value}
      </div>
      <div className="mt-1 font-body text-sm text-on-surface-variant">{subtitle}</div>
    </div>
  );
}

type StandingRowProps = {
  standing: SeasonStandingPreviewDto;
  isCurrentUser: boolean;
  showYouLabel?: boolean;
};

function StandingRow({ standing, isCurrentUser, showYouLabel = false }: StandingRowProps) {
  const displayName =
    isCurrentUser || showYouLabel
      ? `${standing.user.username} (Sen)`
      : standing.user.username;

  return (
    <div
      className={[
        "card-level-1 flex items-center rounded-[0.5rem] px-4 py-3",
        isCurrentUser ? "user-highlight" : "",
      ].join(" ")}
    >
      <div
        className={[
          "w-12 text-center font-body text-[18px] font-semibold",
          standing.rank === 1 || isCurrentUser ? "text-secondary" : "text-on-surface-variant",
        ].join(" ")}
      >
        {standing.rank}
      </div>

      <div className="flex flex-1 items-center gap-3">
        {standing.user.profileImageUrl ? (
          <img
            src={standing.user.profileImageUrl}
            alt=""
            className={[
              "h-8 w-8 rounded-full object-cover",
              isCurrentUser ? "border border-secondary" : "bg-surface-container-high",
            ].join(" ")}
          />
        ) : (
          <div className="flex h-8 w-8 items-center justify-center rounded-full bg-surface-container-high font-label text-[12px] text-on-surface">
            {getInitials(standing.user.username)}
          </div>
        )}
        <span
          className={[
            "font-body text-[16px] text-on-surface",
            isCurrentUser ? "font-bold" : "font-semibold",
          ].join(" ")}
        >
          {displayName}
        </span>
      </div>

      <div className="w-16 text-center font-body text-[16px] text-on-surface-variant">
        {standing.exactScoreCount}
      </div>
      <div
        className={[
          "w-20 text-right font-body text-[18px] font-semibold",
          isCurrentUser ? "font-bold text-secondary" : "text-on-surface",
        ].join(" ")}
      >
        {standing.totalPoints.toLocaleString("tr-TR")}
      </div>
    </div>
  );
}
