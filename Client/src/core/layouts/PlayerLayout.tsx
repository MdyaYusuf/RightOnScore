import type { ReactNode } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { logout } from "../../features/auth/authSlice";
import { clearSeasonHub } from "../../features/competitionSeasons/seasonHubSlice";
import { clearFixtures } from "../../features/matches/fixturesSlice";
import { clearMyPredictions } from "../../features/matchPredictions/myPredictionsSlice";
import { clearLeaderboard } from "../../features/seasonStandings/leaderboardSlice";
import { useAppDispatch, useAppSelector } from "../store/hooks";

const sideNavLinkClass = ({ isActive }: { isActive: boolean }) =>
  [
    "mx-2 flex items-center gap-2 rounded-lg p-3 transition-all duration-300 ease-in-out",
    isActive
      ? "bg-secondary-container font-bold text-on-secondary-container"
      : "text-on-surface-variant hover:bg-surface-container-high hover:text-primary",
  ].join(" ");

const bottomNavLinkClass = ({ isActive }: { isActive: boolean }) =>
  [
    "flex w-16 flex-col items-center justify-center",
    isActive ? "text-secondary" : "text-on-surface-variant hover:text-on-surface",
  ].join(" ");

type PlayerLayoutProps = {
  children: ReactNode;
};

export function PlayerLayout({ children }: PlayerLayoutProps) {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const user = useAppSelector((state) => state.auth.user);
  const myStanding = useAppSelector((state) => state.seasonHub.myStanding);

  const rankLabel = myStanding ? `#${myStanding.rank}` : "—";
  const pointsLabel = myStanding ? `${myStanding.totalPoints} puan` : "0 puan";

  async function handleLogout() {
    await dispatch(logout());
    dispatch(clearSeasonHub());
    dispatch(clearFixtures());
    dispatch(clearLeaderboard());
    dispatch(clearMyPredictions());
    navigate("/login", { replace: true });
  }

  return (
    <div className="flex min-h-screen flex-col overflow-x-hidden bg-[#05120F] text-on-background md:flex-row">
      <div
        className="pointer-events-none fixed inset-0 opacity-[0.05]"
        style={{
          backgroundImage:
            "url(\"data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)' opacity='1'/%3E%3C/svg%3E\")",
        }}
      />

      <header className="fixed top-0 z-50 flex h-16 w-full items-center justify-between border-b border-outline-variant/20 bg-surface-container-lowest px-margin-desktop md:hidden">
        <div className="font-display text-[24px] font-bold tracking-tight text-secondary">
          RightOnScore
        </div>
        <div className="flex items-center gap-2">
          <span className="material-symbols-outlined cursor-default text-on-surface-variant">
            account_circle
          </span>
        </div>
      </header>

      <nav className="fixed top-0 left-0 z-40 hidden h-full w-64 flex-col border-r border-outline-variant/10 bg-primary-container/30 py-6 shadow-xl md:flex">
        <div className="mb-8 px-6">
          <div className="mb-6 font-display text-[32px] font-bold tracking-tight text-secondary">
            RightOnScore
          </div>

          <div className="mb-4 flex items-center gap-4">
            {user?.profileImageUrl ? (
              <img
                src={user.profileImageUrl}
                alt={user.username}
                className="h-12 w-12 rounded-full border border-outline-variant/20 object-cover"
              />
            ) : (
              <div className="flex h-12 w-12 items-center justify-center rounded-full border border-outline-variant/20 bg-surface-container-lowest">
                <span className="material-symbols-outlined text-primary">person</span>
              </div>
            )}
            <div>
              <h3 className="text-lg font-semibold text-on-surface">Puanım</h3>
              <p className="text-sm text-on-surface-variant">
                Sıra: {rankLabel} • {pointsLabel}
              </p>
            </div>
          </div>

          <NavLink
            to="/fixtures"
            className="block w-full rounded bg-secondary py-2 text-center font-bold text-black transition-colors hover:bg-secondary-fixed"
          >
            Yeni Tahmin
          </NavLink>
        </div>

        <div className="flex-1 overflow-y-auto">
          <ul className="space-y-2">
            <li>
              <NavLink to="/fixtures" className={sideNavLinkClass}>
                <span className="material-symbols-outlined">sports_soccer</span>
                Maçlar
              </NavLink>
            </li>
            <li>
              <NavLink to="/predictions" className={sideNavLinkClass}>
                <span className="material-symbols-outlined">ads_click</span>
                Tahminlerim
              </NavLink>
            </li>
            <li>
              <NavLink to="/leaderboard" className={sideNavLinkClass}>
                <span className="material-symbols-outlined">leaderboard</span>
                Sıralama
              </NavLink>
            </li>
            <li>
              <NavLink to="/seasons" className={sideNavLinkClass}>
                <span className="material-symbols-outlined">calendar_today</span>
                Sezonlar
              </NavLink>
            </li>
          </ul>
        </div>

        <div className="mt-auto px-2">
          <ul className="space-y-2 border-t border-outline-variant/20 pt-4">
            <li>
              <button
                type="button"
                className="mx-2 flex w-[calc(100%-1rem)] items-center gap-2 rounded-lg p-3 text-on-surface-variant transition-all duration-300 hover:bg-surface-container-high hover:text-primary"
                onClick={handleLogout}
              >
                <span className="material-symbols-outlined">logout</span>
                Çıkış
              </button>
            </li>
          </ul>
        </div>
      </nav>

      <main className="relative z-10 w-full flex-1 px-margin-mobile pt-20 pb-24 md:ml-64 md:px-margin-desktop md:pt-8 md:pb-8">
        {children}
      </main>

      <nav className="fixed bottom-0 left-0 z-50 flex w-full items-center justify-around rounded-t-[0.75rem] border-t border-white/5 bg-surface-container-highest px-4 py-3 shadow-[0_-4px_10px_rgba(0,0,0,0.3)] md:hidden">
        <NavLink to="/fixtures" className={bottomNavLinkClass}>
          <span className="material-symbols-outlined mb-1">stadium</span>
          <span className="font-label text-[12px] font-medium tracking-[0.05em] uppercase">
            Maçlar
          </span>
        </NavLink>
        <NavLink to="/predictions" className={bottomNavLinkClass}>
          <span className="material-symbols-outlined mb-1">edit_square</span>
          <span className="font-label text-[12px] font-medium tracking-[0.05em] uppercase">
            Tahmin
          </span>
        </NavLink>
        <NavLink to="/leaderboard" className={bottomNavLinkClass}>
          <span className="material-symbols-outlined mb-1">military_tech</span>
          <span className="font-label text-[12px] font-medium tracking-[0.05em] uppercase">
            Sıra
          </span>
        </NavLink>
        <NavLink to="/seasons" className={bottomNavLinkClass}>
          <span className="material-symbols-outlined mb-1">calendar_today</span>
          <span className="font-label text-[12px] font-medium tracking-[0.05em] uppercase">
            Sezon
          </span>
        </NavLink>
      </nav>
    </div>
  );
}
