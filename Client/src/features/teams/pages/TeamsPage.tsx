import { useEffect, useMemo, useRef, type FormEvent } from "react";
import { toast } from "react-toastify";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  clearSearchMode,
  closePanel,
  loadTeams,
  openCreatePanel,
  openEditPanel,
  removeTeam,
  saveTeam,
  searchTeams,
  selectTeam,
  setCrestFile,
  setSearchQuery,
  updateForm,
} from "../teamsSlice";
import { TEAM_COUNTRY_OPTIONS, teamInitials } from "../teamTypes";

export function TeamsPage() {
  const dispatch = useAppDispatch();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const {
    status,
    saveStatus,
    items,
    pageNumber,
    totalCount,
    totalPages,
    searchQuery,
    isSearchMode,
    selectedId,
    panelMode,
    form,
  } = useAppSelector((state) => state.teams);

  useEffect(() => {
    void dispatch(loadTeams(1));
  }, [dispatch]);

  const countryOptions = useMemo(() => {
    const options = new Set<string>(TEAM_COUNTRY_OPTIONS);

    if (form.country.trim()) {
      options.add(form.country.trim());
    }

    return Array.from(options);
  }, [form.country]);

  const isLoading = status === "idle" || status === "loading";
  const isSaving = saveStatus === "saving";
  const isPanelOpen = panelMode !== "closed";
  const panelTitle = panelMode === "create" ? "Yeni Takım" : "Takım Düzenle";
  const crestDisplayUrl = form.crestPreviewUrl ?? form.crestUrl;
  const crestLabel = teamInitials(form.shortName, form.name || "T");

  async function handleSearchSubmit(event: FormEvent) {
    event.preventDefault();
    const trimmed = searchQuery.trim();

    if (!trimmed) {
      dispatch(clearSearchMode());
      void dispatch(loadTeams(1));
      return;
    }

    if (trimmed.length < 2) {
      toast.error("Arama için en az 2 karakter girin.");
      return;
    }

    void dispatch(searchTeams(trimmed));
  }

  async function handleSave() {
    if (!form.name.trim() || !form.shortName.trim() || !form.country.trim()) {
      toast.error("Takım adı, kısa ad ve ülke zorunludur.");
      return;
    }

    const result = await dispatch(saveTeam());

    if (saveTeam.fulfilled.match(result)) {
      if (isSearchMode && searchQuery.trim().length >= 2) {
        void dispatch(searchTeams(searchQuery.trim()));
      } else {
        void dispatch(loadTeams(pageNumber));
      }
    }
  }

  async function handleDelete() {
    if (!window.confirm("Bu takımı silmek istediğinize emin misiniz?")) {
      return;
    }

    const result = await dispatch(removeTeam());

    if (removeTeam.fulfilled.match(result)) {
      if (isSearchMode && searchQuery.trim().length >= 2) {
        void dispatch(searchTeams(searchQuery.trim()));
      } else {
        const nextPage =
          items.length <= 1 && pageNumber > 1 ? pageNumber - 1 : pageNumber;
        void dispatch(loadTeams(nextPage));
      }
    }
  }

  function handleCrestPick(file: File | null) {
    if (!file) {
      dispatch(setCrestFile(null));
      return;
    }

    if (!file.type.startsWith("image/")) {
      toast.error("Lütfen bir görsel dosyası seçin.");
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      toast.error("Görsel en fazla 5MB olabilir.");
      return;
    }

    dispatch(setCrestFile(file));
  }

  return (
    <div className="relative mx-auto flex w-full max-w-[1600px] flex-col gap-6 lg:flex-row">
      <div className="flex min-w-0 flex-1 flex-col">
        <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 className="mb-1 font-display text-[36px] leading-tight font-bold text-on-surface md:text-[48px] md:leading-[56px]">
              Takımlar
            </h2>
            <p className="font-body text-base text-on-surface-variant">
              Sistemdeki tüm takımları yönetin ve düzenleyin.
            </p>
          </div>
          <button
            type="button"
            className="inline-flex items-center gap-2 self-start rounded bg-secondary px-6 py-3 font-label text-[14px] font-semibold tracking-[0.05em] text-on-secondary whitespace-nowrap transition-colors hover:bg-secondary-fixed sm:self-auto"
            onClick={() => dispatch(openCreatePanel())}
          >
            <span className="material-symbols-outlined text-[20px]">add</span>
            Yeni takım
          </button>
        </div>

        <form
          className="mb-4 flex items-center gap-3"
          onSubmit={(event) => void handleSearchSubmit(event)}
        >
          <div className="relative w-full max-w-md">
            <span className="material-symbols-outlined absolute top-1/2 left-3 -translate-y-1/2 text-[20px] text-on-surface-variant">
              search
            </span>
            <input
              type="search"
              value={searchQuery}
              onChange={(event) => dispatch(setSearchQuery(event.target.value))}
              placeholder="Takım Ara..."
              className="h-10 w-full rounded border border-outline-variant/20 bg-surface-container py-2 pr-4 pl-10 font-body text-sm text-on-surface outline-none transition-all placeholder:text-on-surface-variant/50 focus:border-secondary focus:ring-1 focus:ring-secondary"
            />
          </div>
          <button
            type="submit"
            className="rounded border border-outline-variant/30 px-4 py-2 font-label text-[14px] font-semibold text-on-surface-variant transition-colors hover:border-secondary hover:text-secondary"
          >
            Ara
          </button>
          {isSearchMode && (
            <button
              type="button"
              className="font-label text-[14px] text-tertiary hover:text-secondary"
              onClick={() => {
                dispatch(clearSearchMode());
                void dispatch(loadTeams(1));
              }}
            >
              Temizle
            </button>
          )}
        </form>

        <div className="admin-surface-texture flex flex-1 flex-col overflow-hidden rounded-lg border border-outline-variant/10 bg-[#101e1b]">
          <div className="overflow-x-auto">
            <table className="w-full border-collapse text-left">
              <thead>
                <tr className="border-b border-outline-variant/10">
                  <th className="px-4 py-3 font-display text-[14px] font-semibold tracking-wider text-tertiary/80 uppercase whitespace-nowrap">
                    Arma
                  </th>
                  <th className="px-4 py-3 font-display text-[14px] font-semibold tracking-wider text-tertiary/80 uppercase whitespace-nowrap">
                    Takım Adı
                  </th>
                  <th className="px-4 py-3 font-display text-[14px] font-semibold tracking-wider text-tertiary/80 uppercase whitespace-nowrap">
                    Kısa Ad
                  </th>
                  <th className="px-4 py-3 font-display text-[14px] font-semibold tracking-wider text-tertiary/80 uppercase whitespace-nowrap">
                    Ülke
                  </th>
                  <th className="px-4 py-3 font-display text-[14px] font-semibold tracking-wider text-tertiary/80 uppercase whitespace-nowrap">
                    Durum
                  </th>
                  <th className="px-4 py-3 text-right font-display text-[14px] font-semibold tracking-wider text-tertiary/80 uppercase whitespace-nowrap">
                    İşlemler
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-outline-variant/10">
                {isLoading && (
                  <tr>
                    <td colSpan={6} className="px-4 py-10 text-center text-on-surface-variant">
                      Yükleniyor…
                    </td>
                  </tr>
                )}

                {!isLoading && items.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-4 py-10 text-center text-on-surface-variant">
                      {isSearchMode
                        ? "Aramayla eşleşen takım bulunamadı."
                        : "Henüz takım yok. Yeni takım ekleyerek başlayın."}
                    </td>
                  </tr>
                )}

                {items.map((team) => {
                  const isSelected = selectedId === team.id && panelMode === "edit";
                  const initials = teamInitials(team.shortName, team.name);

                  return (
                    <tr
                      key={team.id}
                      className={[
                        "group cursor-pointer transition-colors hover:bg-surface-container/50",
                        isSelected ? "bg-primary-container/10" : "",
                        !team.isActive ? "opacity-70" : "",
                      ].join(" ")}
                      onClick={() => dispatch(selectTeam(team.id))}
                    >
                      <td className="px-4 py-3">
                        <div className="flex h-10 w-10 items-center justify-center overflow-hidden rounded border border-outline-variant/20 bg-surface-container-high">
                          {team.crestUrl ? (
                            <img
                              src={team.crestUrl}
                              alt=""
                              className="h-full w-full object-cover"
                            />
                          ) : (
                            <span className="font-display text-[16px] font-bold text-on-surface">
                              {initials}
                            </span>
                          )}
                        </div>
                      </td>
                      <td
                        className={[
                          "px-4 py-3 font-body text-on-surface",
                          isSelected ? "font-semibold" : "",
                        ].join(" ")}
                      >
                        {team.name}
                      </td>
                      <td className="px-4 py-3 font-body text-on-surface-variant">
                        {team.shortName}
                      </td>
                      <td className="px-4 py-3 font-body text-on-surface-variant">
                        {team.country}
                      </td>
                      <td className="px-4 py-3">
                        {team.isActive ? (
                          <span className="inline-flex items-center rounded-full border border-primary/20 bg-primary/10 px-2.5 py-0.5 text-[12px] font-medium text-primary">
                            Aktif
                          </span>
                        ) : (
                          <span className="inline-flex items-center rounded-full border border-outline-variant/30 bg-surface-container-highest px-2.5 py-0.5 text-[12px] font-medium text-on-surface-variant">
                            Pasif
                          </span>
                        )}
                      </td>
                      <td className="px-4 py-3 text-right">
                        <button
                          type="button"
                          className="rounded p-1 text-on-surface-variant opacity-0 transition-all group-hover:opacity-100 hover:text-secondary focus:opacity-100"
                          aria-label="Düzenle"
                          onClick={(event) => {
                            event.stopPropagation();
                            dispatch(openEditPanel(team.id));
                          }}
                        >
                          <span className="material-symbols-outlined text-[20px]">edit</span>
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          <div className="mt-auto flex items-center justify-between border-t border-outline-variant/10 p-4">
            <span className="font-label text-[12px] font-medium text-on-surface-variant">
              Toplam {totalCount} takım
            </span>
            {!isSearchMode && (
              <div className="flex items-center gap-2">
                <button
                  type="button"
                  className="flex h-8 w-8 items-center justify-center rounded border border-outline-variant/20 text-on-surface-variant transition-colors hover:border-secondary hover:text-secondary disabled:opacity-50"
                  disabled={pageNumber <= 1 || isLoading}
                  onClick={() => void dispatch(loadTeams(pageNumber - 1))}
                  aria-label="Önceki sayfa"
                >
                  <span className="material-symbols-outlined text-[20px]">chevron_left</span>
                </button>
                <span className="px-2 font-label text-[12px] font-medium text-on-surface">
                  {pageNumber} / {totalPages}
                </span>
                <button
                  type="button"
                  className="flex h-8 w-8 items-center justify-center rounded border border-outline-variant/20 text-on-surface-variant transition-colors hover:border-secondary hover:text-secondary disabled:opacity-50"
                  disabled={pageNumber >= totalPages || isLoading}
                  onClick={() => void dispatch(loadTeams(pageNumber + 1))}
                  aria-label="Sonraki sayfa"
                >
                  <span className="material-symbols-outlined text-[20px]">chevron_right</span>
                </button>
              </div>
            )}
          </div>
        </div>
      </div>

      {isPanelOpen && (
        <aside className="flex w-full shrink-0 flex-col lg:w-[400px]">
          <div className="admin-surface-texture sticky top-[88px] overflow-hidden rounded-lg border border-outline-variant/10 bg-[#101e1b]">
            <div className="flex items-center justify-between border-b border-outline-variant/10 bg-surface-container/50 p-6">
              <h3 className="font-display text-2xl font-semibold tracking-[0.02em] text-on-surface">
                {panelTitle}
              </h3>
              <div className="flex items-center gap-2">
                <span className="font-label text-[12px] font-medium text-on-surface-variant">
                  Aktif
                </span>
                <button
                  type="button"
                  role="switch"
                  aria-checked={form.isActive}
                  className={[
                    "relative inline-flex h-6 w-11 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors duration-200 ease-in-out focus:ring-2 focus:ring-secondary focus:ring-offset-2 focus:ring-offset-[#081613] focus:outline-none",
                    form.isActive ? "bg-secondary" : "bg-outline-variant/40",
                  ].join(" ")}
                  onClick={() => dispatch(updateForm({ isActive: !form.isActive }))}
                >
                  <span
                    className={[
                      "pointer-events-none inline-block h-5 w-5 transform rounded-full bg-background shadow transition duration-200 ease-in-out",
                      form.isActive ? "translate-x-5" : "translate-x-0",
                    ].join(" ")}
                  />
                </button>
              </div>
            </div>

            <div className="flex flex-col gap-4 p-6">
              <div className="flex flex-col gap-2">
                <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant">
                  Arma
                </span>
                <button
                  type="button"
                  className="group flex cursor-pointer flex-col items-center justify-center gap-3 rounded-lg border-2 border-dashed border-outline-variant/30 p-6 transition-all hover:border-secondary/50 hover:bg-surface-container"
                  onClick={() => fileInputRef.current?.click()}
                >
                  <div className="mb-2 flex h-16 w-16 items-center justify-center overflow-hidden rounded border border-outline-variant/20 bg-surface-container-high">
                    {crestDisplayUrl ? (
                      <img
                        src={crestDisplayUrl}
                        alt=""
                        className="h-full w-full object-cover"
                      />
                    ) : (
                      <span className="font-display text-2xl font-bold text-on-surface transition-transform group-hover:scale-110">
                        {crestLabel}
                      </span>
                    )}
                  </div>
                  <div className="text-center">
                    <p className="mb-1 font-label text-[14px] font-semibold text-secondary group-hover:underline">
                      Görsel yükle
                    </p>
                    <p className="font-label text-[12px] font-medium text-on-surface-variant">
                      PNG, JPG veya WebP (max 5MB)
                    </p>
                  </div>
                </button>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/png,image/jpeg,image/webp"
                  className="hidden"
                  onChange={(event) =>
                    handleCrestPick(event.target.files?.[0] ?? null)
                  }
                />
              </div>

              <label className="flex flex-col gap-2">
                <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant">
                  Takım Adı
                </span>
                <input
                  type="text"
                  value={form.name}
                  onChange={(event) => dispatch(updateForm({ name: event.target.value }))}
                  className="h-11 w-full rounded border border-outline-variant/30 bg-primary-container/20 px-4 font-body text-base text-on-surface outline-none transition-all focus:border-secondary focus:ring-1 focus:ring-secondary"
                />
              </label>

              <label className="flex flex-col gap-2">
                <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant">
                  Kısa Ad
                </span>
                <input
                  type="text"
                  value={form.shortName}
                  maxLength={30}
                  onChange={(event) =>
                    dispatch(updateForm({ shortName: event.target.value.toUpperCase() }))
                  }
                  className="h-11 w-full rounded border border-outline-variant/30 bg-primary-container/20 px-4 font-body text-base text-on-surface uppercase outline-none transition-all focus:border-secondary focus:ring-1 focus:ring-secondary"
                />
              </label>

              <label className="flex flex-col gap-2">
                <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant">
                  Ülke
                </span>
                <div className="relative">
                  <select
                    value={form.country}
                    onChange={(event) =>
                      dispatch(updateForm({ country: event.target.value }))
                    }
                    className="h-11 w-full appearance-none rounded border border-outline-variant/30 bg-primary-container/20 px-4 pr-10 font-body text-base text-on-surface outline-none transition-all focus:border-secondary focus:ring-1 focus:ring-secondary"
                  >
                    {countryOptions.map((country) => (
                      <option key={country} value={country}>
                        {country}
                      </option>
                    ))}
                  </select>
                  <span className="material-symbols-outlined pointer-events-none absolute top-1/2 right-3 -translate-y-1/2 text-on-surface-variant">
                    expand_more
                  </span>
                </div>
              </label>

              <div className="mt-2 flex gap-3 border-t border-outline-variant/10 pt-6">
                <button
                  type="button"
                  className="flex-1 rounded border border-outline-variant/30 px-4 py-3 font-label text-[14px] font-semibold tracking-[0.05em] text-tertiary transition-colors hover:bg-surface-container"
                  onClick={() => dispatch(closePanel())}
                  disabled={isSaving}
                >
                  İptal Et
                </button>
                <button
                  type="button"
                  className="flex-1 rounded bg-secondary px-4 py-3 font-label text-[14px] font-semibold tracking-[0.05em] text-on-secondary transition-colors hover:bg-secondary-fixed disabled:opacity-60"
                  onClick={() => void handleSave()}
                  disabled={isSaving}
                >
                  {isSaving ? "Kaydediliyor…" : "Kaydet"}
                </button>
              </div>

              {panelMode === "edit" && (
                <div className="mt-2 flex justify-center">
                  <button
                    type="button"
                    className="flex items-center gap-1 font-label text-[12px] font-medium text-error opacity-80 hover:underline hover:opacity-100"
                    onClick={() => void handleDelete()}
                    disabled={isSaving}
                  >
                    <span className="material-symbols-outlined text-[16px]">delete</span>
                    Takımı Sil
                  </button>
                </div>
              )}
            </div>
          </div>
        </aside>
      )}
    </div>
  );
}
