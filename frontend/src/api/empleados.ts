import { apiFetch } from "./client";
import type { EmpleadoFiltroDto, EmpleadoRequestDto, EmpleadoResponseDto, ReportePagoDto } from "./types";

export function listarEmpleados(): Promise<EmpleadoResponseDto[]> {
  return apiFetch<EmpleadoResponseDto[]>("/empleados");
}

export function filtrarEmpleados(filtro: EmpleadoFiltroDto): Promise<EmpleadoResponseDto[]> {
  const params = new URLSearchParams();
  if (filtro.nombre) params.set("nombre", filtro.nombre);
  if (filtro.departamento) params.set("departamento", filtro.departamento);
  if (filtro.activo !== undefined) params.set("activo", String(filtro.activo));

  const query = params.toString();
  return apiFetch<EmpleadoResponseDto[]>(`/empleados/filtrar${query ? `?${query}` : ""}`);
}

export function obtenerEmpleado(id: number): Promise<EmpleadoResponseDto> {
  return apiFetch<EmpleadoResponseDto>(`/empleados/${id}`);
}

export function crearEmpleado(dto: EmpleadoRequestDto): Promise<EmpleadoResponseDto> {
  return apiFetch<EmpleadoResponseDto>("/empleados", {
    method: "POST",
    body: JSON.stringify(dto),
  });
}

export function actualizarEmpleado(id: number, dto: EmpleadoRequestDto): Promise<EmpleadoResponseDto> {
  return apiFetch<EmpleadoResponseDto>(`/empleados/${id}`, {
    method: "PUT",
    body: JSON.stringify(dto),
  });
}

export function eliminarEmpleado(id: number): Promise<void> {
  return apiFetch<void>(`/empleados/${id}`, { method: "DELETE" });
}

export function reporteSemanal(): Promise<ReportePagoDto[]> {
  return apiFetch<ReportePagoDto[]>("/empleados/reporte-semanal");
}
