import { useEffect, useMemo } from "react";
import { toast } from "react-toastify";
import { useAppDispatch, useAppSelector } from "../../../core/store/hooks";
import {
  closePanel,
  loadCompetitions,
  openCreatePanel,
  openEditPanel,
  saveCompetition,
  selectCompetition,
  updateForm,
} from "../competitionsSlice";
import {
  COMPETITION_COUNTRY_OPTIONS,
  COMPETITION_TYPE,
  COMPETITION_TYPE_ICON,
  COMPETITION_TYPE_LABEL,
  type CompetitionType,
} from "../competitionTypes";

const TYPE_OPTIONS: CompetitionType[] = [
  COMPETITION_TYPE.League,
  COMPETITION_TYPE.Cup,
  COMPETITION_TYPE.Hybrid,
];

export function CompetitionsPage() {
  const dispatch = useAppDispatch();
  const { status, saveStatus, items, selectedId, panelMode, form } = useAppSelector(
    (state) => state.competitions,
  );

  useEffect(() => {
    void dispatch(loadCompetitions());
  }, [dispatch]);

  const countryOptions = useMemo(() => {
    const options = new Set<string>(COMPETITION_COUNTRY_OPTIONS);

    if (form.country.trim()) {
      options.add(form.country.trim());
    }

    return Array.from(options);
  }, [form.country]);

  const isLoading = status === "idle" || status === "loading";
  const isSaving = saveStatus === "saving";
  const isPanelOpen = panelMode !== "closed";
  const panelTitle = panelMode === "create" ? "Yeni Yarışma" : "Detayları Düzenle";
  const saveLabel = panelMode === "create" ? "Kaydet" : "Değişiklikleri Kaydet";

  async function handleSave() {
    if (!form.name.trim() || !form.country.trim()) {
      toast.error("Yarışma adı ve ülke zorunludur.");
      return;
    }

    const result = await dispatch(saveCompetition());

    if (saveCompetition.fulfilled.match(result)) {
      void dispatch(loadCompetitions());
    }
  }

  return (
    <div className="relative mx-auto flex h-[calc(100vh-6rem)] w-full max-w-[1440px] flex-col md:h-[calc(100vh-7rem)]">
      <div className="pointer-events-none absolute inset-0 -z-10 opacity-20 mix-blend-screen">
        <div className="h-full w-full bg-[radial-gradient(ellipse_at_top_right,rgba(11,43,36,0.4),transparent_55%)]" />
      </div>

      <div className="mb-8 flex shrink-0 flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <span className="mb-2 block font-label text-[14px] font-semibold tracking-wider text-tertiary uppercase">
            Katalog Yönetimi
          </span>
          <h1 className="font-display text-[36px] leading-tight font-bold tracking-tight text-on-surface md:text-[48px] md:leading-[56px]">
            Yarışmalar
          </h1>
        </div>
        <button
          type="button"
          className="inline-flex items-center gap-2 rounded bg-secondary px-6 py-3 font-label text-[14px] font-bold tracking-[0.05em] text-on-secondary shadow-[0_0_15px_rgba(233,195,73,0.1)] transition-all hover:bg-secondary-fixed"
          onClick={() => dispatch(openCreatePanel())}
        >
          <span
            className="material-symbols-outlined text-[20px]"
            style={{ fontVariationSettings: "'FILL' 1", fontWeight: 700 }}
          >
            add
          </span>
          Yeni yarışma
        </button>
      </div>

      <div className="flex min-h-0 flex-1 flex-col gap-6 overflow-hidden xl:flex-row">
        <div className="group relative flex min-h-0 flex-1 flex-col overflow-hidden rounded-lg border border-outline-variant/20 bg-surface-container/50 backdrop-blur-sm">
          <div className="hidden shrink-0 grid-cols-12 items-center gap-4 border-b border-outline-variant/20 bg-surface-container-high/80 px-6 py-3 md:grid">
            <div className="col-span-4 font-display text-[11px] font-bold tracking-widest text-tertiary/80 uppercase">
              Yarışma Adı
            </div>
            <div className="col-span-3 font-display text-[11px] font-bold tracking-widest text-tertiary/80 uppercase">
              Ülke / Bölge
            </div>
            <div className="col-span-2 font-display text-[11px] font-bold tracking-widest text-tertiary/80 uppercase">
              Tür
            </div>
            <div className="col-span-2 font-display text-[11px] font-bold tracking-widest text-tertiary/80 uppercase">
              Durum
            </div>
            <div className="col-span-1 text-right font-display text-[11px] font-bold tracking-widest text-tertiary/80 uppercase">
              Eylem
            </div>
          </div>

          <div className="flex-1 overflow-y-auto">
            {isLoading && (
              <p className="px-6 py-10 text-center text-on-surface-variant">Yükleniyor…</p>
            )}

            {!isLoading && items.length === 0 && (
              <p className="px-6 py-10 text-center text-on-surface-variant">
                Henüz yarışma yok. Yeni yarışma ekleyerek başlayın.
              </p>
            )}

            {items.map((competition) => {
              const isSelected = selectedId === competition.id && panelMode === "edit";
              const typeIcon = COMPETITION_TYPE_ICON[competition.type];
              const typeLabel = COMPETITION_TYPE_LABEL[competition.type];

              return (
                <div
                  key={competition.id}
                  role="button"
                  tabIndex={0}
                  className={[
                    "grid cursor-pointer grid-cols-1 items-center gap-3 border-b border-outline-variant/10 px-6 py-4 transition-colors hover:bg-surface-container-high/50 md:grid-cols-12 md:gap-4",
                    isSelected
                      ? "border-l-2 border-l-secondary bg-primary-container/10"
                      : "border-l-2 border-l-transparent",
                  ].join(" ")}
                  onClick={() => dispatch(selectCompetition(competition.id))}
                  onKeyDown={(event) => {
                    if (event.key === "Enter" || event.key === " ") {
                      event.preventDefault();
                      dispatch(selectCompetition(competition.id));
                    }
                  }}
                >
                  <div className="flex items-center gap-3 md:col-span-4">
                    <div
                      className={[
                        "flex h-8 w-8 items-center justify-center rounded border border-outline-variant/30 bg-surface-container",
                        isSelected ? "text-secondary" : "text-tertiary",
                      ].join(" ")}
                    >
                      {competition.logoUrl ? (
                        <img
                          src={competition.logoUrl}
                          alt=""
                          className="h-full w-full rounded object-cover"
                        />
                      ) : (
                        <span className="material-symbols-outlined text-[18px]">{typeIcon}</span>
                      )}
                    </div>
                    <span
                      className={[
                        "font-body text-base text-on-surface",
                        isSelected ? "font-semibold" : "",
                      ].join(" ")}
                    >
                      {competition.name}
                    </span>
                  </div>

                  <div className="md:col-span-3">
                    <span className="font-body text-base text-on-surface-variant">
                      {competition.country}
                    </span>
                  </div>

                  <div className="md:col-span-2">
                    <span className="rounded border border-outline-variant/20 bg-surface-container-high px-2 py-1 font-label text-[12px] font-medium text-on-surface">
                      {typeLabel}
                    </span>
                  </div>

                  <div className="md:col-span-2">
                    {competition.isActive ? (
                      <span className="inline-flex items-center gap-1.5 rounded-full border border-primary/20 bg-primary-container/50 px-2.5 py-1 font-label text-[12px] font-medium text-primary">
                        <span className="h-1.5 w-1.5 animate-pulse rounded-full bg-primary" />
                        Canlı
                      </span>
                    ) : (
                      <span className="inline-flex items-center gap-1.5 rounded-full border border-outline-variant/30 bg-surface-container-highest px-2.5 py-1 font-label text-[12px] font-medium text-tertiary">
                        <span className="h-1.5 w-1.5 rounded-full bg-tertiary" />
                        Beklemede
                      </span>
                    )}
                  </div>

                  <div className="text-left md:col-span-1 md:text-right">
                    <button
                      type="button"
                      className="text-tertiary transition-colors hover:text-secondary"
                      aria-label="Düzenle"
                      onClick={(event) => {
                        event.stopPropagation();
                        dispatch(openEditPanel(competition.id));
                      }}
                    >
                      <span className="material-symbols-outlined">edit</span>
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {isPanelOpen && (
          <aside className="relative flex w-full shrink-0 flex-col overflow-hidden rounded-lg border border-outline-variant/20 bg-[#101e1b] shadow-2xl shadow-background xl:w-[420px]">
            <div className="flex shrink-0 items-center justify-between border-b border-outline-variant/10 bg-[#04110e]/50 p-6">
              <div className="flex items-center gap-3">
                <span
                  className="material-symbols-outlined text-secondary"
                  style={{ fontVariationSettings: "'FILL' 1" }}
                >
                  edit_square
                </span>
                <h2 className="font-display text-2xl font-semibold tracking-[0.02em] text-on-surface">
                  {panelTitle}
                </h2>
              </div>
              <button
                type="button"
                className="rounded p-1 text-tertiary transition-colors hover:bg-surface-container-high hover:text-on-surface"
                aria-label="Paneli kapat"
                onClick={() => dispatch(closePanel())}
              >
                <span className="material-symbols-outlined">close</span>
              </button>
            </div>

            <div className="flex flex-1 flex-col gap-8 overflow-y-auto p-6">
              <div className="group flex flex-col gap-2">
                <label className="font-label text-[14px] font-semibold tracking-[0.05em] text-tertiary transition-colors group-focus-within:text-secondary">
                  Yarışma Adı
                </label>
                <input
                  type="text"
                  value={form.name}
                  onChange={(event) => dispatch(updateForm({ name: event.target.value }))}
                  className="w-full rounded border border-outline-variant/30 bg-[#081613] px-4 py-3 font-body text-base text-on-surface transition-all outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
                />
              </div>

              <div className="group flex flex-col gap-2">
                <label className="font-label text-[14px] font-semibold tracking-[0.05em] text-tertiary transition-colors group-focus-within:text-secondary">
                  Ülke / Bölge
                </label>
                <div className="relative">
                  <select
                    value={form.country}
                    onChange={(event) => dispatch(updateForm({ country: event.target.value }))}
                    className="w-full appearance-none rounded border border-outline-variant/30 bg-[#081613] px-4 py-3 font-body text-base text-on-surface transition-all outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
                  >
                    {countryOptions.map((country) => (
                      <option key={country} value={country}>
                        {country}
                      </option>
                    ))}
                  </select>
                  <span className="material-symbols-outlined pointer-events-none absolute top-1/2 right-3 -translate-y-1/2 text-tertiary">
                    expand_more
                  </span>
                </div>
              </div>

              <div className="flex flex-col gap-3">
                <label className="font-label text-[14px] font-semibold tracking-[0.05em] text-tertiary">
                  Yarışma Türü
                </label>
                <div className="flex rounded border border-outline-variant/20 bg-[#081613] p-1">
                  {TYPE_OPTIONS.map((type) => {
                    const isActive = form.type === type;

                    return (
                      <button
                        key={type}
                        type="button"
                        className={[
                          "flex-1 rounded py-2 font-label text-[14px] font-semibold tracking-[0.05em] transition-all",
                          isActive
                            ? "border border-secondary/50 bg-primary-container/30 text-secondary shadow-[0_0_10px_rgba(11,43,36,0.5)]"
                            : "border border-transparent text-tertiary hover:text-on-surface",
                        ].join(" ")}
                        onClick={() => dispatch(updateForm({ type }))}
                      >
                        {COMPETITION_TYPE_LABEL[type]}
                      </button>
                    );
                  })}
                </div>
                <p className="mt-1 font-label text-[12px] font-medium text-tertiary/60">
                  Puanlama sistemi türüne göre otomatik yapılandırılır.
                </p>
              </div>

              <hr className="my-2 border-outline-variant/10" />

              <div className="flex items-center justify-between py-2">
                <div className="flex flex-col">
                  <span className="font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface">
                    Aktif Durumu
                  </span>
                  <span className="font-label text-[12px] font-medium text-tertiary/80">
                    Uygulama genelinde görünürlük
                  </span>
                </div>
                <button
                  type="button"
                  role="switch"
                  aria-checked={form.isActive}
                  className={[
                    "relative flex h-6 w-12 items-center rounded-full px-1 transition-colors focus:ring-2 focus:ring-secondary focus:ring-offset-2 focus:ring-offset-[#081613] focus:outline-none",
                    form.isActive ? "bg-secondary" : "bg-outline-variant/40",
                  ].join(" ")}
                  onClick={() => dispatch(updateForm({ isActive: !form.isActive }))}
                >
                  <span
                    className={[
                      "h-4 w-4 rounded-full bg-on-secondary shadow-sm transition-transform",
                      form.isActive ? "translate-x-6" : "translate-x-0",
                    ].join(" ")}
                  />
                </button>
              </div>
            </div>

            <div className="flex shrink-0 gap-4 border-t border-outline-variant/10 bg-[#04110e]/80 p-6 backdrop-blur-sm">
              <button
                type="button"
                className="flex-1 rounded border border-outline-variant/40 py-3 font-label text-[14px] font-bold tracking-[0.05em] text-tertiary transition-all hover:border-tertiary hover:bg-surface-container-high hover:text-on-surface"
                onClick={() => dispatch(closePanel())}
                disabled={isSaving}
              >
                İptal Et
              </button>
              <button
                type="button"
                className="flex-1 rounded bg-secondary py-3 font-label text-[14px] font-bold tracking-[0.05em] text-on-secondary shadow-[0_0_15px_rgba(233,195,73,0.1)] transition-all hover:bg-secondary-fixed disabled:opacity-60"
                onClick={() => void handleSave()}
                disabled={isSaving || !form.name.trim()}
              >
                {isSaving ? "Kaydediliyor…" : saveLabel}
              </button>
            </div>
          </aside>
        )}
      </div>
    </div>
  );
}
