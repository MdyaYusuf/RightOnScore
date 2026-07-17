import { useEffect } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  COMPETITION_SEASON_STATUS,
  COMPETITION_SEASON_STATUS_LABEL,
  formatSeasonDateRange,
  type CompetitionSeasonStatus,
} from "../competitionSeasonTypes";
import {
  closeDialog,
  createGroupForStage,
  createSeasonForCompetition,
  createStageForSeason,
  loadGroupsForStage,
  loadSeasonStructure,
  loadStagesForSeason,
  openDialog,
  selectSeason,
  selectStage,
  updateGroupForm,
  updateSeasonForm,
  updateStageForm,
} from "../seasonStructureSlice";
import {
  COMPETITION_STAGE_TYPE,
  COMPETITION_STAGE_TYPE_LABEL,
  type CompetitionStageType,
} from "../../competitionStages/competitionStageTypes";

const STAGE_TYPE_OPTIONS: CompetitionStageType[] = [
  COMPETITION_STAGE_TYPE.LeagueTable,
  COMPETITION_STAGE_TYPE.GroupStage,
  COMPETITION_STAGE_TYPE.Knockout,
  COMPETITION_STAGE_TYPE.Final,
];

const SEASON_STATUS_OPTIONS: CompetitionSeasonStatus[] = [
  COMPETITION_SEASON_STATUS.Upcoming,
  COMPETITION_SEASON_STATUS.Active,
  COMPETITION_SEASON_STATUS.Finished,
  COMPETITION_SEASON_STATUS.Cancelled,
];

function seasonStatusBadgeClass(status: CompetitionSeasonStatus): string {
  if (status === COMPETITION_SEASON_STATUS.Active) {
    return "border-primary/20 bg-primary-container text-primary";
  }

  if (status === COMPETITION_SEASON_STATUS.Finished) {
    return "border-secondary/20 bg-secondary-container/20 text-secondary";
  }

  return "border-outline-variant/30 bg-surface-bright text-tertiary";
}

