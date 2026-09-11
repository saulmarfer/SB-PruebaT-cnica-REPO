import type { ApiErrorPayload } from "./types";

export const API_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7099/api";

const TOKEN_KEY = "sb_token";
const USER_KEY = "sb_usuario";

export function guardarSesion(token: string, nombreUsuario: string, rol: string) {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify({ nombreUsuario, rol }));
}

export function obtenerToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function obtenerUsuarioSesion(): { nombreUsuario: string; rol: string } | null {
  const raw = localStorage.getItem(USER_KEY);
  return raw ? JSON.parse(raw) : null;
}

export function cerrarSesion() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

export class ApiError extends Error {
  codigo: number;

  constructor(message: string, codigo: number) {
    super(message);
    this.name = "ApiError";
    this.codigo = codigo;
  }
}

interface Opciones extends RequestInit {
  sinAuth?: boolean;
}

/**
 * Agrega el Authorization Bearer, serializa JSON y traduce el formato de error uniforme 
 * en la API ({ exitoso, mensaje, codigo }) a un ApiError legible en la interfaz.
 */
export async function apiFetch<T>(ruta: string, opciones: Opciones = {}): Promise<T> {
  const { sinAuth, headers, ...resto } = opciones;
  const token = obtenerToken();

  const respuesta = await fetch(`${API_URL}${ruta}`, {
    ...resto,
    headers: {
      "Content-Type": "application/json",
      ...(token && !sinAuth ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
  });

  if (respuesta.status === 401) {
    cerrarSesion();
    window.location.href = "/login";
    throw new ApiError("Tu sesión expiró. Inicia sesión nuevamente.", 401);
  }

  if (!respuesta.ok) {
    let mensaje = `Error inesperado (HTTP ${respuesta.status}).`;
    try {
      const payload = (await respuesta.json()) as ApiErrorPayload;
      if (payload?.mensaje) mensaje = payload.mensaje;
    } catch {
    }
    throw new ApiError(mensaje, respuesta.status);
  }

  if (respuesta.status === 204) return undefined as T;

  return (await respuesta.json()) as T;
}
