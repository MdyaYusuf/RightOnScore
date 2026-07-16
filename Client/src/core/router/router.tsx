import { createBrowserRouter } from "react-router-dom";
import { AdminHomePage } from "../../features/admin/pages/AdminHomePage";
import { LoginPage } from "../../features/auth/pages/LoginPage";
import { RegisterPage } from "../../features/auth/pages/RegisterPage";
import { SeasonHubPage } from "../../features/competitionSeasons/pages/SeasonHubPage";
import { FixturesPage } from "../../features/matches/pages/FixturesPage";
import { MyPredictionsPage } from "../../features/matchPredictions/pages/MyPredictionsPage";
import { LeaderboardPage } from "../../features/seasonStandings/pages/LeaderboardPage";
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
            ],
          },
        ],
      },
    ],
  },
]);
