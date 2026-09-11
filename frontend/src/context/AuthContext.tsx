import { createContext, useContext, useMemo, useState, type ReactNode } from "react";
import { login as loginRequest } from "../api/auth";
import { cerrarSesion, guardarSesion, obtenerUsuarioSesion, obtenerToken } from "../api/client";
import type { RolUsuario } from "../api/types";

interface SesionUsuario {
  nombreUsuario: string;
  rol: RolUsuario;
}

interface AuthContextValue {
  usuario: SesionUsuario | null;
  estaAutenticado: boolean;
  esAdmin: boolean;
  cargando: boolean;
  error: string | null;
  iniciarSesion: (nombreUsuario: string, password: string) => Promise<void>;
  cerrarSesion: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<SesionUsuario | null>(() => {
    const sesion = obtenerUsuarioSesion();
    const token = obtenerToken();
    return sesion && token ? { nombreUsuario: sesion.nombreUsuario, rol: sesion.rol as RolUsuario } : null;
  });
  const [cargando, setCargando] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function iniciarSesion(nombreUsuario: string, password: string) {
    setCargando(true);
    setError(null);
    try {
      const respuesta = await loginRequest({ nombreUsuario, password });
      guardarSesion(respuesta.token, respuesta.nombreUsuario, respuesta.rol);
      setUsuario({ nombreUsuario: respuesta.nombreUsuario, rol: respuesta.rol });
    } catch (err) {
      const mensaje = err instanceof Error ? err.message : "No fue posible iniciar sesión.";
      setError(mensaje);
      throw err;
    } finally {
      setCargando(false);
    }
  }

  function cerrar() {
    cerrarSesion();
    setUsuario(null);
  }

  const value = useMemo<AuthContextValue>(
    () => ({
      usuario,
      estaAutenticado: usuario !== null,
      esAdmin: usuario?.rol === "Admin",
      cargando,
      error,
      iniciarSesion,
      cerrarSesion: cerrar,
    }),
    [usuario, cargando, error]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth debe usarse dentro de <AuthProvider>");
  return context;
}
