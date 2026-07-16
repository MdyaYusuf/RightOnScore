import { Navigate, Outlet } from "react-router-dom";
import { PlayerLayout } from "../layouts/PlayerLayout";

export function PlayerAppLayout() {
  return (
    <PlayerLayout>
      <Outlet />
    </PlayerLayout>
  );
}

export function RedirectToSeasons() {
  return <Navigate to="/seasons" replace />;
}
