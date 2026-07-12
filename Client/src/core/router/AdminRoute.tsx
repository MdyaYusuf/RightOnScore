import { Navigate, Outlet } from "react-router-dom";
import { useAppSelector } from "../store/hooks";

export function AdminRoute() {
  const user = useAppSelector((state) => state.auth.user);
  const status = useAppSelector((state) => state.auth.status);

  if (status !== "authenticated") {
    return <Navigate to="/login" replace />;
  }

  if (user?.roleName !== "Admin") {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
