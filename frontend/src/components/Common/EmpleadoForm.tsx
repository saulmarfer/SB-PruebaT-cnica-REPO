import { useEffect, useState, type FormEvent } from "react";
import type { EmpleadoRequestDto, EmpleadoResponseDto, TipoEmpleado } from "../../api/types";
import { ETIQUETAS_TIPO_EMPLEADO } from "../../api/types";

interface Props {
  valoresIniciales?: EmpleadoResponseDto;
  departamentos: string[];
  enviando: boolean;
  onGuardar: (dto: EmpleadoRequestDto) => void;
  onCancelar?: () => void;
  textoBotonGuardar?: string;
}

const vacio: EmpleadoRequestDto = {
  tipo: "Asalariado",
  primerNombre: "",
  apellidoPaterno: "",
  numeroSeguroSocial: "",
  departamento: "",
  activo: true,
};

/** Campos que aplican a cada tipo, tal como los define la Sección 3 del PDF. */
const CAMPOS_POR_TIPO: Record<TipoEmpleado, string[]> = {
  Asalariado: ["primerNombre", "apellidoPaterno", "numeroSeguroSocial", "salarioSemanal"],
  PorHoras: ["apellidoPaterno", "numeroSeguroSocial", "sueldoPorHora", "horasTrabajadas"],
  PorComision: ["primerNombre", "apellidoPaterno", "numeroSeguroSocial", "ventasBrutas", "tarifaComision"],
  AsalariadoPorComision: [
    "primerNombre",
    "apellidoPaterno",
    "numeroSeguroSocial",
    "ventasBrutas",
    "tarifaComision",
    "salarioBase",
  ],
};

