import { useState, type FormEvent } from "react";
import type { EntidadGubernamental, EntidadGubernamentalInput } from "../../api/types";

const SECTORES = [
  "Bancos Múltiples",
  "Bancos de Ahorro y Crédito",
  "Asociaciones de Ahorros y Préstamos",
  "Corporaciones de Crédito",
  "Entidades Fiduciarias",
  "Entidades Públicas de Intermediación Financiera",
  "Agentes de Cambio",
  "Agentes de Remesas y Cambio",
  "Otro",
];

interface Props {
  valoresIniciales?: EntidadGubernamental;
  enviando: boolean;
  onGuardar: (dto: EntidadGubernamentalInput) => void;
  onCancelar?: () => void;
  textoBotonGuardar?: string;
}

const vacio: EntidadGubernamentalInput = {
  nombre: "",
  siglas: "",
  sector: SECTORES[0],
  direccion: "",
  telefono: "",
  sitioWeb: "",
  activo: true,
};

export function EntidadForm({ valoresIniciales, enviando, onGuardar, onCancelar, textoBotonGuardar }: Props) {
  const [valores, setValores] = useState<EntidadGubernamentalInput>(
    valoresIniciales
      ? {
          nombre: valoresIniciales.nombre,
          siglas: valoresIniciales.siglas,
          sector: valoresIniciales.sector,
          direccion: valoresIniciales.direccion ?? "",
          telefono: valoresIniciales.telefono ?? "",
          sitioWeb: valoresIniciales.sitioWeb ?? "",
          activo: valoresIniciales.activo,
        }
      : vacio
  );
  const [errores, setErrores] = useState<Record<string, string>>({});

  function actualizar<K extends keyof EntidadGubernamentalInput>(campo: K, valor: EntidadGubernamentalInput[K]) {
    setValores((actual) => ({ ...actual, [campo]: valor }));
  }

  function validar(): boolean {
    const nuevosErrores: Record<string, string> = {};
    if (!valores.nombre.trim()) nuevosErrores.nombre = "El nombre es obligatorio.";
    if (!valores.siglas.trim()) nuevosErrores.siglas = "Las siglas son obligatorias.";
    if (!valores.sector.trim()) nuevosErrores.sector = "Selecciona un sector.";
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
        <label className="campo">
          <span>Nombre de la entidad *</span>
          <input
            type="text"
            value={valores.nombre}
            onChange={(e) => actualizar("nombre", e.target.value)}
            placeholder="Ej. Banco de Reservas de la República Dominicana"
            aria-invalid={Boolean(errores.nombre)}
          />
          {errores.nombre && <small className="campo__error">{errores.nombre}</small>}
        </label>

        <label className="campo">
          <span>Siglas *</span>
          <input
            type="text"
            value={valores.siglas}
            onChange={(e) => actualizar("siglas", e.target.value.toUpperCase())}
            placeholder="Ej. BANRESERVAS"
            aria-invalid={Boolean(errores.siglas)}
          />
          {errores.siglas && <small className="campo__error">{errores.siglas}</small>}
        </label>

        <label className="campo">
          <span>Sector *</span>
          <select value={valores.sector} onChange={(e) => actualizar("sector", e.target.value)}>
            {SECTORES.map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>
        </label>

        <label className="campo">
          <span>Teléfono</span>
          <input
            type="text"
            value={valores.telefono}
            onChange={(e) => actualizar("telefono", e.target.value)}
            placeholder="(809) 000-0000"
          />
        </label>

        <label className="campo campo--ancho">
          <span>Dirección</span>
          <input
            type="text"
            value={valores.direccion}
            onChange={(e) => actualizar("direccion", e.target.value)}
            placeholder="Ej. Av. Abraham Lincoln, Santo Domingo"
          />
        </label>

        <label className="campo campo--ancho">
          <span>Sitio web</span>
          <input
            type="url"
            value={valores.sitioWeb}
            onChange={(e) => actualizar("sitioWeb", e.target.value)}
            placeholder="https://..."
          />
        </label>

        <label className="campo campo--switch">
          <input
            type="checkbox"
            checked={valores.activo}
            onChange={(e) => actualizar("activo", e.target.checked)}
          />
          <span>Entidad activa</span>
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
