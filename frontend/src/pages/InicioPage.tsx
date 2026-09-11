import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { listarEmpleados } from "../api/empleados";
import { listarEntidades } from "../api/entidades";
import type { EmpleadoResponseDto } from "../api/types";
import { ETIQUETAS_TIPO_EMPLEADO } from "../api/types";
import { useAuth } from "../context/AuthContext";

const formatoRD = new Intl.NumberFormat("es-DO", { style: "currency", currency: "DOP" });

export function InicioPage() {
  const { usuario, esAdmin } = useAuth();
  const [empleados, setEmpleados] = useState<EmpleadoResponseDto[] | null>(null);
  const [totalDepartamentos, setTotalDepartamentos] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    listarEmpleados()
      .then(setEmpleados)
      .catch((err) => setError(err instanceof Error ? err.message : "No fue posible cargar el resumen."));

    listarEntidades()
      .then((entidades) => setTotalDepartamentos(entidades.filter((e) => e.activo).length))
      .catch(() => setTotalDepartamentos(null));
  }, []);

  const activos = empleados?.filter((e) => e.activo) ?? [];
  const nominaSemanal = activos.reduce((suma, e) => suma + e.pagoSemanal, 0);

  const resumenPorTipo = useMemo(() => {
    if (!empleados) return [];
    const mapa = new Map<string, number>();
    for (const e of empleados) mapa.set(e.tipoDescripcion, (mapa.get(e.tipoDescripcion) ?? 0) + 1);
    return Array.from(mapa.entries()).sort((a, b) => b[1] - a[1]);
  }, [empleados]);

  const maximoPorTipo = Math.max(1, ...resumenPorTipo.map(([, cantidad]) => cantidad));

  return (
    <div className="inicio">
      <p className="inicio__saludo">
        Bienvenido/a, <strong>{usuario?.nombreUsuario}</strong>. Este es el estado actual de la nómina y la
        gestión de empleados.
      </p>

      {error && <p className="mensaje-error">{error}</p>}

      <div className="tarjetas-resumen">
        <div className="tarjeta-resumen">
          <span className="tarjeta-resumen__numero">{empleados?.length ?? "…"}</span>
          <span className="tarjeta-resumen__etiqueta">Empleados registrados</span>
        </div>
        <div className="tarjeta-resumen tarjeta-resumen--activo">
          <span className="tarjeta-resumen__numero">{empleados ? formatoRD.format(nominaSemanal) : "…"}</span>
          <span className="tarjeta-resumen__etiqueta">Nómina semanal (activos)</span>
        </div>
        <div className="tarjeta-resumen">
          <span className="tarjeta-resumen__numero">{totalDepartamentos ?? "…"}</span>
          <span className="tarjeta-resumen__etiqueta">Departamentos disponibles</span>
        </div>
      </div>

      <div className="inicio__cuerpo">
        <section className="inicio__distribucion">
          <h2>Empleados por tipo</h2>
          <ul className="barra-lista">
            {resumenPorTipo.map(([tipo, cantidad]) => (
              <li key={tipo}>
                <div className="barra-lista__fila">
                  <span>{tipo}</span>
                  <span>{cantidad}</span>
                </div>
                <div className="barra-lista__pista">
                  <div className="barra-lista__relleno" style={{ width: `${(cantidad / maximoPorTipo) * 100}%` }} />
                </div>
              </li>
            ))}
            {resumenPorTipo.length === 0 && empleados && (
              <li className="mensaje-vacio" style={{ padding: 0 }}>
                Todavía no hay empleados registrados.
              </li>
            )}
          </ul>
          <p style={{ marginTop: 16, fontSize: 12.5, color: "var(--texto-secundario)" }}>
            Tipos soportados: {Object.values(ETIQUETAS_TIPO_EMPLEADO).join(", ")}.
          </p>
        </section>

        <section className="inicio__accesos">
          <h2>Accesos rápidos</h2>
          <Link to="/consulta" className="boton boton--secundario boton--ancho">
            Ir a Consulta
          </Link>
          {esAdmin && (
            <>
              <Link to="/crear-registro" className="boton boton--primario boton--ancho">
                Crear registro
              </Link>
              <Link to="/entidades" className="boton boton--secundario boton--ancho">
                Administrar departamentos
              </Link>
            </>
          )}
        </section>
      </div>
    </div>
  );
}