export function EmpleadoForm({
  valoresIniciales,
  departamentos,
  enviando,
  onGuardar,
  onCancelar,
  textoBotonGuardar,
}: Props) {
  const [valores, setValores] = useState<EmpleadoRequestDto>(
    valoresIniciales
      ? {
          tipo: valoresIniciales.tipo,
          primerNombre: valoresIniciales.primerNombre ?? "",
          apellidoPaterno: valoresIniciales.apellidoPaterno,
          numeroSeguroSocial: valoresIniciales.numeroSeguroSocial,
          departamento: valoresIniciales.departamento ?? "",
          activo: valoresIniciales.activo,
          salarioSemanal: valoresIniciales.salarioSemanal ?? undefined,
          sueldoPorHora: valoresIniciales.sueldoPorHora ?? undefined,
          horasTrabajadas: valoresIniciales.horasTrabajadas ?? undefined,
          ventasBrutas: valoresIniciales.ventasBrutas ?? undefined,
          tarifaComision: valoresIniciales.tarifaComision ?? undefined,
          salarioBase: valoresIniciales.salarioBase ?? undefined,
        }
      : vacio
  );
  const [errores, setErrores] = useState<Record<string, string>>({});

  useEffect(() => {
    const campos = CAMPOS_POR_TIPO[valores.tipo];
    setValores((actual) => ({
      ...actual,
      salarioSemanal: campos.includes("salarioSemanal") ? actual.salarioSemanal : undefined,
      sueldoPorHora: campos.includes("sueldoPorHora") ? actual.sueldoPorHora : undefined,
      horasTrabajadas: campos.includes("horasTrabajadas") ? actual.horasTrabajadas : undefined,
      ventasBrutas: campos.includes("ventasBrutas") ? actual.ventasBrutas : undefined,
      tarifaComision: campos.includes("tarifaComision") ? actual.tarifaComision : undefined,
      salarioBase: campos.includes("salarioBase") ? actual.salarioBase : undefined,
    }));
  }, [valores.tipo]);

  function actualizar<K extends keyof EmpleadoRequestDto>(campo: K, valor: EmpleadoRequestDto[K]) {
    setValores((actual) => ({ ...actual, [campo]: valor }));
  }

  const campos = CAMPOS_POR_TIPO[valores.tipo];

  function validar(): boolean {
    const nuevosErrores: Record<string, string> = {};

    if (campos.includes("primerNombre") && !valores.primerNombre?.trim()) {
      nuevosErrores.primerNombre = "El primer nombre es obligatorio para este tipo de empleado.";
    }
    if (!valores.apellidoPaterno.trim()) nuevosErrores.apellidoPaterno = "El apellido paterno es obligatorio.";
    if (!valores.numeroSeguroSocial.trim()) nuevosErrores.numeroSeguroSocial = "El número de seguro social es obligatorio.";

    if (campos.includes("salarioSemanal") && !(Number(valores.salarioSemanal) > 0)) {
      nuevosErrores.salarioSemanal = "El salario semanal debe ser mayor a cero.";
    }
    if (campos.includes("sueldoPorHora") && !(Number(valores.sueldoPorHora) > 0)) {
      nuevosErrores.sueldoPorHora = "El sueldo por hora debe ser mayor a cero.";
    }
    if (campos.includes("horasTrabajadas") && !(Number(valores.horasTrabajadas) >= 0)) {
      nuevosErrores.horasTrabajadas = "Las horas trabajadas no pueden ser negativas.";
    }
    if (campos.includes("ventasBrutas") && !(Number(valores.ventasBrutas) >= 0)) {
      nuevosErrores.ventasBrutas = "Las ventas brutas no pueden ser negativas.";
    }
    if (campos.includes("tarifaComision") && !(Number(valores.tarifaComision) > 0)) {
      nuevosErrores.tarifaComision = "La tarifa de comisión debe ser mayor a cero.";
    }
    if (campos.includes("salarioBase") && !(Number(valores.salarioBase) > 0)) {
      nuevosErrores.salarioBase = "El salario base debe ser mayor a cero.";
    }

    setErrores(nuevosErrores);
    return Object.keys(nuevosErrores).length === 0;
  }

  function manejarEnvio(evento: FormEvent) {
    evento.preventDefault();
    if (!validar()) return;
    onGuardar(valores);
  }

  return (
    <form className="formulario" onSubmit={manejarEnvio} noValidate>
      <div className="formulario__grid">
        <label className="campo campo--ancho">
          <span>Tipo de empleado *</span>
          <select value={valores.tipo} onChange={(e) => actualizar("tipo", e.target.value as TipoEmpleado)}>
            {(Object.keys(ETIQUETAS_TIPO_EMPLEADO) as TipoEmpleado[]).map((tipo) => (
              <option key={tipo} value={tipo}>
                {ETIQUETAS_TIPO_EMPLEADO[tipo]}
              </option>
            ))}
          </select>
        </label>

        {campos.includes("primerNombre") && (
          <label className="campo">
            <span>Primer nombre *</span>
            <input
              type="text"
              value={valores.primerNombre}
              onChange={(e) => actualizar("primerNombre", e.target.value)}
              aria-invalid={Boolean(errores.primerNombre)}
            />
            {errores.primerNombre && <small className="campo__error">{errores.primerNombre}</small>}
          </label>
        )}

        <label className="campo">
          <span>Apellido paterno *</span>
          <input
            type="text"
            value={valores.apellidoPaterno}
            onChange={(e) => actualizar("apellidoPaterno", e.target.value)}
            aria-invalid={Boolean(errores.apellidoPaterno)}
          />
          {errores.apellidoPaterno && <small className="campo__error">{errores.apellidoPaterno}</small>}
        </label>

        <label className="campo">
          <span>Número de seguro social *</span>
          <input
            type="text"
            value={valores.numeroSeguroSocial}
            onChange={(e) => actualizar("numeroSeguroSocial", e.target.value)}
            placeholder="000-0000000-0"
            aria-invalid={Boolean(errores.numeroSeguroSocial)}
          />
          {errores.numeroSeguroSocial && <small className="campo__error">{errores.numeroSeguroSocial}</small>}
        </label>

        <label className="campo">
          <span>Departamento</span>
          <select value={valores.departamento} onChange={(e) => actualizar("departamento", e.target.value)}>
            <option value="">Sin asignar</option>
            {departamentos.map((d) => (
              <option key={d} value={d}>
                {d}
              </option>
            ))}
          </select>
        </label>

        {campos.includes("salarioSemanal") && (
          <label className="campo">
            <span>Salario semanal (RD$) *</span>
            <input
              type="number"
              min="0"
              step="0.01"
              value={valores.salarioSemanal ?? ""}
              onChange={(e) => actualizar("salarioSemanal", Number(e.target.value))}
              aria-invalid={Boolean(errores.salarioSemanal)}
            />
            {errores.salarioSemanal && <small className="campo__error">{errores.salarioSemanal}</small>}
          </label>
        )}

        {campos.includes("sueldoPorHora") && (
          <label className="campo">
            <span>Sueldo por hora (RD$) *</span>
            <input
              type="number"
              min="0"
              step="0.01"
              value={valores.sueldoPorHora ?? ""}
              onChange={(e) => actualizar("sueldoPorHora", Number(e.target.value))}
              aria-invalid={Boolean(errores.sueldoPorHora)}
            />
            {errores.sueldoPorHora && <small className="campo__error">{errores.sueldoPorHora}</small>}
          </label>
        )}

        {campos.includes("horasTrabajadas") && (
          <label className="campo">
            <span>Horas trabajadas esta semana *</span>
            <input
              type="number"
              min="0"
              step="0.5"
              value={valores.horasTrabajadas ?? ""}
              onChange={(e) => actualizar("horasTrabajadas", Number(e.target.value))}
              aria-invalid={Boolean(errores.horasTrabajadas)}
            />
            <small>Más de 40 horas se pagan a 1.5x automáticamente.</small>
            {errores.horasTrabajadas && <small className="campo__error">{errores.horasTrabajadas}</small>}
          </label>
        )}

        {campos.includes("ventasBrutas") && (
          <label className="campo">
            <span>Ventas brutas (RD$) *</span>
            <input
              type="number"
              min="0"
              step="0.01"
              value={valores.ventasBrutas ?? ""}
              onChange={(e) => actualizar("ventasBrutas", Number(e.target.value))}
              aria-invalid={Boolean(errores.ventasBrutas)}
            />
            {errores.ventasBrutas && <small className="campo__error">{errores.ventasBrutas}</small>}
          </label>
        )}

        {campos.includes("tarifaComision") && (
          <label className="campo">
            <span>Tarifa de comisión (ej. 0.05 = 5%) *</span>
            <input
              type="number"
              min="0"
              max="1"
              step="0.01"
              value={valores.tarifaComision ?? ""}
              onChange={(e) => actualizar("tarifaComision", Number(e.target.value))}
              aria-invalid={Boolean(errores.tarifaComision)}
            />
            {errores.tarifaComision && <small className="campo__error">{errores.tarifaComision}</small>}
          </label>
        )}

        {campos.includes("salarioBase") && (
          <label className="campo">
            <span>Salario base (RD$) *</span>
            <input
              type="number"
              min="0"
              step="0.01"
              value={valores.salarioBase ?? ""}
              onChange={(e) => actualizar("salarioBase", Number(e.target.value))}
              aria-invalid={Boolean(errores.salarioBase)}
            />
            <small>Recibe además un 10% adicional automático sobre este monto.</small>
            {errores.salarioBase && <small className="campo__error">{errores.salarioBase}</small>}
          </label>
        )}

        <label className="campo campo--switch">
          <input type="checkbox" checked={valores.activo} onChange={(e) => actualizar("activo", e.target.checked)} />
          <span>Empleado activo</span>
        </label>
      </div>

      <div className="formulario__acciones">
        {onCancelar && (
          <button type="button" className="boton boton--secundario" onClick={onCancelar} disabled={enviando}>
            Cancelar
          </button>
        )}
        <button type="submit" className="boton boton--primario" disabled={enviando}>
          {enviando ? "Guardando..." : textoBotonGuardar ?? "Guardar"}
        </button>
      </div>
    </form>
  );
}
