export type RolUsuario = "Admin" | "Usuario";

export interface LoginRequest {
  nombreUsuario: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  nombreUsuario: string;
  rol: RolUsuario;
  expiraEn: string;
}

export interface EntidadGubernamental {
  id: number;
  nombre: string;
  siglas: string;
  sector: string;
  direccion?: string | null;
  telefono?: string | null;
  sitioWeb?: string | null;
  activo: boolean;
}

export interface EntidadGubernamentalInput {
  nombre: string;
  siglas: string;
  sector: string;
  direccion?: string;
  telefono?: string;
  sitioWeb?: string;
  activo: boolean;
}

// ---- Empleados ----

export type TipoEmpleado = "Asalariado" | "PorHoras" | "PorComision" | "AsalariadoPorComision";

export const ETIQUETAS_TIPO_EMPLEADO: Record<TipoEmpleado, string> = {
  Asalariado: "Asalariado",
  PorHoras: "Por horas",
  PorComision: "Por comisión",
  AsalariadoPorComision: "Asalariado por comisión",
};

export interface EmpleadoResponseDto {
  id: number;
  tipo: TipoEmpleado;
  tipoDescripcion: string;
  primerNombre?: string | null;
  apellidoPaterno: string;
  numeroSeguroSocial: string;
  departamento?: string | null;
  activo: boolean;

  salarioSemanal?: number | null;
  sueldoPorHora?: number | null;
  horasTrabajadas?: number | null;
  ventasBrutas?: number | null;
  tarifaComision?: number | null;
  salarioBase?: number | null;

  pagoSemanal: number;
}

export interface EmpleadoRequestDto {
  tipo: TipoEmpleado;
  primerNombre?: string;
  apellidoPaterno: string;
  numeroSeguroSocial: string;
  departamento?: string;
  activo: boolean;

  salarioSemanal?: number;
  sueldoPorHora?: number;
  horasTrabajadas?: number;
  ventasBrutas?: number;
  tarifaComision?: number;
  salarioBase?: number;
}

export interface EmpleadoFiltroDto {
  nombre?: string;
  departamento?: string;
  activo?: boolean;
}

export interface ReportePagoDto {
  empleadoId: number;
  nombreCompleto: string;
  tipoDescripcion: string;
  detalleCalculo: string;
  pagoSemanal: number;
}

export interface ApiErrorPayload {
  exitoso: false;
  mensaje: string;
  codigo: number;
}
