import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { AppLayout } from "./Layout/AppLayout";

export function ProtectedRoute({
  children,
  soloAdmin = false,
}: {
  children: ReactNode;
  soloAdmin?: boolean;
}) {
  const { estaAutenticado, esAdmin } = useAuth();

  if (!estaAutenticado) return <Navigate to="/login" replace />;
  if (soloAdmin && !esAdmin) return <Navigate to="/" replace />;

  return <AppLayout>{children}</AppLayout>;
}
