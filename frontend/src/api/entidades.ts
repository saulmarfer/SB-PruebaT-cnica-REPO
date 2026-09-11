import { apiFetch } from "./client";
import type { EntidadGubernamental, EntidadGubernamentalInput } from "./types";

export function listarEntidades(termino?: string): Promise<EntidadGubernamental[]> {
  const query = termino ? `?termino=${encodeURIComponent(termino)}` : "";
  return apiFetch<EntidadGubernamental[]>(`/entidadesgubernamentales${query}`);
}

export function obtenerEntidad(id: number): Promise<EntidadGubernamental> {
  return apiFetch<EntidadGubernamental>(`/entidadesgubernamentales/${id}`);
}

export function crearEntidad(dto: EntidadGubernamentalInput): Promise<EntidadGubernamental> {
  return apiFetch<EntidadGubernamental>("/entidadesgubernamentales", {
    method: "POST",
    body: JSON.stringify(dto),
  });
}

export function actualizarEntidad(
  id: number,
  dto: EntidadGubernamentalInput
): Promise<EntidadGubernamental> {
  return apiFetch<EntidadGubernamental>(`/entidadesgubernamentales/${id}`, {
    method: "PUT",
    body: JSON.stringify(dto),
  });
}

export function eliminarEntidad(id: number): Promise<void> {
  return apiFetch<void>(`/entidadesgubernamentales/${id}`, { method: "DELETE" });
}
