import { Navigate, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { ToastProvider } from "./context/ToastContext";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { LoginPage } from "./pages/LoginPage";
import { InicioPage } from "./pages/InicioPage";
import { ConsultaPage } from "./pages/ConsultaPage";
import { CrearRegistroPage } from "./pages/CrearRegistroPage";
import { EntidadesAdminPage } from "./pages/EntidadesAdminPage";

export default function App() {
  return (
    <AuthProvider>
      <ToastProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />

          <Route
            path="/"
            element={
              <ProtectedRoute>
                <InicioPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/consulta"
            element={
              <ProtectedRoute>
                <ConsultaPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/crear-registro"
            element={
              <ProtectedRoute soloAdmin>
                <CrearRegistroPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/entidades"
            element={
              <ProtectedRoute soloAdmin>
                <EntidadesAdminPage />
              </ProtectedRoute>
            }
          />

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </ToastProvider>
    </AuthProvider>
  );
}
