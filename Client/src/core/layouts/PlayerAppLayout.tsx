import { Outlet } from "react-router-dom";
import { PlayerLayout } from "../layouts/PlayerLayout";

export function PlayerAppLayout() {
  return (
    <PlayerLayout>
      <Outlet />
    </PlayerLayout>
  );
}
