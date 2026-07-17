import { useEffect, useMemo } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import { COMPETITION_STAGE_TYPE } from "../../competitionStages/competitionStageTypes";
import { teamInitials } from "../../teams/teamTypes";
import {
  assignTeamToSeason,
  loadSeasonTeamsPage,
  setCountryFilter,
  setPoolSearch,
  setSortMode,
  unassignTeamFromSeason,
  updateAssignedTeamPlacement,
} from "../seasonTeamsSlice";

export function SeasonTeamsPage() {
  const { seasonId = "" } = useParams();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const {
    status,
    actionStatus,
    season,
    catalogTeams,
    assignedTeams,
    stages,
    groupsByStageId,
    poolSearch,
    countryFilter,
    sortMode,
  } = useAppSelector((state) => state.seasonTeams);

  useEffect(() => {
    if (!seasonId) {
      return;
    }

    void dispatch(loadSeasonTeamsPage(seasonId));
  }, [seasonId, dispatch]);

  const assignedTeamIds = useMemo(
    () => new Set(assignedTeams.map((item) => item.teamId)),
    [assignedTeams],
  );

  const countryOptions = useMemo(() => {
    const countries = new Set(
      catalogTeams.map((team) => team.country).filter(Boolean),
    );
    return Array.from(countries).sort((a, b) => a.localeCompare(b, "tr"));
  }, [catalogTeams]);

  const availableTeams = useMemo(() => {
    const query = poolSearch.trim().toLocaleLowerCase("tr-TR");

    return catalogTeams.filter((team) => {
      if (assignedTeamIds.has(team.id)) {
        return false;
      }

      if (countryFilter && team.country !== countryFilter) {
        return false;
      }

      if (!query) {
        return true;
      }

      return (
        team.name.toLocaleLowerCase("tr-TR").includes(query) ||
        team.shortName.toLocaleLowerCase("tr-TR").includes(query)
      );
    });
  }, [catalogTeams, assignedTeamIds, countryFilter, poolSearch]);

  const sortedAssigned = useMemo(() => {
    const items = [...assignedTeams];

    if (sortMode === "name") {
      items.sort((a, b) => a.team.name.localeCompare(b.team.name, "tr"));
    } else if (sortMode === "recent") {
      items.reverse();
    } else {
      items.sort((a, b) => {
        const stageA = stages.find((stage) => stage.id === a.competitionStageId);
        const stageB = stages.find((stage) => stage.id === b.competitionStageId);
        const orderA = stageA?.displayOrder ?? 999;
        const orderB = stageB?.displayOrder ?? 999;

        if (orderA !== orderB) {
          return orderA - orderB;
        }

        return a.team.name.localeCompare(b.team.name, "tr");
      });
    }

    return items;
  }, [assignedTeams, sortMode, stages]);

  const isLoading = status === "idle" || status === "loading";
  const isBusy = actionStatus === "loading";
  const structurePath = season
    ? `/admin/competitions/${season.competitionId}/structure`
    : "/admin/competitions";

  async function handleAssign(teamId: string) {
    const result = await dispatch(assignTeamToSeason(teamId));

    if (assignTeamToSeason.fulfilled.match(result) && seasonId) {
      void dispatch(loadSeasonTeamsPage(seasonId));
    } else if (assignTeamToSeason.rejected.match(result)) {
      toast.error(String(result.payload ?? "Takım atanamadı."));
    }
  }

  async function handleUnassign(competitionTeamId: string) {
    const result = await dispatch(unassignTeamFromSeason(competitionTeamId));

    if (unassignTeamFromSeason.fulfilled.match(result) && seasonId) {
      void dispatch(loadSeasonTeamsPage(seasonId));
    } else if (unassignTeamFromSeason.rejected.match(result)) {
      toast.error(String(result.payload ?? "Takım kaldırılamadı."));
    }
  }

  async function handleStageChange(competitionTeamId: string, stageId: string) {
    const nextStageId = stageId || null;
    const stage = stages.find((item) => item.id === nextStageId);
    const allowGroup = stage?.type === COMPETITION_STAGE_TYPE.GroupStage;

    const result = await dispatch(
      updateAssignedTeamPlacement({
        competitionTeamId,
        competitionStageId: nextStageId,
        competitionGroupId: allowGroup
          ? assignedTeams.find((item) => item.id === competitionTeamId)
              ?.competitionGroupId ?? null
          : null,
      }),
    );

    if (updateAssignedTeamPlacement.fulfilled.match(result) && seasonId) {
      void dispatch(loadSeasonTeamsPage(seasonId));
    } else if (updateAssignedTeamPlacement.rejected.match(result)) {
      toast.error(String(result.payload ?? "Aşama güncellenemedi."));
    }
  }

  async function handleGroupChange(competitionTeamId: string, groupId: string) {
    const existing = assignedTeams.find((item) => item.id === competitionTeamId);

    if (!existing) {
      return;
    }

    const result = await dispatch(
      updateAssignedTeamPlacement({
        competitionTeamId,
        competitionStageId: existing.competitionStageId,
        competitionGroupId: groupId || null,
      }),
    );

    if (updateAssignedTeamPlacement.fulfilled.match(result) && seasonId) {
      void dispatch(loadSeasonTeamsPage(seasonId));
    } else if (updateAssignedTeamPlacement.rejected.match(result)) {
      toast.error(String(result.payload ?? "Grup güncellenemedi."));
    }
  }

  return (
    <div className="mx-auto flex w-full max-w-7xl flex-col gap-8">
      <div className="flex flex-col justify-between gap-4 md:flex-row md:items-end">
        <div>
          <nav className="mb-3 flex gap-2 font-label text-[11px] font-medium tracking-wider text-on-surface-variant/60 uppercase">
            <Link to="/admin/competitions" className="hover:text-secondary">
              Yarışmalar
            </Link>
            <span>/</span>
            <Link to={structurePath} className="hover:text-secondary">
              {season?.competition.name ?? "Yarışma"}
            </Link>
            <span>/</span>
            <span className="text-on-surface">Sezon Yönetimi</span>
          </nav>
          <h2 className="flex flex-wrap items-center gap-3 font-display text-[36px] leading-tight font-bold text-on-surface md:text-[48px] md:leading-[56px]">
            {season?.competition.name ?? "Yarışma"}
            <span className="font-light text-outline-variant">|</span>
            <span className="text-secondary">{season?.name ?? "Sezon"}</span>
          </h2>
          <p className="mt-2 max-w-2xl font-body text-base text-on-surface-variant">
            Bu sezon için takımları atayın, grupları belirleyin ve başlangıç aşamalarını
            yapılandırın. Sağdaki liste bu sezonda aktif olan takımları gösterir.
          </p>
        </div>
        <div className="flex flex-wrap gap-3">
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded border border-outline-variant/50 px-4 py-2 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface transition-colors hover:border-secondary hover:text-secondary"
            onClick={() => navigate(structurePath)}
          >
            <span className="material-symbols-outlined text-sm">settings</span>
            Sezon Ayarları
          </button>
          <button
            type="button"
            className="inline-flex items-center gap-2 rounded border border-secondary/50 bg-secondary px-6 py-2 font-label text-[14px] font-bold tracking-[0.05em] text-primary-container transition-colors hover:bg-secondary-fixed"
            onClick={() => navigate(structurePath)}
          >
            <span className="material-symbols-outlined text-sm">save</span>
            Değişiklikleri Kaydet
          </button>
        </div>
      </div>

      {isLoading && (
        <p className="py-16 text-center text-on-surface-variant">Yükleniyor…</p>
      )}

      {!isLoading && (
        <div className="flex min-h-[500px] flex-col gap-6 lg:h-[calc(100vh-280px)] lg:flex-row">
          {/* Available pool */}
          <section className="flex w-full shrink-0 flex-col overflow-hidden rounded-xl border border-outline-variant/20 bg-surface-container lg:w-1/3">
            <div className="flex shrink-0 flex-col gap-3 border-b border-outline-variant/10 bg-surface-container-high p-4">
              <div className="flex items-center justify-between">
                <h3 className="font-display text-lg font-semibold text-on-surface">
                  Mevcut Takımlar
                </h3>
                <span className="rounded bg-surface-container-low px-2 py-0.5 text-[11px] font-bold text-on-surface-variant">
                  {availableTeams.length}
                </span>
              </div>
              <div className="relative">
                <span className="material-symbols-outlined absolute top-1/2 left-3 -translate-y-1/2 text-sm text-on-surface-variant">
                  search
                </span>
                <input
                  type="search"
                  value={poolSearch}
                  onChange={(event) => dispatch(setPoolSearch(event.target.value))}
                  placeholder="Takım ara..."
                  className="w-full rounded border border-outline-variant/30 bg-surface py-2 pr-3 pl-9 font-label text-[12px] text-on-surface outline-none transition-colors focus:border-secondary focus:ring-1 focus:ring-secondary"
                />
              </div>
              <div className="flex gap-2 overflow-x-auto pb-1">
                <button
                  type="button"
                  className={[
                    "whitespace-nowrap rounded-full border px-3 py-1 font-label text-[11px] font-medium transition-colors",
                    !countryFilter
                      ? "border-secondary/30 bg-secondary/10 text-secondary"
                      : "border-outline-variant/30 bg-surface text-on-surface-variant hover:text-on-surface",
                  ].join(" ")}
                  onClick={() => dispatch(setCountryFilter(null))}
                >
                  Tümü
                </button>
                {countryOptions.map((country) => (
                  <button
                    key={country}
                    type="button"
                    className={[
                      "whitespace-nowrap rounded-full border px-3 py-1 font-label text-[11px] font-medium transition-colors",
                      countryFilter === country
                        ? "border-secondary/30 bg-secondary/10 text-secondary"
                        : "border-outline-variant/30 bg-surface text-on-surface-variant hover:text-on-surface",
                    ].join(" ")}
                    onClick={() => dispatch(setCountryFilter(country))}
                  >
                    {country}
                  </button>
                ))}
              </div>
            </div>

            <div className="flex-1 overflow-y-auto p-2">
              {availableTeams.length === 0 && (
                <div className="flex h-full flex-col items-center justify-center p-6 text-center opacity-50">
                  <span className="material-symbols-outlined mb-2 text-4xl text-outline">
                    search_off
                  </span>
                  <p className="font-label text-[14px] font-semibold text-on-surface">
                    Sonuç bulunamadı
                  </p>
                  <p className="mt-1 text-[12px] text-on-surface-variant">
                    Arama kriterlerinizi değiştirin.
                  </p>
                </div>
              )}

              {availableTeams.map((team) => (
                <div
                  key={team.id}
                  className="group mb-1 flex cursor-pointer items-center justify-between rounded border border-transparent p-3 transition-colors hover:border-outline-variant/10 hover:bg-surface-container-high"
                >
                  <div className="flex min-w-0 items-center gap-3">
                    <div className="flex h-8 w-8 items-center justify-center overflow-hidden rounded border border-outline-variant/20 bg-surface p-1">
                      {team.crestUrl ? (
                        <img
                          src={team.crestUrl}
                          alt=""
                          className="h-full w-full object-contain"
                        />
                      ) : (
                        <span className="text-[10px] font-bold text-on-surface">
                          {teamInitials(team.shortName, team.name)}
                        </span>
                      )}
                    </div>
                    <div className="min-w-0">
                      <div className="truncate font-label text-[14px] font-semibold text-on-surface transition-colors group-hover:text-secondary">
                        {team.name}
                      </div>
                      <div className="text-[11px] text-on-surface-variant">
                        {team.shortName} • {team.country}
                      </div>
                    </div>
                  </div>
                  <button
                    type="button"
                    disabled={isBusy}
                    className="flex h-8 w-8 items-center justify-center rounded-full border border-outline-variant/30 bg-surface text-on-surface-variant opacity-0 transition-all group-hover:opacity-100 hover:border-secondary hover:bg-secondary hover:text-primary-container focus:opacity-100 disabled:opacity-40"
                    aria-label={`${team.name} ekle`}
                    onClick={() => void handleAssign(team.id)}
                  >
                    <span className="material-symbols-outlined text-[18px]">add</span>
                  </button>
                </div>
              ))}
            </div>
          </section>

          {/* Assigned teams */}
          <section className="relative flex w-full flex-col overflow-hidden rounded-xl border border-outline-variant/20 bg-[#04110e] lg:w-2/3">
            <div className="flex shrink-0 items-center justify-between border-b border-outline-variant/10 bg-surface-container-low p-4">
              <div className="flex items-center gap-3">
                <h3 className="font-display text-lg font-semibold text-on-surface">
                  Sezon Takımları
                </h3>
                <span className="rounded bg-primary-container px-2 py-0.5 text-[11px] font-bold text-secondary">
                  {assignedTeams.length}
                </span>
              </div>
              <select
                value={sortMode}
                onChange={(event) =>
                  dispatch(
                    setSortMode(event.target.value as "stage" | "name" | "recent"),
                  )
                }
                className="rounded border border-outline-variant/30 bg-surface px-2 py-1 font-label text-[12px] text-on-surface outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
              >
                <option value="stage">Aşamaya Göre Grupla</option>
                <option value="name">İsme Göre (A-Z)</option>
                <option value="recent">Son Eklenenler</option>
              </select>
            </div>

            <div className="flex-1 overflow-y-auto bg-surface/50 p-6">
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                {sortedAssigned.map((assignment) => {
                  const stage = stages.find(
                    (item) => item.id === assignment.competitionStageId,
                  );
                  const isGroupStage =
                    stage?.type === COMPETITION_STAGE_TYPE.GroupStage;
                  const groups = assignment.competitionStageId
                    ? (groupsByStageId[assignment.competitionStageId] ?? [])
                    : [];

                  return (
                    <div
                      key={assignment.id}
                      className="group relative flex flex-col rounded-lg border border-outline-variant/20 bg-surface-container-high/40 p-4 backdrop-blur-sm transition-all hover:border-secondary/50 hover:bg-surface-container-high/60"
                    >
                      <button
                        type="button"
                        disabled={isBusy}
                        className="absolute top-2 right-2 flex h-8 w-8 items-center justify-center rounded text-outline opacity-0 transition-colors group-hover:opacity-100 hover:bg-error/10 hover:text-error disabled:opacity-40"
                        aria-label="Kaldır"
                        onClick={() => void handleUnassign(assignment.id)}
                      >
                        <span className="material-symbols-outlined text-[18px]">close</span>
                      </button>

                      <div className="mb-4 flex items-start gap-4">
                        <div className="flex h-12 w-12 shrink-0 items-center justify-center overflow-hidden rounded border border-outline-variant/30 bg-surface p-1.5">
                          {assignment.team.crestUrl ? (
                            <img
                              src={assignment.team.crestUrl}
                              alt=""
                              className="h-full w-full object-contain"
                            />
                          ) : (
                            <span className="text-xs font-bold text-on-surface">
                              {teamInitials(
                                assignment.team.shortName,
                                assignment.team.name,
                              )}
                            </span>
                          )}
                        </div>
                        <div className="pr-8">
                          <h4 className="font-display text-[16px] leading-tight font-semibold text-on-surface">
                            {assignment.team.name}
                          </h4>
                          <div className="mt-0.5 text-[12px] text-on-surface-variant">
                            {assignment.team.shortName}
                          </div>
                        </div>
                      </div>

                      <div className="mt-auto space-y-2 border-t border-outline-variant/10 pt-3">
                        <div className="flex items-center justify-between gap-2">
                          <span className="text-[11px] tracking-wider text-on-surface-variant uppercase">
                            Aşama
                          </span>
                          <select
                            value={assignment.competitionStageId ?? ""}
                            disabled={isBusy}
                            onChange={(event) =>
                              void handleStageChange(assignment.id, event.target.value)
                            }
                            className="h-7 min-w-[120px] rounded border border-outline-variant/30 bg-surface px-2 py-0.5 text-[12px] text-on-surface outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
                          >
                            <option value="">Seçilmedi</option>
                            {stages.map((stageOption) => (
                              <option key={stageOption.id} value={stageOption.id}>
                                {stageOption.name}
                              </option>
                            ))}
                          </select>
                        </div>
                        <div className="flex items-center justify-between gap-2">
                          <span className="text-[11px] tracking-wider text-on-surface-variant uppercase">
                            Grup
                          </span>
                          <select
                            value={assignment.competitionGroupId ?? ""}
                            disabled={isBusy || !isGroupStage || groups.length === 0}
                            onChange={(event) =>
                              void handleGroupChange(assignment.id, event.target.value)
                            }
                            className="h-7 min-w-[120px] rounded border border-outline-variant/30 bg-surface px-2 py-0.5 text-[12px] text-on-surface outline-none focus:border-secondary focus:ring-1 focus:ring-secondary disabled:opacity-50"
                          >
                            <option value="">
                              {isGroupStage ? "Seçilmedi" : "—"}
                            </option>
                            {groups.map((group) => (
                              <option key={group.id} value={group.id}>
                                {group.name}
                              </option>
                            ))}
                          </select>
                        </div>
                      </div>
                    </div>
                  );
                })}

                <div className="flex min-h-[160px] flex-col items-center justify-center rounded-lg border-2 border-dashed border-outline-variant/20 p-4 text-center opacity-60 transition-all hover:border-secondary/50 hover:bg-surface-container-low hover:opacity-100">
                  <span className="material-symbols-outlined mb-2 text-3xl text-on-surface-variant">
                    library_add
                  </span>
                  <p className="font-label text-sm font-semibold text-on-surface">
                    Takım Ekle
                  </p>
                  <p className="mt-1 text-[11px] text-on-surface-variant">
                    Sol panelden seçim yapın
                  </p>
                </div>
              </div>
            </div>
          </section>
        </div>
      )}
    </div>
  );
}
