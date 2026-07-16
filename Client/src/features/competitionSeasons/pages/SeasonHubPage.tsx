import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  COMPETITION_SEASON_STATUS_LABEL,
  type CompetitionSeasonPreviewDto,
} from "../competitionSeasonTypes";
import {
  loadActiveSeasons,
  loadSelectedSeasonDetails,
  selectSeason,
} from "../seasonHubSlice";

function seasonIconName(index: number): string {
  return index % 2 === 0 ? "emoji_events" : "public";
}

export function SeasonHubPage() {
  const dispatch = useAppDispatch();
  const { status, seasons, selectedSeasonId, selectedSeason, myStanding, detailStatus } =
    useAppSelector((state) => state.seasonHub);

  useEffect(() => {
    void dispatch(loadActiveSeasons());
  }, [dispatch]);

  useEffect(() => {
    if (!selectedSeasonId) {
      return;
    }

    void dispatch(loadSelectedSeasonDetails(selectedSeasonId));
  }, [dispatch, selectedSeasonId]);

  function handleSelectSeason(seasonId: string) {
    if (seasonId === selectedSeasonId) {
      return;
    }

    dispatch(selectSeason(seasonId));
  }

  const otherSeasons = seasons.filter((season) => season.id !== selectedSeasonId);
  const selectedPreview = seasons.find((season) => season.id === selectedSeasonId) ?? null;

  return (
    <div className="mx-auto flex w-full max-w-[600px] animate-[authFadeIn_480ms_ease-out] flex-col gap-8">
      <header>
        <h1 className="mb-2 font-display text-[40px] font-bold leading-[1.1] tracking-[-0.02em] text-on-surface md:text-[48px]">
          Sezonlar
        </h1>
        <p className="text-lg text-on-surface-variant">
          Aktif bir turnuva seç, fikstürü gör ve tahmin yap.
        </p>
      </header>

      {status === "loading" && (
        <p className="text-on-surface-variant">Sezonlar yükleniyor...</p>
      )}

      {status === "error" && (
        <p className="text-error">Aktif sezonlar yüklenemedi. Daha sonra tekrar dene.</p>
      )}

      {status === "ready" && seasons.length === 0 && (
        <section className="card-level-1 rounded-[0.5rem] p-6">
          <p className="text-on-surface-variant">
            Şu an aktif turnuva yok. Admin bir sezon açtığında burada görünecek.
          </p>
        </section>
      )}

      {status === "ready" && selectedPreview && (
        <SelectedSeasonCard
          preview={selectedPreview}
          seasonName={selectedSeason?.name ?? selectedPreview.name}
          logoUrl={selectedSeason?.competition.logoUrl ?? null}
          myStanding={myStanding}
          isLoadingDetails={detailStatus === "loading"}
        />
      )}

      {status === "ready" && otherSeasons.length > 0 && (
        <section className="flex flex-col gap-4">
          <h3 className="mb-2 font-body text-[18px] font-semibold text-on-surface-variant">
            Diğer Turnuvalar
          </h3>

          {otherSeasons.map((season, index) => (
            <button
              key={season.id}
              type="button"
              onClick={() => handleSelectSeason(season.id)}
              className="card-level-1 group flex w-full items-center justify-between rounded-[0.5rem] border border-transparent p-5 text-left transition-colors hover:border-outline-variant/30 hover:bg-surface-container-high"
            >
              <div className="flex items-center gap-4">
                <div className="flex h-12 w-12 items-center justify-center rounded-full border border-outline-variant/20 bg-surface-container-lowest">
                  <span className="material-symbols-outlined text-primary transition-colors group-hover:text-secondary">
                    {seasonIconName(index)}
                  </span>
                </div>
                <div>
                  <h4 className="font-body text-[18px] font-semibold text-on-surface">
                    {season.name}
                  </h4>
                  <p className="mt-1 text-sm text-on-surface-variant">
                    {COMPETITION_SEASON_STATUS_LABEL[season.status]}
                  </p>
                </div>
              </div>
              <span className="material-symbols-outlined text-on-surface-variant group-hover:text-on-surface">
                chevron_right
              </span>
            </button>
          ))}
        </section>
      )}
    </div>
  );
}

type SelectedSeasonCardProps = {
  preview: CompetitionSeasonPreviewDto;
  seasonName: string;
  logoUrl: string | null;
  myStanding: {
    rank: number;
    totalPoints: number;
    exactScoreCount: number;
  } | null;
  isLoadingDetails: boolean;
};

function SelectedSeasonCard({
  preview,
  seasonName,
  logoUrl,
  myStanding,
  isLoadingDetails,
}: SelectedSeasonCardProps) {
  const rank = myStanding ? `#${myStanding.rank}` : "—";
  const points = myStanding ? String(myStanding.totalPoints) : "0";
  const exactScores = myStanding ? String(myStanding.exactScoreCount) : "0";

  return (
    <section className="card-level-2 group relative flex flex-col gap-6 overflow-hidden rounded-[0.75rem] p-6">
      <div className="absolute top-0 right-0 h-32 w-32 translate-x-1/2 -translate-y-1/2 rounded-full bg-primary/10 blur-3xl" />

      <div className="relative z-10 flex items-start justify-between">
        <div>
          <span className="mb-3 inline-block rounded-full border border-primary/20 bg-surface-container-lowest px-3 py-1 text-xs font-bold tracking-wider text-primary uppercase">
            Aktif Seçim
          </span>
          <h2 className="font-display text-[24px] font-semibold leading-[1.2] text-secondary md:text-[32px]">
            {seasonName}
          </h2>
          <p className="mt-1 text-sm text-on-primary-container">
            {COMPETITION_SEASON_STATUS_LABEL[preview.status]}
            {isLoadingDetails ? " • Yükleniyor..." : null}
          </p>
        </div>

        {logoUrl ? (
          <img
            src={logoUrl}
            alt=""
            className="h-16 w-16 object-contain opacity-80"
          />
        ) : (
          <div className="flex h-16 w-16 items-center justify-center rounded-full border border-outline-variant/20 bg-surface-container-lowest">
            <span className="material-symbols-outlined text-3xl text-primary">emoji_events</span>
          </div>
        )}
      </div>

      <div className="relative z-10 grid grid-cols-3 gap-4 border-t border-primary/20 pt-4">
        <div>
          <p className="mb-1 text-sm tracking-wider text-on-primary-container uppercase">Sıra</p>
          <p className="font-display text-[36px] font-bold leading-none text-on-surface">{rank}</p>
        </div>
        <div>
          <p className="mb-1 text-sm tracking-wider text-on-primary-container uppercase">Puan</p>
          <p className="font-display text-[36px] font-bold leading-none text-on-surface">{points}</p>
        </div>
        <div>
          <p className="mb-1 text-sm tracking-wider text-on-primary-container uppercase">
            Tam İskor
          </p>
          <p className="font-display text-[36px] font-bold leading-none text-secondary">
            {exactScores}
          </p>
        </div>
      </div>
    </section>
  );
}
