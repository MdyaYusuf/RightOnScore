import { useEffect, useMemo, type ReactNode } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import { COMPETITION_STAGE_TYPE } from "../../competitionStages/competitionStageTypes";
import {
  canCorrectResult,
  canEnterOrCorrectResult,
  fixtureContextLabel,
  fixtureStagePrimaryLabel,
  fixtureStageSecondaryLabel,
  formatFixtureDateLabel,
  formatFixtureTime,
  matchStatusLabel,
} from "../adminFixturesHelpers";
import {
  closeDialog,
  createFixture,
  loadActiveSeasonsForFixtures,
  loadAdminFixturesPage,
  openCreateDialog,
  openMatchMenu,
  openResultDialog,
  removeFixture,
  saveMatchResult,
  setStageFilter,
  setStatusFilter,
  updateCreateForm,
  updateResultForm,
} from "../adminFixturesSlice";
import {
  isKnockoutStage,
  MATCH_STATUS,
  type MatchPreviewDto,
  type MatchStatus,
} from "../matchTypes";

export function AdminFixturesPage() {
  const { seasonId: routeSeasonId = "" } = useParams();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const {
    status,
    actionStatus,
    seasonId,
    season,
    activeSeasons,
    matches,
    stages,
    groupsByStageId,
    seasonTeams,
    stageFilter,
    statusFilter,
    dialog,
    selectedMatchId,
    createForm,
    resultForm,
  } = useAppSelector((state) => state.adminFixtures);

  useEffect(() => {
    void dispatch(loadActiveSeasonsForFixtures());
  }, [dispatch]);

  useEffect(() => {
    if (routeSeasonId) {
      void dispatch(loadAdminFixturesPage(routeSeasonId));
      return;
    }

    if (activeSeasons.length === 0) {
      return;
    }

    navigate(`/admin/seasons/${activeSeasons[0].id}/fixtures`, { replace: true });
  }, [routeSeasonId, activeSeasons, dispatch, navigate]);

  const selectedMatch = useMemo(
    () => matches.find((match) => match.id === selectedMatchId) ?? null,
    [matches, selectedMatchId],
  );

  const filteredMatches = useMemo(() => {
    return matches.filter((match) => {
      if (stageFilter !== "all" && match.competitionStageId !== stageFilter) {
        return false;
      }

      if (statusFilter !== "all" && match.status !== statusFilter) {
        return false;
      }

      return true;
    });
  }, [matches, stageFilter, statusFilter]);

  const createGroups = createForm.competitionStageId
    ? (groupsByStageId[createForm.competitionStageId] ?? [])
    : [];

  const createStage = stages.find((stage) => stage.id === createForm.competitionStageId);
  const showCreateGroups = createStage?.type === COMPETITION_STAGE_TYPE.GroupStage;

  const isKnockoutResult = selectedMatch ? isKnockoutStage(selectedMatch.competitionStage) : false;
  const homeScoreNum = Number.parseInt(resultForm.homeScore, 10);
  const awayScoreNum = Number.parseInt(resultForm.awayScore, 10);
  const showAdvancing =
    isKnockoutResult &&
    !Number.isNaN(homeScoreNum) &&
    !Number.isNaN(awayScoreNum) &&
    homeScoreNum === awayScoreNum;

  const isLoading = Boolean(routeSeasonId) && (status === "idle" || status === "loading");
  const isBusy = actionStatus === "loading";
  const pageTitle = season?.name ?? "Maçlar";

  async function handleCreate() {
    const result = await dispatch(createFixture());

    if (createFixture.fulfilled.match(result) && (seasonId || routeSeasonId)) {
      void dispatch(loadAdminFixturesPage(seasonId ?? routeSeasonId));
    } else if (createFixture.rejected.match(result)) {
      toast.error(String(result.payload ?? "Maç oluşturulamadı."));
    }
  }

  async function handleSaveResult() {
    const result = await dispatch(saveMatchResult());

    if (saveMatchResult.fulfilled.match(result) && (seasonId || routeSeasonId)) {
      void dispatch(loadAdminFixturesPage(seasonId ?? routeSeasonId));
    } else if (saveMatchResult.rejected.match(result)) {
      toast.error(String(result.payload ?? "Sonuç kaydedilemedi."));
    }
  }

  async function handleDelete() {
    if (!selectedMatchId) {
      return;
    }

    const result = await dispatch(removeFixture(selectedMatchId));

    if (removeFixture.rejected.match(result)) {
      toast.error(String(result.payload ?? "Maç silinemedi."));
    }
  }

  function handleSeasonChange(nextSeasonId: string) {
    if (!nextSeasonId) {
      return;
    }

    navigate(`/admin/seasons/${nextSeasonId}/fixtures`);
  }

  return (
    <div className="mx-auto flex w-full max-w-[1440px] flex-col gap-8">
      <div className="flex flex-col justify-between gap-6 lg:flex-row lg:items-center">
        <div className="flex flex-col gap-2">
          <div className="flex flex-wrap items-center gap-3">
            <h2 className="font-display text-[36px] leading-tight font-bold tracking-tight text-on-surface md:text-[48px] md:leading-[56px]">
              {pageTitle}
            </h2>
            {activeSeasons.length > 1 && (
              <select
                className="rounded border border-outline-variant/30 bg-[#101e1b] px-3 py-2 font-label text-[12px] font-medium text-on-surface outline-none focus:border-secondary"
                value={routeSeasonId || seasonId || ""}
                onChange={(event) => handleSeasonChange(event.target.value)}
                aria-label="Sezon seç"
              >
                {activeSeasons.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.name}
                  </option>
                ))}
              </select>
            )}
          </div>
          <p className="font-body text-base text-on-surface-variant">
            Fikstür ve maç sonuçları yönetimi.
          </p>
        </div>

        <div className="flex flex-wrap items-center gap-4">
          <div className="flex items-center rounded-lg border border-outline-variant/20 bg-[#101e1b] p-1">
            <select
              className="cursor-pointer appearance-none border-none bg-transparent py-2 pr-8 pl-3 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface outline-none hover:text-secondary"
              value={stageFilter}
              onChange={(event) => dispatch(setStageFilter(event.target.value))}
              aria-label="Aşama filtresi"
            >
              <option value="all">Tüm Aşamalar</option>
              {stages.map((stage) => (
                <option key={stage.id} value={stage.id}>
                  {stage.name}
                </option>
              ))}
            </select>
            <div className="mx-2 my-auto h-6 w-px bg-outline-variant/30" />
            <select
              className="cursor-pointer appearance-none border-none bg-transparent py-2 pr-8 pl-3 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface outline-none hover:text-secondary"
              value={statusFilter === "all" ? "all" : String(statusFilter)}
              onChange={(event) => {
                const value = event.target.value;
                dispatch(
                  setStatusFilter(value === "all" ? "all" : (Number(value) as MatchStatus)),
                );
              }}
              aria-label="Durum filtresi"
            >
              <option value="all">Tüm Durumlar</option>
              <option value={MATCH_STATUS.Scheduled}>Planlandı</option>
              <option value={MATCH_STATUS.Live}>Canlı</option>
              <option value={MATCH_STATUS.Finished}>Bitti</option>
              <option value={MATCH_STATUS.Postponed}>Ertelendi</option>
              <option value={MATCH_STATUS.Cancelled}>İptal</option>
            </select>
          </div>

          <button
            type="button"
            className="inline-flex items-center gap-2 rounded-lg border border-secondary/50 bg-secondary px-6 py-3 font-label text-[14px] font-semibold tracking-[0.05em] text-primary-container shadow-[0_0_15px_rgba(233,195,73,0.1)] transition-colors hover:bg-secondary/90 disabled:opacity-50"
            disabled={!routeSeasonId && !seasonId}
            onClick={() => dispatch(openCreateDialog())}
          >
            <span className="material-symbols-outlined text-[20px]">add</span>
            Yeni Maç
          </button>
        </div>
      </div>

      <div className="admin-surface-texture overflow-hidden rounded-xl border border-outline-variant/20 bg-[#04110e] shadow-lg">
        <div className="hidden grid-cols-12 gap-4 border-b border-outline-variant/10 bg-[#101e1b]/50 px-6 py-4 md:grid">
          <div className="col-span-3 font-label text-[12px] font-medium tracking-widest text-outline uppercase opacity-80">
            Maç
          </div>
          <div className="col-span-2 font-label text-[12px] font-medium tracking-widest text-outline uppercase opacity-80">
            Zaman
          </div>
          <div className="col-span-2 font-label text-[12px] font-medium tracking-widest text-outline uppercase opacity-80">
            Aşama/Grup
          </div>
          <div className="col-span-2 font-label text-[12px] font-medium tracking-widest text-outline uppercase opacity-80">
            Durum
          </div>
          <div className="col-span-3 text-right font-label text-[12px] font-medium tracking-widest text-outline uppercase opacity-80">
            İşlemler
          </div>
        </div>

        <div className="flex flex-col divide-y divide-outline-variant/10">
          {isLoading && (
            <p className="px-6 py-10 text-center text-on-surface-variant">Yükleniyor…</p>
          )}

          {!isLoading && !routeSeasonId && activeSeasons.length === 0 && (
            <p className="px-6 py-10 text-center text-on-surface-variant">
              Aktif sezon bulunamadı. Önce bir sezon oluşturun.
            </p>
          )}

          {!isLoading && routeSeasonId && filteredMatches.length === 0 && (
            <p className="px-6 py-10 text-center text-on-surface-variant">
              Bu filtrelerle maç bulunamadı.
            </p>
          )}

          {filteredMatches.map((match) => (
            <FixtureRow
              key={match.id}
              match={match}
              onRecord={() => dispatch(openResultDialog(match.id))}
              onMenu={() => dispatch(openMatchMenu(match.id))}
            />
          ))}
        </div>
      </div>

      {dialog === "result" && selectedMatch && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-[#081613]/80 backdrop-blur-sm"
          onClick={() => dispatch(closeDialog())}
          role="presentation"
        >
          <div
            className="w-full max-w-lg overflow-hidden rounded-xl border border-outline-variant/20 bg-[#101e1b] shadow-2xl"
            onClick={(event) => event.stopPropagation()}
            role="dialog"
            aria-modal="true"
            aria-labelledby="result-modal-title"
          >
            <div className="flex items-center justify-between border-b border-outline-variant/10 bg-[#0d1a17] px-6 py-4">
              <h3 id="result-modal-title" className="font-display text-xl font-semibold text-on-surface">
                {canCorrectResult(selectedMatch.status) ? "Maç Sonucu Düzelt" : "Maç Sonucu Gir"}
              </h3>
              <button
                type="button"
                className="rounded-full p-1 text-outline-variant transition-colors hover:bg-surface-container-high hover:text-on-surface"
                onClick={() => dispatch(closeDialog())}
                aria-label="Kapat"
              >
                <span className="material-symbols-outlined">close</span>
              </button>
            </div>

            <div className="p-6">
              <div className="mb-6 text-center">
                <p className="mb-1 font-label text-[12px] font-medium tracking-wider text-outline uppercase">
                  {fixtureContextLabel(selectedMatch)}
                </p>
                <div className="flex items-center justify-center gap-6">
                  <span className="font-display text-2xl font-semibold text-on-surface">
                    {selectedMatch.homeTeam.name}
                  </span>
                  <span className="text-outline-variant">vs</span>
                  <span className="font-display text-2xl font-semibold text-on-surface">
                    {selectedMatch.awayTeam.name}
                  </span>
                </div>
              </div>

              <div className="mb-8 flex items-center justify-center gap-8">
                <div className="flex flex-col items-center gap-2">
                  <label className="font-label text-[12px] text-outline" htmlFor="home-score">
                    Ev Sahibi
                  </label>
                  <input
                    id="home-score"
                    type="number"
                    min={0}
                    className="h-24 w-20 rounded-lg border border-outline-variant/30 bg-primary-container/30 text-center font-display text-4xl text-on-surface outline-none transition-colors focus:border-secondary focus:ring-1 focus:ring-secondary"
                    value={resultForm.homeScore}
                    onChange={(event) =>
                      dispatch(updateResultForm({ homeScore: event.target.value }))
                    }
                  />
                </div>
                <span className="mt-6 font-display text-2xl text-outline-variant">-</span>
                <div className="flex flex-col items-center gap-2">
                  <label className="font-label text-[12px] text-outline" htmlFor="away-score">
                    Deplasman
                  </label>
                  <input
                    id="away-score"
                    type="number"
                    min={0}
                    className="h-24 w-20 rounded-lg border border-outline-variant/30 bg-primary-container/30 text-center font-display text-4xl text-on-surface outline-none transition-colors focus:border-secondary focus:ring-1 focus:ring-secondary"
                    value={resultForm.awayScore}
                    onChange={(event) =>
                      dispatch(updateResultForm({ awayScore: event.target.value }))
                    }
                  />
                </div>
              </div>

              {showAdvancing && (
                <div className="rounded-lg border border-outline-variant/20 bg-[#0a1613] p-4">
                  <label className="mb-3 block font-label text-[14px] font-medium text-on-surface">
                    Tur Atlayan Takım (Beraberlik Durumunda)
                  </label>
                  <div className="flex gap-4">
                    {[selectedMatch.homeTeam, selectedMatch.awayTeam].map((team) => (
                      <label key={team.id} className="flex-1 cursor-pointer">
                        <input
                          className="peer sr-only"
                          type="radio"
                          name="advancing"
                          checked={resultForm.advancingTeamId === team.id}
                          onChange={() =>
                            dispatch(updateResultForm({ advancingTeamId: team.id }))
                          }
                        />
                        <div className="w-full rounded-lg border border-outline-variant/30 px-4 py-3 text-center font-label text-[14px] font-semibold text-outline transition-colors peer-checked:border-secondary peer-checked:bg-secondary/10 peer-checked:text-secondary">
                          {team.name}
                        </div>
                      </label>
                    ))}
                  </div>
                </div>
              )}
            </div>

            <div className="flex justify-end gap-3 border-t border-outline-variant/10 bg-[#0d1a17] px-6 py-4">
              <button
                type="button"
                className="rounded-lg border border-outline-variant/30 px-6 py-2 font-label text-[14px] font-semibold text-on-surface transition-colors hover:bg-surface-container-high"
                onClick={() => dispatch(closeDialog())}
              >
                İptal Et
              </button>
              <button
                type="button"
                className="rounded-lg border border-secondary/50 bg-secondary px-6 py-2 font-label text-[14px] font-semibold text-primary-container shadow-[0_0_15px_rgba(233,195,73,0.1)] transition-colors hover:bg-secondary/90 disabled:opacity-50"
                disabled={isBusy}
                onClick={() => void handleSaveResult()}
              >
                {canCorrectResult(selectedMatch.status) ? "Düzelt ve Yeniden Puanla" : "Kaydet"}
              </button>
            </div>
          </div>
        </div>
      )}

      {dialog === "create" && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-[#081613]/80 backdrop-blur-sm"
          onClick={() => dispatch(closeDialog())}
          role="presentation"
        >
          <div
            className="max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-xl border border-outline-variant/20 bg-[#101e1b] shadow-2xl"
            onClick={(event) => event.stopPropagation()}
            role="dialog"
            aria-modal="true"
            aria-labelledby="create-modal-title"
          >
            <div className="flex items-center justify-between border-b border-outline-variant/10 bg-[#0d1a17] px-6 py-4">
              <h3 id="create-modal-title" className="font-display text-xl font-semibold text-on-surface">
                Yeni Maç
              </h3>
              <button
                type="button"
                className="rounded-full p-1 text-outline-variant transition-colors hover:bg-surface-container-high hover:text-on-surface"
                onClick={() => dispatch(closeDialog())}
                aria-label="Kapat"
              >
                <span className="material-symbols-outlined">close</span>
              </button>
            </div>

            <div className="flex flex-col gap-4 p-6">
              <FieldSelect
                label="Aşama"
                value={createForm.competitionStageId}
                onChange={(value) => dispatch(updateCreateForm({ competitionStageId: value }))}
              >
                <option value="">Aşama seçin</option>
                {stages.map((stage) => (
                  <option key={stage.id} value={stage.id}>
                    {stage.name}
                  </option>
                ))}
              </FieldSelect>

              {showCreateGroups && (
                <FieldSelect
                  label="Grup"
                  value={createForm.competitionGroupId}
                  onChange={(value) => dispatch(updateCreateForm({ competitionGroupId: value }))}
                >
                  <option value="">Grup seçin (opsiyonel)</option>
                  {createGroups.map((group) => (
                    <option key={group.id} value={group.id}>
                      {group.name}
                    </option>
                  ))}
                </FieldSelect>
              )}

              <FieldSelect
                label="Ev Sahibi"
                value={createForm.homeTeamId}
                onChange={(value) => dispatch(updateCreateForm({ homeTeamId: value }))}
              >
                <option value="">Takım seçin</option>
                {seasonTeams.map((item) => (
                  <option key={item.teamId} value={item.teamId}>
                    {item.team.name}
                  </option>
                ))}
              </FieldSelect>

              <FieldSelect
                label="Deplasman"
                value={createForm.awayTeamId}
                onChange={(value) => dispatch(updateCreateForm({ awayTeamId: value }))}
              >
                <option value="">Takım seçin</option>
                {seasonTeams.map((item) => (
                  <option key={item.teamId} value={item.teamId}>
                    {item.team.name}
                  </option>
                ))}
              </FieldSelect>

              <label className="flex flex-col gap-1.5">
                <span className="font-label text-[12px] font-medium text-outline">Başlama</span>
                <input
                  type="datetime-local"
                  className="rounded-lg border border-outline-variant/30 bg-[#0a1613] px-3 py-2.5 font-label text-[14px] text-on-surface outline-none focus:border-secondary"
                  value={createForm.kickoffTime}
                  onChange={(event) =>
                    dispatch(updateCreateForm({ kickoffTime: event.target.value }))
                  }
                />
              </label>

              <label className="flex flex-col gap-1.5">
                <span className="font-label text-[12px] font-medium text-outline">
                  Hafta / Tur (ops.)
                </span>
                <input
                  type="number"
                  min={1}
                  className="rounded-lg border border-outline-variant/30 bg-[#0a1613] px-3 py-2.5 font-label text-[14px] text-on-surface outline-none focus:border-secondary"
                  value={createForm.round}
                  onChange={(event) => dispatch(updateCreateForm({ round: event.target.value }))}
                />
              </label>

              <label className="flex flex-col gap-1.5">
                <span className="font-label text-[12px] font-medium text-outline">
                  Stadyum (ops.)
                </span>
                <input
                  type="text"
                  className="rounded-lg border border-outline-variant/30 bg-[#0a1613] px-3 py-2.5 font-label text-[14px] text-on-surface outline-none focus:border-secondary"
                  value={createForm.venue}
                  onChange={(event) => dispatch(updateCreateForm({ venue: event.target.value }))}
                />
              </label>
            </div>

            <div className="flex justify-end gap-3 border-t border-outline-variant/10 bg-[#0d1a17] px-6 py-4">
              <button
                type="button"
                className="rounded-lg border border-outline-variant/30 px-6 py-2 font-label text-[14px] font-semibold text-on-surface transition-colors hover:bg-surface-container-high"
                onClick={() => dispatch(closeDialog())}
              >
                İptal Et
              </button>
              <button
                type="button"
                className="rounded-lg border border-secondary/50 bg-secondary px-6 py-2 font-label text-[14px] font-semibold text-primary-container transition-colors hover:bg-secondary/90 disabled:opacity-50"
                disabled={isBusy}
                onClick={() => void handleCreate()}
              >
                Oluştur
              </button>
            </div>
          </div>
        </div>
      )}

      {dialog === "menu" && selectedMatch && (
        <div
          className="fixed inset-0 z-50 flex items-end justify-center bg-[#081613]/60 sm:items-center"
          onClick={() => dispatch(closeDialog())}
          role="presentation"
        >
          <div
            className="w-full max-w-sm overflow-hidden rounded-t-xl border border-outline-variant/20 bg-[#101e1b] shadow-2xl sm:rounded-xl"
            onClick={(event) => event.stopPropagation()}
            role="dialog"
            aria-modal="true"
          >
            <div className="border-b border-outline-variant/10 px-4 py-3">
              <p className="font-label text-[14px] font-semibold text-on-surface">
                {selectedMatch.homeTeam.shortName} vs {selectedMatch.awayTeam.shortName}
              </p>
            </div>
            <div className="flex flex-col p-2">
              <button
                type="button"
                className="rounded px-4 py-3 text-left font-label text-[14px] font-semibold text-on-surface transition-colors hover:bg-surface-container-high"
                onClick={() => {
                  dispatch(closeDialog());
                  navigate(`/admin/matches/${selectedMatch.id}/predictions`);
                }}
              >
                Tahminleri Gör
              </button>
              {canEnterOrCorrectResult(selectedMatch.status) && (
                <button
                  type="button"
                  className="rounded px-4 py-3 text-left font-label text-[14px] font-semibold text-on-surface transition-colors hover:bg-surface-container-high"
                  onClick={() => dispatch(openResultDialog(selectedMatch.id))}
                >
                  {canCorrectResult(selectedMatch.status) ? "Sonuç Düzelt" : "Sonuç Gir"}
                </button>
              )}
              <button
                type="button"
                className="rounded px-4 py-3 text-left font-label text-[14px] font-semibold text-error transition-colors hover:bg-error/10"
                disabled={isBusy}
                onClick={() => void handleDelete()}
              >
                Maçı Sil
              </button>
              <button
                type="button"
                className="rounded px-4 py-3 text-left font-label text-[14px] font-semibold text-on-surface-variant transition-colors hover:bg-surface-container-high"
                onClick={() => dispatch(closeDialog())}
              >
                İptal
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function FieldSelect({
  label,
  value,
  onChange,
  children,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  children: ReactNode;
}) {
  return (
    <label className="flex flex-col gap-1.5">
      <span className="font-label text-[12px] font-medium text-outline">{label}</span>
      <select
        className="rounded-lg border border-outline-variant/30 bg-[#0a1613] px-3 py-2.5 font-label text-[14px] text-on-surface outline-none focus:border-secondary"
        value={value}
        onChange={(event) => onChange(event.target.value)}
      >
        {children}
      </select>
    </label>
  );
}

function FixtureRow({
  match,
  onRecord,
  onMenu,
}: {
  match: MatchPreviewDto;
  onRecord: () => void;
  onMenu: () => void;
}) {
  const finished = match.status === MATCH_STATUS.Finished;
  const live = match.status === MATCH_STATUS.Live;
  const secondary = fixtureStageSecondaryLabel(match);
  const actionLabel = finished ? "Sonuç Düzelt" : "Sonuç Gir";
  const actionIcon = finished ? "edit" : "edit_square";
  const canAct = canEnterOrCorrectResult(match.status);

  return (
    <div className="group grid grid-cols-1 items-center gap-4 px-6 py-5 transition-colors hover:bg-[#101e1b]/30 md:grid-cols-12">
      <div className="flex items-center gap-4 md:col-span-3">
        <div className="flex w-24 flex-col items-end gap-1">
          <span
            className={[
              "font-display text-base font-semibold",
              finished ? "text-on-surface-variant" : "text-on-surface",
            ].join(" ")}
          >
            {match.homeTeam.shortName || match.homeTeam.name}
          </span>
          <span
            className={[
              "font-display text-base",
              finished ? "text-on-surface-variant" : "text-on-surface-variant",
            ].join(" ")}
          >
            {match.awayTeam.shortName || match.awayTeam.name}
          </span>
        </div>
        <div
          className={[
            "flex h-14 w-12 flex-col items-center justify-center rounded border",
            live
              ? "border-outline-variant/20 bg-surface-container-high"
              : finished
                ? "border-outline-variant/10 bg-[#0a1613]"
                : "border-outline-variant/10 bg-[#0d1a17]",
          ].join(" ")}
        >
          <span
            className={[
              "font-display text-xl leading-none",
              live ? "text-secondary" : finished ? "text-on-surface-variant" : "text-outline-variant",
            ].join(" ")}
          >
            {match.homeScore ?? "-"}
          </span>
          <div className="my-1 h-px w-full bg-outline-variant/20" />
          <span
            className={[
              "font-display text-xl leading-none",
              live ? "text-secondary" : finished ? "text-on-surface-variant" : "text-outline-variant",
            ].join(" ")}
          >
            {match.awayScore ?? "-"}
          </span>
        </div>
      </div>

      <div
        className={[
          "flex flex-col justify-center md:col-span-2",
          finished ? "opacity-60" : "",
        ].join(" ")}
      >
        <span className="font-body text-base text-on-surface">
          {formatFixtureDateLabel(match.kickoffTime)}
        </span>
        <span className="font-label text-[12px] text-outline">
          {formatFixtureTime(match.kickoffTime)}
        </span>
      </div>

      <div
        className={[
          "flex flex-col justify-center md:col-span-2",
          finished ? "opacity-60" : "",
        ].join(" ")}
      >
        <span className="font-body text-base text-on-surface">
          {fixtureStagePrimaryLabel(match)}
        </span>
        {secondary && (
          <span className="font-label text-[12px] text-outline">{secondary}</span>
        )}
      </div>

      <div className="flex items-center md:col-span-2">
        <StatusChip status={match.status} />
      </div>

      <div className="flex justify-start gap-3 md:col-span-3 md:justify-end md:opacity-0 md:transition-opacity md:group-hover:opacity-100">
        {canAct && (
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded border border-outline-variant/30 px-4 py-2 font-label text-[14px] font-semibold text-outline transition-colors hover:border-outline-variant hover:text-on-surface"
            onClick={onRecord}
          >
            <span className="material-symbols-outlined text-[18px]">{actionIcon}</span>
            {actionLabel}
          </button>
        )}
        <button
          type="button"
          className="rounded border border-outline-variant/30 p-2 text-outline transition-colors hover:border-outline-variant hover:text-on-surface"
          onClick={onMenu}
          aria-label="Diğer işlemler"
        >
          <span className="material-symbols-outlined text-[18px]">more_vert</span>
        </button>
      </div>
    </div>
  );
}

function StatusChip({ status }: { status: MatchStatus }) {
  const label = matchStatusLabel(status);

  if (status === MATCH_STATUS.Live) {
    return (
      <span className="inline-flex items-center gap-1.5 rounded-full border border-primary/30 bg-primary/10 px-3 py-1 font-label text-[12px] font-medium text-primary">
        <span className="h-1.5 w-1.5 animate-pulse rounded-full bg-primary" />
        {label}
      </span>
    );
  }

  if (status === MATCH_STATUS.Finished) {
    return (
      <span className="inline-flex items-center rounded-full border border-secondary/30 bg-secondary/10 px-3 py-1 font-label text-[12px] font-medium text-secondary">
        {label}
      </span>
    );
  }

  return (
    <span className="inline-flex items-center rounded-full border border-outline-variant/30 px-3 py-1 font-label text-[12px] font-medium text-outline">
      {label}
    </span>
  );
}
