import { NavLink, Outlet, useLocation, useNavigate } from "react-router-dom";
import { clearAdminHome } from "../../features/admin/adminHomeSlice";
import { clearCompetitions } from "../../features/competitions/competitionsSlice";
import { clearSeasonStructure } from "../../features/competitionSeasons/seasonStructureSlice";
import { clearSeasonTeams } from "../../features/competitionTeams/seasonTeamsSlice";
import { clearAdminFixtures } from "../../features/matches/adminFixturesSlice";
import { clearAdminPredictions } from "../../features/matchPredictions/adminPredictionsSlice";
import { clearTeams } from "../../features/teams/teamsSlice";
import { logout } from "../../features/auth/authSlice";
import { useAppDispatch, useAppSelector } from "../store/hooks";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  [
    "flex items-center gap-3 border-l-4 px-4 py-3 font-label text-[14px] font-semibold tracking-[0.05em] transition-all duration-150",
    isActive
      ? "border-secondary bg-primary-container/20 text-secondary opacity-90"
      : "border-transparent text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface",
  ].join(" ");

function isFixturesPath(pathname: string): boolean {
  return (
    pathname === "/admin/fixtures" ||
    /\/admin\/seasons\/[^/]+\/fixtures$/.test(pathname) ||
    /\/admin\/matches\/[^/]+\/predictions$/.test(pathname)
  );
}

