import { Navigate, Outlet } from "react-router-dom";
import { useAppSelector } from "../store/hooks";

/** Redirects authenticated users away from guest-only pages (landing, login, register). */
export function GuestRoute() {
  const status = useAppSelector((state) => state.auth.status);

  if (status === "authenticated") {
    return <Navigate to="/seasons" replace />;
  }

  return <Outlet />;
}
