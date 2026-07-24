import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

const HOW_IT_WORKS = [
  {
    icon: "calendar_today",
    title: "1. Sezonunu Seç",
    body: "Dünya Kupası, ligler veya kupalar arasından tercihini yap.",
  },
  {
    icon: "edit_square",
    title: "2. Skorları Tahmin Et",
    body: "Başlama vuruşundan önce tahminini yap ve sisteme kaydet.",
  },
  {
    icon: "add_circle",
    title: "3. Puanları Topla",
    body: "Tam skor, sonuç ve tur atlayan tahminleriyle puan kazan.",
  },
  {
    icon: "leaderboard",
    title: "4. Lider Ol",
    body: "Sezon sonunda rakiplerini geçerek zirveye yerleş.",
  },
] as const;

const SCORING_RULES = [
  { points: "3", title: "Tam Skor", detail: "Skoru tam bil" },
  { points: "1", title: "Doğru Sonuç", detail: "Kazananı doğru bil" },
  { points: "+1", title: "Tur Atlayan", detail: "Eleme maçlarında" },
  { points: "+2", title: "Seri Bonusu", detail: "Üst üste tam skorlar" },
] as const;

export function LandingPage() {
  const [headerSolid, setHeaderSolid] = useState(false);

  useEffect(() => {
    function onScroll() {
      setHeaderSolid(window.scrollY > 50);
    }

    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });

    return () => {
      window.removeEventListener("scroll", onScroll);
    };
  }, []);

  return (
    <div className="flex min-h-screen flex-col bg-background font-body text-on-background antialiased selection:bg-secondary selection:text-on-secondary">
      <header
        className={[
          "fixed top-0 z-50 flex h-16 w-full items-center justify-between px-4 transition-all duration-300 md:px-8",
          headerSolid
            ? "border-b border-outline-variant/20 bg-[#04110e] shadow-xl"
            : "border-b border-outline-variant/20 bg-[rgba(11,43,36,0.4)] backdrop-blur-[12px]",
        ].join(" ")}
      >
        <Link
          to="/"
          className="font-display text-2xl font-bold tracking-tight text-secondary md:text-[32px] md:leading-10"
        >
          RightOnScore
        </Link>
        <div className="flex gap-2 md:gap-4">
          <Link
            to="/login"
            className="rounded px-3 py-2 font-label text-[12px] font-semibold tracking-wide text-[#e1e3e4] border border-[#e1e3e4]/30 transition-colors hover:bg-surface-container-high md:px-6 md:text-[14px] md:leading-5"
          >
            Giriş Yap
          </Link>
          <Link
            to="/register"
            className="rounded bg-secondary px-3 py-2 font-label text-[12px] font-semibold tracking-wide text-primary-container transition-colors hover:bg-secondary-fixed md:px-6 md:text-[14px] md:leading-5"
          >
            Oyuna Başla
          </Link>
        </div>
      </header>

      <main className="flex-grow">
        <section className="relative flex min-h-screen items-center justify-center overflow-hidden pt-16">
          <div className="pointer-events-none absolute inset-0 z-0">
            <div className="absolute inset-0 z-10 bg-gradient-to-b from-background/40 via-background/60 to-background" />
            <img
              src="/images/landing-hero.png"
              alt=""
              className="h-full w-full object-cover object-center"
            />
            <div className="grain-overlay" />
          </div>

          <div className="relative z-20 mx-auto flex max-w-4xl flex-col items-center px-4 text-center md:px-8">
            <img
              src="/images/logo.png"
              alt="RightOnScore"
              className="mb-8 w-40 drop-shadow-2xl md:mb-8 md:w-64"
            />
            <h1 className="mb-4 font-display text-[36px] leading-tight font-bold text-on-background drop-shadow-lg md:text-[48px] md:leading-[56px]">
              Futbol Heyecanını Tahminlerinle Yaşa
            </h1>
            <p className="mb-8 max-w-2xl font-body text-[16px] leading-6 text-on-surface-variant md:text-[18px] md:leading-7">
              Arkadaşlarınla yarış, skorları doğru bil, liderlik koltuğuna otur.
            </p>
            <div className="flex w-full flex-col justify-center gap-4 sm:w-auto sm:flex-row">
              <Link
                to="/register"
                className="rounded-lg bg-secondary px-8 py-4 font-label text-[14px] font-semibold tracking-wide text-primary-container shadow-lg transition-all hover:bg-secondary-fixed hover:shadow-secondary/20"
              >
                Oyuna Başla
              </Link>
              <Link
                to="/login"
                className="rounded-lg border border-[#e1e3e4]/30 bg-[rgba(11,43,36,0.4)] px-8 py-4 font-label text-[14px] font-semibold tracking-wide text-[#e1e3e4] backdrop-blur-[12px] transition-all hover:bg-surface-container-high"
              >
                Giriş Yap
              </Link>
            </div>
          </div>
        </section>

        <section className="relative z-10 bg-[#04110e] py-16 md:py-24">
          <div className="mx-auto max-w-7xl px-4 md:px-8">
            <h2 className="mb-12 text-center font-display text-[28px] font-bold text-on-background md:mb-16 md:text-[32px] md:leading-10">
              Nasıl Çalışır?
            </h2>
            <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
              {HOW_IT_WORKS.map((step) => (
                <div
                  key={step.title}
                  className="group flex flex-col items-start rounded-xl border border-[#e1e3e4]/10 bg-[rgba(11,43,36,0.4)] p-8 backdrop-blur-[12px] transition-colors hover:bg-surface-container-high"
                >
                  <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-full border border-outline-variant/30 bg-primary-container text-secondary transition-colors group-hover:border-secondary/50">
                    <span
                      className="material-symbols-outlined"
                      style={{ fontVariationSettings: "'FILL' 1" }}
                    >
                      {step.icon}
                    </span>
                  </div>
                  <h3 className="mb-2 font-display text-[24px] leading-8 font-semibold tracking-wide text-on-surface">
                    {step.title}
                  </h3>
                  <p className="font-body text-[16px] leading-6 text-on-surface-variant">{step.body}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="relative z-10 border-t border-outline-variant/10 bg-background py-16 md:py-24">
          <div className="mx-auto max-w-7xl px-4 md:px-8">
            <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
              <div className="admin-surface-texture relative overflow-hidden rounded-xl border border-[#e1e3e4]/10 bg-[rgba(11,43,36,0.4)] p-8 backdrop-blur-[12px] md:p-10 lg:col-span-2">
                <div className="pointer-events-none absolute top-0 right-0 p-8 opacity-10">
                  <span className="material-symbols-outlined text-[9rem] text-secondary">
                    calculate
                  </span>
                </div>
                <h2 className="relative z-10 mb-8 font-display text-[28px] font-bold text-on-background md:text-[32px] md:leading-10">
                  Puanlama Sistemi
                </h2>
                <div className="relative z-10 grid grid-cols-1 gap-6 sm:grid-cols-2">
                  {SCORING_RULES.map((rule) => (
                    <div
                      key={rule.title}
                      className="flex items-center gap-4 rounded-lg border border-outline-variant/20 bg-surface-container/50 p-4"
                    >
                      <div className="font-display text-[24px] leading-8 font-semibold text-secondary">
                        {rule.points}
                      </div>
                      <div>
                        <div className="font-label text-[14px] font-semibold tracking-wide text-on-surface">
                          {rule.title}
                        </div>
                        <div className="font-body text-sm text-on-surface-variant">{rule.detail}</div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              <div className="relative flex flex-col justify-center overflow-hidden rounded-xl border border-[#e1e3e4]/10 bg-[rgba(11,43,36,0.4)] p-8 backdrop-blur-[12px] md:p-10 lg:col-span-1">
                <div className="pointer-events-none absolute inset-0 bg-primary-container/20 mix-blend-overlay" />
                <h2 className="relative z-10 mb-6 font-display text-[28px] font-bold text-on-background md:text-[32px] md:leading-10">
                  Neden RightOnScore?
                </h2>
                <p className="relative z-10 font-body text-[16px] leading-6 text-on-surface-variant md:text-[18px] md:leading-7">
                  Arkadaş grupları, ofis turnuvaları ve maç izleme partileri için mükemmel rekabet
                  ortamı. Teknik hassasiyet, gerçek zamanlı skorlar ve sıfır karmaşa.
                </p>
              </div>
            </div>
          </div>
        </section>
      </main>

      <footer className="border-t border-outline-variant/20 bg-[#04110e] py-12">
        <div className="mx-auto flex max-w-7xl flex-col items-center px-4 md:px-8">
          <h2 className="mb-8 text-center font-display text-[24px] leading-8 font-semibold tracking-wide text-on-background">
            Rekabete Katılmaya Hazır Mısın?
          </h2>
          <Link
            to="/register"
            className="mb-12 rounded-lg bg-secondary px-10 py-4 font-label text-[14px] font-semibold tracking-wide text-primary-container shadow-lg transition-all hover:bg-secondary-fixed hover:shadow-secondary/20"
          >
            Hemen Tahmin Yapmaya Başla
          </Link>
          <div className="mb-8 h-px w-full bg-outline-variant/20" />
          <div className="flex w-full flex-col items-center justify-between gap-4 md:flex-row">
            <div className="font-display text-2xl font-bold tracking-tight text-secondary opacity-50 md:text-[32px] md:leading-10">
              RightOnScore
            </div>
            <div className="flex gap-6 font-label text-[12px] font-medium text-on-surface-variant">
              <span>Gizlilik Politikası</span>
              <span>Kullanım Şartları</span>
              <span>İletişim</span>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}