export function AdminLayout() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const user = useAppSelector((state) => state.auth.user);
  const fixturesActive = isFixturesPath(pathname);

  async function handleLogout() {
    await dispatch(logout());
    dispatch(clearAdminHome());
    dispatch(clearCompetitions());
    dispatch(clearSeasonStructure());
    dispatch(clearSeasonTeams());
    dispatch(clearAdminFixtures());
    dispatch(clearAdminPredictions());
    dispatch(clearTeams());
    navigate("/login", { replace: true });
  }

  return (
    <div className="flex min-h-screen bg-[#081613] text-on-surface selection:bg-secondary/30 selection:text-secondary">
      <aside
        className="fixed top-0 left-0 z-40 hidden h-full flex-col border-r border-outline-variant/10 bg-[#04110e] md:flex"
        style={{ width: "280px" }}
      >
        <div className="border-b border-outline-variant/10 p-6">
          <h1 className="font-display text-[32px] leading-10 font-bold text-on-surface">
            RightOnScore
          </h1>
          <p className="mt-1 font-label text-[12px] font-medium tracking-wider text-on-surface-variant uppercase">
            Admin Console
          </p>
        </div>

        <nav className="flex flex-1 flex-col gap-1 py-6">
          <NavLink to="/admin" end className={navLinkClass}>
            {({ isActive }) => (
              <>
                <span
                  className="material-symbols-outlined"
                  style={{
                    fontVariationSettings: isActive ? "'FILL' 1" : "'FILL' 0",
                  }}
                >
                  dashboard
                </span>
                Genel Bakış
              </>
            )}
          </NavLink>
          <NavLink to="/admin/competitions" className={navLinkClass}>
            <span className="material-symbols-outlined">emoji_events</span>
            Yarışmalar
          </NavLink>
          <NavLink to="/admin/teams" className={navLinkClass}>
            <span className="material-symbols-outlined">groups</span>
            Takımlar
          </NavLink>
          <NavLink
            to="/admin/fixtures"
            className={() => navLinkClass({ isActive: fixturesActive })}
          >
            <span
              className="material-symbols-outlined"
              style={{
                fontVariationSettings: fixturesActive ? "'FILL' 1" : "'FILL' 0",
              }}
            >
              sports_soccer
            </span>
            Maçlar
          </NavLink>
        </nav>

        <div className="border-t border-outline-variant/10 p-4">
          <button
            type="button"
            className="flex w-full items-center gap-3 rounded border-l-4 border-transparent px-4 py-3 font-label text-[14px] font-semibold tracking-[0.05em] text-on-surface-variant transition-colors hover:bg-surface-container-high hover:text-on-surface"
            onClick={handleLogout}
          >
            <span className="material-symbols-outlined">logout</span>
            Çıkış
          </button>
        </div>
      </aside>

      <header
        className="fixed top-0 z-30 flex h-16 items-center justify-between border-b border-outline-variant/10 bg-[#081613]/95 px-4 backdrop-blur-md md:px-6"
        style={{
          left: 0,
          width: "100%",
        }}
      >
        <div
          className="flex w-full items-center justify-between md:ml-[280px] md:w-[calc(100%-280px)]"
        >
          <div className="flex items-center gap-3">
            <span className="font-display text-xl font-bold text-on-surface md:hidden">
              RightOnScore
            </span>
            <div className="relative hidden w-64 md:block">
              <span className="material-symbols-outlined absolute top-1/2 left-3 -translate-y-1/2 text-sm text-on-surface-variant">
                search
              </span>
              <input
                type="search"
                placeholder="Hızlı Arama..."
                className="w-full rounded border border-outline-variant/30 bg-[#191c1d] py-1.5 pr-4 pl-10 font-label text-[12px] font-medium text-on-surface outline-none transition-colors focus:border-secondary focus:ring-1 focus:ring-secondary"
                disabled
                aria-label="Hızlı arama"
              />
            </div>
          </div>

          <div className="flex items-center gap-4">
            <button
              type="button"
              className="relative text-on-surface-variant transition-colors hover:text-secondary"
              aria-label="Bildirimler"
              disabled
            >
              <span className="material-symbols-outlined">notifications</span>
            </button>
            <button
              type="button"
              className="text-on-surface-variant transition-colors hover:text-secondary"
              aria-label="Ayarlar"
              disabled
            >
              <span className="material-symbols-outlined">settings</span>
            </button>
            <div className="ml-2 flex h-8 w-8 items-center justify-center overflow-hidden rounded border border-outline-variant/30 bg-surface-container-high">
              {user?.profileImageUrl ? (
                <img
                  src={user.profileImageUrl}
                  alt={user.username}
                  className="h-full w-full object-cover"
                />
              ) : (
                <span className="material-symbols-outlined text-sm text-on-surface-variant">
                  person
                </span>
              )}
            </div>
          </div>
        </div>
      </header>

      <nav className="fixed bottom-0 left-0 z-50 flex w-full items-center justify-around border-t border-outline-variant/20 bg-[#04110e] px-2 py-2 md:hidden">
        <NavLink
          to="/admin"
          end
          className={({ isActive }) =>
            [
              "flex flex-col items-center gap-0.5 px-2 py-1 text-[11px] font-medium",
              isActive ? "text-secondary" : "text-on-surface-variant",
            ].join(" ")
          }
        >
          <span className="material-symbols-outlined text-[22px]">dashboard</span>
          Özet
        </NavLink>
        <NavLink
          to="/admin/competitions"
          className={({ isActive }) =>
            [
              "flex flex-col items-center gap-0.5 px-2 py-1 text-[11px] font-medium",
              isActive ? "text-secondary" : "text-on-surface-variant",
            ].join(" ")
          }
        >
          <span className="material-symbols-outlined text-[22px]">emoji_events</span>
          Yarışma
        </NavLink>
        <NavLink
          to="/admin/teams"
          className={({ isActive }) =>
            [
              "flex flex-col items-center gap-0.5 px-2 py-1 text-[11px] font-medium",
              isActive ? "text-secondary" : "text-on-surface-variant",
            ].join(" ")
          }
        >
          <span className="material-symbols-outlined text-[22px]">groups</span>
          Takım
        </NavLink>
        <NavLink
          to="/admin/fixtures"
          className={() =>
            [
              "flex flex-col items-center gap-0.5 px-2 py-1 text-[11px] font-medium",
              fixturesActive ? "text-secondary" : "text-on-surface-variant",
            ].join(" ")
          }
        >
          <span className="material-symbols-outlined text-[22px]">sports_soccer</span>
          Maç
        </NavLink>
      </nav>

      <main className="min-h-screen w-full flex-1 px-4 pt-20 pb-24 md:ml-[280px] md:px-6 md:pt-24 md:pb-6">
        <Outlet />
      </main>
    </div>
  );
}
