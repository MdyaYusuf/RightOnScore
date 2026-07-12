import { useEffect } from "react";
import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import { restoreSession } from "../../features/auth/authSlice";
import { useAppDispatch, useAppSelector } from "../store/hooks";

export function RootLayout() {
  const dispatch = useAppDispatch();
  const status = useAppSelector((state) => state.auth.status);

  useEffect(() => {
    void dispatch(restoreSession());
  }, [dispatch]);

  if (status === "idle" || status === "loading") {
    return null;
  }

  return (
    <>
      <Outlet />
      <ToastContainer position="top-right" autoClose={4000} newestOnTop closeOnClick />
    </>
  );
}
