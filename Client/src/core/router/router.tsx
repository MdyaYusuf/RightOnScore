import { createBrowserRouter, Navigate } from "react-router-dom";
import { AdminHomePage } from "../../features/admin/pages/AdminHomePage";
import { LoginPage } from "../../features/auth/pages/LoginPage";
import { RegisterPage } from "../../features/auth/pages/RegisterPage";
import { CompetitionsPage } from "../../features/competitions/pages/CompetitionsPage";
import { SeasonStructurePage } from "../../features/competitionSeasons/pages/SeasonStructurePage";
import { SeasonHubPage } from "../../features/competitionSeasons/pages/SeasonHubPage";
import { AdminFixturesPage } from "../../features/matches/pages/AdminFixturesPage";
import { FixturesPage } from "../../features/matches/pages/FixturesPage";
import { AdminPredictionsPage } from "../../features/matchPredictions/pages/AdminPredictionsPage";
import { MyPredictionsPage } from "../../features/matchPredictions/pages/MyPredictionsPage";
import { LeaderboardPage } from "../../features/seasonStandings/pages/LeaderboardPage";
import { TeamsPage } from "../../features/teams/pages/TeamsPage";
import { SeasonTeamsPage } from "../../features/competitionTeams/pages/SeasonTeamsPage";
import { AdminLayout } from "../layouts/AdminLayout";
import { AppLayout } from "../layouts/AppLayout";
import { PlayerAppLayout, RedirectToSeasons } from "../layouts/PlayerAppLayout";
import { RootLayout } from "../layouts/RootLayout";
import { AdminRoute } from "./AdminRoute";
import { GuestRoute } from "./GuestRoute";
import { ProtectedRoute } from "./ProtectedRoute";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <RootLayout />,
    children: [
      {
        element: <AppLayout />,
        children: [
          {
            element: <GuestRoute />,
            children: [
              {
                path: "login",
                element: <LoginPage />,
              },
              {
                path: "register",
                element: <RegisterPage />,
              },
            ],
          },
          {
            element: <ProtectedRoute />,
            children: [
              {
                element: <PlayerAppLayout />,
                children: [
                  {
                    index: true,
                    element: <RedirectToSeasons />,
                  },
                  {
                    path: "seasons",
                    element: <SeasonHubPage />,
                  },
                  {
                    path: "fixtures",
                    element: <FixturesPage />,
                  },
                  {
                    path: "predictions",
                    element: <MyPredictionsPage />,
                  },
                  {
                    path: "leaderboard",
                    element: <LeaderboardPage />,
                  },
                ],
              },
            ],
          },
        ],
      },
      {
        path: "admin",
        element: <AdminRoute />,
        children: [
          {
            element: <AdminLayout />,
            children: [
              {
                index: true,
                element: <AdminHomePage />,
              },
              {
                path: "competitions",
                element: <CompetitionsPage />,
              },
              {
                path: "competitions/:competitionId/structure",
                element: <SeasonStructurePage />,
              },
              {
                path: "seasons/:seasonId/teams",
                element: <SeasonTeamsPage />,
              },
              {
                path: "seasons/:seasonId/fixtures",
                element: <AdminFixturesPage />,
              },
              {
                path: "teams",
                element: <TeamsPage />,
              },
              {
                path: "fixtures",
                element: <AdminFixturesPage />,
              },
              {
                path: "matches/:matchId/predictions",
                element: <AdminPredictionsPage />,
              },
              {
                path: "predictions",
                element: <Navigate to="/admin/fixtures" replace />,
              },
            ],
          },
        ],
      },
    ],
  },
]);