export function SeasonStructurePage() {
  const { competitionId = "" } = useParams();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const {
    competition,
    status,
    seasons,
    stages,
    groups,
    selectedSeasonId,
    selectedStageId,
    stagesStatus,
    groupsStatus,
    saveStatus,
    dialog,
    seasonForm,
    stageForm,
    groupForm,
  } = useAppSelector((state) => state.seasonStructure);

  const selectedSeason = seasons.find((season) => season.id === selectedSeasonId) ?? null;
  const selectedStage = stages.find((stage) => stage.id === selectedStageId) ?? null;
  const canManageGroups = selectedStage?.type === COMPETITION_STAGE_TYPE.GroupStage;
  const isSaving = saveStatus === "loading";
  const isLoading = status === "idle" || status === "loading";

  useEffect(() => {
    if (!competitionId) {
      return;
    }

    void dispatch(loadSeasonStructure(competitionId));
  }, [competitionId, dispatch]);

  useEffect(() => {
    if (!selectedSeasonId) {
      return;
    }

    void dispatch(loadStagesForSeason(selectedSeasonId));
  }, [selectedSeasonId, dispatch]);

  useEffect(() => {
    if (!selectedStageId) {
      return;
    }

    void dispatch(loadGroupsForStage(selectedStageId));
  }, [selectedStageId, dispatch]);

  async function handleCreateSeason() {
    if (!seasonForm.name.trim()) {
      toast.error("Sezon adı zorunludur.");
      return;
    }

    const result = await dispatch(createSeasonForCompetition());

    if (createSeasonForCompetition.fulfilled.match(result) && competitionId) {
      void dispatch(loadSeasonStructure(competitionId));
    }
  }

  async function handleCreateStage() {
    if (!stageForm.name.trim()) {
      toast.error("Aşama adı zorunludur.");
      return;
    }

    const result = await dispatch(createStageForSeason());

    if (createStageForSeason.fulfilled.match(result) && selectedSeasonId) {
      void dispatch(loadStagesForSeason(selectedSeasonId));
    }
  }

  async function handleCreateGroup() {
    if (!canManageGroups) {
      toast.error("Gruplar yalnızca grup aşamasına eklenebilir.");
      return;
    }

    if (!groupForm.name.trim()) {
      toast.error("Grup adı zorunludur.");
      return;
    }

    const result = await dispatch(createGroupForStage());

    if (createGroupForStage.fulfilled.match(result) && selectedStageId) {
      void dispatch(loadGroupsForStage(selectedStageId));
    }
  }

  function handleAddGroupClick() {
    if (!selectedStageId) {
      toast.error("Önce bir aşama seçin.");
      return;
    }

    if (!canManageGroups) {
      toast.error("Gruplar yalnızca 'Grup aşaması' tipine eklenebilir.");
      return;
    }

    dispatch(openDialog("group"));
  }

  return (
    <div className="relative mx-auto flex w-full max-w-[1440px] flex-col gap-8">
      <div className="hidden items-center gap-2 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant uppercase opacity-60 md:flex">
        <Link to="/admin/competitions" className="hover:text-secondary">
          {competition?.name ?? "Yarışma"}
        </Link>
        <span className="material-symbols-outlined text-[16px]">chevron_right</span>
        <span>{selectedSeason?.name ?? "Sezon"}</span>
        <span className="material-symbols-outlined text-[16px]">chevron_right</span>
        <span className="text-on-surface opacity-100">Yapılandırma</span>
      </div>

      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h2 className="mb-2 font-display text-[36px] leading-tight font-bold tracking-tight text-on-surface md:text-[48px] md:leading-[56px]">
            Sezon Yapılandırması
          </h2>
          <p className="font-body text-lg text-on-surface-variant">
            Hiyerarşik sezon, aşama ve grup düzenleyicisi.
          </p>
        </div>
        <div className="flex flex-wrap gap-4">
          {selectedSeasonId && (
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded border border-outline-variant/50 px-6 py-2.5 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant transition-all hover:border-secondary hover:text-secondary"
              onClick={() => navigate(`/admin/seasons/${selectedSeasonId}/teams`)}
            >
              <span className="material-symbols-outlined text-[18px]">group_add</span>
              Takımları Yönet
            </button>
          )}
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded border border-outline-variant/50 px-6 py-2.5 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant transition-all hover:border-secondary hover:text-secondary"
            onClick={() => navigate("/admin/competitions")}
          >
            <span className="material-symbols-outlined text-[18px]">close</span>
            İptal Et
          </button>
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded bg-secondary px-6 py-2.5 font-label text-[14px] font-semibold tracking-[0.05em] text-primary-container transition-all hover:bg-secondary/90"
            onClick={() => navigate("/admin/competitions")}
          >
            <span className="material-symbols-outlined text-[18px]">save</span>
            Değişiklikleri Kaydet
          </button>
        </div>
      </div>

      {isLoading && (
        <p className="py-10 text-center text-on-surface-variant">Yükleniyor…</p>
      )}

      {!isLoading && (
        <div className="grid min-h-[600px] grid-cols-1 gap-6 lg:grid-cols-12 lg:h-[calc(100vh-280px)]">
          {/* Seasons */}
          <section className="relative flex flex-col overflow-hidden rounded-lg border border-outline-variant/20 bg-surface-container lg:col-span-4">
            <div className="grain-overlay opacity-[0.4]" />
            <div className="relative z-20 flex items-center justify-between border-b border-outline-variant/10 bg-surface-container-high p-4">
              <h3 className="font-display text-2xl font-semibold tracking-[0.02em] text-secondary">
                Sezonlar
              </h3>
              <button
                type="button"
                className="flex h-8 w-8 items-center justify-center rounded border border-outline-variant/10 bg-primary-container text-primary transition-colors hover:bg-primary-container/80"
                aria-label="Sezon ekle"
                onClick={() => dispatch(openDialog("season"))}
              >
                <span className="material-symbols-outlined text-[20px]">add</span>
              </button>
            </div>
            <div className="relative z-20 flex-1 space-y-1 overflow-y-auto p-2">
              {seasons.length === 0 && (
                <p className="px-4 py-8 text-center text-sm text-on-surface-variant">
                  Henüz sezon yok.
                </p>
              )}
              {seasons.map((season) => {
                const isSelected = season.id === selectedSeasonId;

                return (
                  <button
                    key={season.id}
                    type="button"
                    className={[
                      "relative w-full rounded border p-4 text-left transition-colors",
                      isSelected
                        ? "border-secondary/50 bg-primary-container/30"
                        : "border-transparent hover:bg-surface-container-high",
                    ].join(" ")}
                    onClick={() => dispatch(selectSeason(season.id))}
                  >
                    {isSelected && (
                      <span className="absolute top-0 bottom-0 left-0 w-1 rounded-l bg-secondary" />
                    )}
                    <div className="mb-2 flex items-start justify-between gap-2">
                      <h4
                        className={[
                          "font-body text-base font-semibold",
                          isSelected ? "text-on-surface" : "text-on-surface-variant",
                        ].join(" ")}
                      >
                        {season.name}
                      </h4>
                      <span
                        className={[
                          "rounded border px-2 py-0.5 font-label text-[12px] font-medium",
                          seasonStatusBadgeClass(season.status),
                        ].join(" ")}
                      >
                        {COMPETITION_SEASON_STATUS_LABEL[season.status]}
                      </span>
                    </div>
                    <div className="flex items-center gap-1.5 font-label text-[12px] font-medium text-on-surface-variant">
                      <span className="material-symbols-outlined text-[14px]">
                        calendar_today
                      </span>
                      {formatSeasonDateRange(season.startDate, season.endDate)}
                    </div>
                  </button>
                );
              })}
            </div>
          </section>

          {/* Stages */}
          <section className="relative flex flex-col overflow-hidden rounded-lg border border-outline-variant/20 bg-surface-container opacity-95 lg:col-span-4">
            <div className="grain-overlay opacity-[0.4]" />
            <div className="relative z-20 flex items-center justify-between border-b border-outline-variant/10 bg-surface-container-high p-4">
              <div className="flex items-center gap-2">
                <span className="material-symbols-outlined hidden text-[20px] text-outline lg:block">
                  subdirectory_arrow_right
                </span>
                <h3 className="font-display text-2xl font-semibold tracking-[0.02em] text-on-surface">
                  Aşamalar{" "}
                  {selectedSeason && (
                    <span className="ml-1 text-sm font-normal text-outline">
                      {selectedSeason.name}
                    </span>
                  )}
                </h3>
              </div>
              <button
                type="button"
                className="flex h-8 w-8 items-center justify-center rounded border border-outline-variant/20 bg-surface-bright text-on-surface-variant transition-colors hover:bg-surface-container-highest hover:text-on-surface disabled:opacity-40"
                aria-label="Aşama ekle"
                disabled={!selectedSeasonId}
                onClick={() => dispatch(openDialog("stage"))}
              >
                <span className="material-symbols-outlined text-[20px]">add</span>
              </button>
            </div>
            <div className="relative z-20 flex-1 space-y-1 overflow-y-auto p-2">
              {!selectedSeasonId && (
                <p className="px-4 py-8 text-center text-sm text-on-surface-variant">
                  Aşamaları görmek için bir sezon seçin.
                </p>
              )}
              {selectedSeasonId && stagesStatus === "loading" && (
                <p className="px-4 py-8 text-center text-sm text-on-surface-variant">
                  Yükleniyor…
                </p>
              )}
              {selectedSeasonId && stagesStatus !== "loading" && stages.length === 0 && (
                <p className="px-4 py-8 text-center text-sm text-on-surface-variant">
                  Bu sezonda aşama yok.
                </p>
              )}
              {stages.map((stage) => {
                const isSelected = stage.id === selectedStageId;

                return (
                  <button
                    key={stage.id}
                    type="button"
                    className={[
                      "relative w-full rounded border p-4 text-left transition-colors",
                      isSelected
                        ? "border-outline-variant/40 bg-surface-container-highest shadow-sm"
                        : "border-transparent hover:bg-surface-container-high",
                    ].join(" ")}
                    onClick={() => dispatch(selectStage(stage.id))}
                  >
                    {isSelected && (
                      <span className="absolute top-0 bottom-0 left-0 w-0.5 rounded-l bg-outline" />
                    )}
                    <div className="mb-3 flex items-start justify-between gap-2">
                      <h4
                        className={[
                          "font-body text-base font-semibold",
                          isSelected ? "text-on-surface" : "text-on-surface-variant",
                        ].join(" ")}
                      >
                        {stage.name}
                      </h4>
                    </div>
                    <div className="flex flex-wrap gap-2">
                      <span className="rounded-sm border border-outline-variant/20 bg-surface px-2.5 py-1 font-label text-[12px] font-medium text-on-surface-variant">
                        {COMPETITION_STAGE_TYPE_LABEL[stage.type]}
                      </span>
                    </div>
                  </button>
                );
              })}
            </div>
          </section>

          {/* Groups */}
          <section className="relative flex flex-col overflow-hidden rounded-lg border border-outline-variant/20 bg-surface-container opacity-90 lg:col-span-4">
            <div className="grain-overlay opacity-[0.4]" />
            <div className="relative z-20 flex items-center justify-between border-b border-outline-variant/10 bg-surface-container-high p-4">
              <div className="flex items-center gap-2">
                <span className="material-symbols-outlined hidden text-[20px] text-outline/60 lg:block">
                  subdirectory_arrow_right
                </span>
                <h3 className="font-display text-2xl font-semibold tracking-[0.02em] text-on-surface">
                  Gruplar{" "}
                  {selectedStage && (
                    <span className="ml-1 text-sm font-normal text-outline/60">
                      {selectedStage.name.length > 12
                        ? `${selectedStage.name.slice(0, 10)}...`
                        : selectedStage.name}
                    </span>
                  )}
                </h3>
              </div>
              <button
                type="button"
                className="flex h-8 w-8 items-center justify-center rounded border border-outline-variant/20 bg-surface-bright text-on-surface-variant transition-colors hover:bg-surface-container-highest hover:text-on-surface disabled:opacity-40"
                aria-label="Grup ekle"
                disabled={!selectedStageId}
                onClick={handleAddGroupClick}
              >
                <span className="material-symbols-outlined text-[20px]">add</span>
              </button>
            </div>
            <div className="relative z-20 flex-1 p-4">
              {!selectedStageId && (
                <p className="py-8 text-center text-sm text-on-surface-variant">
                  Grupları görmek için bir aşama seçin.
                </p>
              )}
              {selectedStageId && groupsStatus === "loading" && (
                <p className="py-8 text-center text-sm text-on-surface-variant">Yükleniyor…</p>
              )}
              {selectedStageId &&
                groupsStatus !== "loading" &&
                groups.length === 0 &&
                !canManageGroups && (
                  <div className="flex h-full flex-col items-center justify-center rounded border border-outline-variant/20 bg-surface p-5 text-center">
                    <div className="mb-4 flex h-16 w-16 items-center justify-center rounded-full border border-outline-variant/10 bg-surface-bright">
                      <span className="material-symbols-outlined text-[32px] text-outline">
                        table_rows
                      </span>
                    </div>
                    <h4 className="mb-2 font-display text-2xl font-semibold text-on-surface">
                      Grup Bulunmuyor
                    </h4>
                    <p className="mb-6 max-w-[200px] font-body text-base text-on-surface-variant">
                      &apos;Puan durumu&apos; tipi aşamalar için varsayılan olarak tek bir ana
                      tablo oluşturulur.
                    </p>
                  </div>
                )}
              {selectedStageId &&
                groupsStatus !== "loading" &&
                groups.length === 0 &&
                canManageGroups && (
                  <div className="flex h-full flex-col items-center justify-center rounded border border-outline-variant/20 bg-surface p-5 text-center">
                    <div className="mb-4 flex h-16 w-16 items-center justify-center rounded-full border border-outline-variant/10 bg-surface-bright">
                      <span className="material-symbols-outlined text-[32px] text-outline">
                        table_rows
                      </span>
                    </div>
                    <h4 className="mb-2 font-display text-2xl font-semibold text-on-surface">
                      Grup Bulunmuyor
                    </h4>
                    <p className="mb-6 max-w-[200px] font-body text-base text-on-surface-variant">
                      Bu grup aşamasına henüz grup eklenmedi.
                    </p>
                    <button
                      type="button"
                      className="rounded border border-outline-variant/50 px-4 py-2 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface transition-all hover:border-secondary hover:text-secondary"
                      onClick={handleAddGroupClick}
                    >
                      Grup Ekle
                    </button>
                  </div>
                )}
              {groups.length > 0 && (
                <div className="space-y-2">
                  {groups.map((group) => (
                    <div
                      key={group.id}
                      className="rounded border border-outline-variant/20 bg-surface-container-high/40 px-4 py-3"
                    >
                      <div className="flex items-center justify-between gap-2">
                        <span className="font-body text-base font-semibold text-on-surface">
                          {group.name}
                        </span>
                        <span className="font-label text-[12px] text-on-surface-variant">
                          Sıra {group.displayOrder}
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </section>
        </div>
      )}

      {dialog !== "none" && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4">
          <div className="w-full max-w-md rounded-lg border border-outline-variant/20 bg-[#101e1b] shadow-2xl">
            <div className="flex items-center justify-between border-b border-outline-variant/10 p-5">
              <h3 className="font-display text-xl font-semibold text-on-surface">
                {dialog === "season" && "Yeni Sezon"}
                {dialog === "stage" && "Yeni Aşama"}
                {dialog === "group" && "Yeni Grup"}
              </h3>
              <button
                type="button"
                className="text-tertiary hover:text-on-surface"
                onClick={() => dispatch(closeDialog())}
              >
                <span className="material-symbols-outlined">close</span>
              </button>
            </div>

            <div className="flex flex-col gap-4 p-5">
              {dialog === "season" && (
                <>
                  <label className="flex flex-col gap-2">
                    <span className="font-label text-[14px] font-semibold text-tertiary">
                      Sezon Adı
                    </span>
                    <input
                      type="text"
                      value={seasonForm.name}
                      onChange={(event) =>
                        dispatch(updateSeasonForm({ name: event.target.value }))
                      }
                      className="rounded border border-outline-variant/30 bg-[#081613] px-4 py-3 text-on-surface outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
                    />
                  </label>
                  <div className="grid grid-cols-2 gap-3">
                    <label className="flex flex-col gap-2">
                      <span className="font-label text-[14px] font-semibold text-tertiary">
                        Başlangıç
                      </span>
                      <input
                        type="date"
                        value={seasonForm.startDate}
                        onChange={(event) =>
                          dispatch(updateSeasonForm({ startDate: event.target.value }))
                        }
                        className="rounded border border-outline-variant/30 bg-[#081613] px-3 py-3 text-on-surface outline-none focus:border-secondary"
                      />
                    </label>
                    <label className="flex flex-col gap-2">
                      <span className="font-label text-[14px] font-semibold text-tertiary">
                        Bitiş
                      </span>
                      <input
                        type="date"
                        value={seasonForm.endDate}
                        onChange={(event) =>
                          dispatch(updateSeasonForm({ endDate: event.target.value }))
                        }
                        className="rounded border border-outline-variant/30 bg-[#081613] px-3 py-3 text-on-surface outline-none focus:border-secondary"
                      />
                    </label>
                  </div>
                  <label className="flex flex-col gap-2">
                    <span className="font-label text-[14px] font-semibold text-tertiary">
                      Durum
                    </span>
                    <select
                      value={seasonForm.status}
                      onChange={(event) =>
                        dispatch(
                          updateSeasonForm({
                            status: Number(event.target.value) as CompetitionSeasonStatus,
                          }),
                        )
                      }
                      className="rounded border border-outline-variant/30 bg-[#081613] px-4 py-3 text-on-surface outline-none focus:border-secondary"
                    >
                      {SEASON_STATUS_OPTIONS.map((statusOption) => (
                        <option key={statusOption} value={statusOption}>
                          {COMPETITION_SEASON_STATUS_LABEL[statusOption]}
                        </option>
                      ))}
                    </select>
                  </label>
                  <label className="flex items-center justify-between gap-3 py-1">
                    <span className="font-label text-[14px] font-semibold text-on-surface">
                      Aktif sezon
                    </span>
                    <input
                      type="checkbox"
                      checked={seasonForm.isActive}
                      onChange={(event) =>
                        dispatch(updateSeasonForm({ isActive: event.target.checked }))
                      }
                      className="h-4 w-4 accent-secondary"
                    />
                  </label>
                </>
              )}

              {dialog === "stage" && (
                <>
                  <label className="flex flex-col gap-2">
                    <span className="font-label text-[14px] font-semibold text-tertiary">
                      Aşama Adı
                    </span>
                    <input
                      type="text"
                      value={stageForm.name}
                      onChange={(event) =>
                        dispatch(updateStageForm({ name: event.target.value }))
                      }
                      className="rounded border border-outline-variant/30 bg-[#081613] px-4 py-3 text-on-surface outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
                    />
                  </label>
                  <label className="flex flex-col gap-2">
                    <span className="font-label text-[14px] font-semibold text-tertiary">
                      Aşama Tipi
                    </span>
                    <select
                      value={stageForm.type}
                      onChange={(event) =>
                        dispatch(
                          updateStageForm({
                            type: Number(event.target.value) as CompetitionStageType,
                          }),
                        )
                      }
                      className="rounded border border-outline-variant/30 bg-[#081613] px-4 py-3 text-on-surface outline-none focus:border-secondary"
                    >
                      {STAGE_TYPE_OPTIONS.map((type) => (
                        <option key={type} value={type}>
                          {COMPETITION_STAGE_TYPE_LABEL[type]}
                        </option>
                      ))}
                    </select>
                  </label>
                  <div className="grid grid-cols-2 gap-3">
                    <label className="flex flex-col gap-2">
                      <span className="font-label text-[14px] font-semibold text-tertiary">
                        Başlangıç
                      </span>
                      <input
                        type="date"
                        value={stageForm.startDate}
                        onChange={(event) =>
                          dispatch(updateStageForm({ startDate: event.target.value }))
                        }
                        className="rounded border border-outline-variant/30 bg-[#081613] px-3 py-3 text-on-surface outline-none focus:border-secondary"
                      />
                    </label>
                    <label className="flex flex-col gap-2">
                      <span className="font-label text-[14px] font-semibold text-tertiary">
                        Bitiş
                      </span>
                      <input
                        type="date"
                        value={stageForm.endDate}
                        onChange={(event) =>
                          dispatch(updateStageForm({ endDate: event.target.value }))
                        }
                        className="rounded border border-outline-variant/30 bg-[#081613] px-3 py-3 text-on-surface outline-none focus:border-secondary"
                      />
                    </label>
                  </div>
                </>
              )}

              {dialog === "group" && (
                <label className="flex flex-col gap-2">
                  <span className="font-label text-[14px] font-semibold text-tertiary">
                    Grup Adı
                  </span>
                  <input
                    type="text"
                    value={groupForm.name}
                    onChange={(event) =>
                      dispatch(updateGroupForm({ name: event.target.value }))
                    }
                    className="rounded border border-outline-variant/30 bg-[#081613] px-4 py-3 text-on-surface outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
                    placeholder="Grup A"
                  />
                </label>
              )}
            </div>

            <div className="flex gap-3 border-t border-outline-variant/10 p-5">
              <button
                type="button"
                className="flex-1 rounded border border-outline-variant/40 py-3 font-label text-[14px] font-bold text-tertiary hover:bg-surface-container-high hover:text-on-surface"
                onClick={() => dispatch(closeDialog())}
                disabled={isSaving}
              >
                İptal
              </button>
              <button
                type="button"
                className="flex-1 rounded bg-secondary py-3 font-label text-[14px] font-bold text-on-secondary hover:bg-secondary-fixed disabled:opacity-60"
                disabled={isSaving}
                onClick={() => {
                  if (dialog === "season") {
                    void handleCreateSeason();
                  } else if (dialog === "stage") {
                    void handleCreateStage();
                  } else {
                    void handleCreateGroup();
                  }
                }}
              >
                {isSaving ? "Kaydediliyor…" : "Kaydet"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
