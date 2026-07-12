import { Navigate, Outlet } from "react-router-dom";
import { useAppSelector } from "../store/hooks";

export function ProtectedRoute() {
  const status = useAppSelector((state) => state.auth.status);

  if (status !== "authenticated") {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
