import { createBrowserRouter } from "react-router-dom";
import { AdminHomePage } from "../../features/admin/pages/AdminHomePage";
import { LoginPage } from "../../features/auth/pages/LoginPage";
import { RegisterPage } from "../../features/auth/pages/RegisterPage";
import { SeasonHubPage } from "../../features/competitionSeasons/pages/SeasonHubPage";
import { AdminLayout } from "../layouts/AdminLayout";
import { AppLayout } from "../layouts/AppLayout";
import { PlayerAppLayout, RedirectToSeasons } from "../layouts/PlayerAppLayout";
import { RootLayout } from "../layouts/RootLayout";
import { AdminRoute } from "./AdminRoute";
import { GuestRoute } from "./GuestRoute";
import { ProtectedRoute } from "./ProtectedRoute";

function PlaceholderPage({ title }: { title: string }) {
  return (
    <div className="mx-auto max-w-[600px]">
      <h1 className="font-display text-[32px] font-bold text-on-surface">{title}</h1>
      <p className="mt-2 text-on-surface-variant">Bu ekran yakında eklenecek.</p>
    </div>
  );
}

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
                    element: <PlaceholderPage title="Maçlar" />,
                  },
                  {
                    path: "predictions",
                    element: <PlaceholderPage title="Tahminlerim" />,
                  },
                  {
                    path: "leaderboard",
                    element: <PlaceholderPage title="Sıralama" />,
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
