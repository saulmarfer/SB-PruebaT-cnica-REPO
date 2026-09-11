import { useCallback, useEffect, useMemo, useState } from "react";
import {
  actualizarEmpleado,
  eliminarEmpleado,
  filtrarEmpleados,
  reporteSemanal,
} from "../api/empleados";
import { listarEntidades } from "../api/entidades";
import type { EmpleadoRequestDto, EmpleadoResponseDto, ReportePagoDto } from "../api/types";
import { ApiError } from "../api/client";
import { EmpleadoForm } from "../components/Common/EmpleadoForm";
import { Modal } from "../components/Common/Modal";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";

const formatoRD = new Intl.NumberFormat("es-DO", { style: "currency", currency: "DOP" });

export function ConsultaPage() {
  const { esAdmin } = useAuth();
  const { notificarExito, notificarError } = useToast();

  const [empleados, setEmpleados] = useState<EmpleadoResponseDto[]>([]);
  const [departamentos, setDepartamentos] = useState<string[]>([]);
  const [cargando, setCargando] = useState(true);

  // Los 3 filtros que exige la Sección 3 del PDF: nombre, departamento y estado.
  const [nombre, setNombre] = useState("");
  const [departamento, setDepartamento] = useState("");
  const [estado, setEstado] = useState<"todos" | "activos" | "inactivos">("todos");

  const [empleadoEditando, setEmpleadoEditando] = useState<EmpleadoResponseDto | null>(null);
  const [empleadoAEliminar, setEmpleadoAEliminar] = useState<EmpleadoResponseDto | null>(null);
  const [procesando, setProcesando] = useState(false);

  const [mostrandoReporte, setMostrandoReporte] = useState(false);
  const [reporte, setReporte] = useState<ReportePagoDto[] | null>(null);
  const [cargandoReporte, setCargandoReporte] = useState(false);

  const cargar = useCallback(async () => {
    setCargando(true);
    try {
      const datos = await filtrarEmpleados({
        nombre: nombre.trim() || undefined,
        departamento: departamento || undefined,
        activo: estado === "todos" ? undefined : estado === "activos",
      });
      setEmpleados(datos);
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible cargar los empleados.");
    } finally {
      setCargando(false);
    }
  }, [nombre, departamento, estado]);

  useEffect(() => {
    listarEntidades()
      .then((entidades) => setDepartamentos(entidades.filter((e) => e.activo).map((e) => e.nombre)))
      .catch(() => {
        /* si falla, el filtro de departamento simplemente queda sin opciones */
      });
  }, []);

  useEffect(() => {
    cargar();
  }, [cargar]);

  function manejarBusqueda(evento: React.FormEvent) {
    evento.preventDefault();
    cargar();
  }

  async function manejarActualizar(dto: EmpleadoRequestDto) {
    if (!empleadoEditando) return;
    setProcesando(true);
    try {
      const actualizado = await actualizarEmpleado(empleadoEditando.id, dto);
      setEmpleados((actuales) => actuales.map((e) => (e.id === actualizado.id ? actualizado : e)));
      notificarExito(`El pago de "${actualizado.apellidoPaterno}" se recalculó correctamente.`);
      setEmpleadoEditando(null);
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible actualizar el registro.");
    } finally {
      setProcesando(false);
    }
  }

  async function confirmarEliminacion() {
    if (!empleadoAEliminar) return;
    setProcesando(true);
    try {
      await eliminarEmpleado(empleadoAEliminar.id);
      setEmpleados((actuales) => actuales.filter((e) => e.id !== empleadoAEliminar.id));
      notificarExito("El empleado se eliminó del listado.");
      setEmpleadoAEliminar(null);
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible eliminar el registro.");
    } finally {
      setProcesando(false);
    }
  }

  async function abrirReporteSemanal() {
    setMostrandoReporte(true);
    setCargandoReporte(true);
    try {
      setReporte(await reporteSemanal());
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible generar el reporte.");
    } finally {
      setCargandoReporte(false);
    }
  }

  const totalReporte = useMemo(
    () => (reporte ? reporte.reduce((suma, r) => suma + r.pagoSemanal, 0) : 0),
    [reporte]
  );

  return (
    <div>
      <form className="barra-busqueda" onSubmit={manejarBusqueda}>
        <input
          type="search"
          placeholder="Buscar por nombre o apellido..."
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
        />
        <select value={departamento} onChange={(e) => setDepartamento(e.target.value)}>
          <option value="">Todos los departamentos</option>
          {departamentos.map((d) => (
            <option key={d} value={d}>
              {d}
            </option>
          ))}
        </select>
        <select value={estado} onChange={(e) => setEstado(e.target.value as typeof estado)}>
          <option value="todos">Todos los estados</option>
          <option value="activos">Solo activos</option>
          <option value="inactivos">Solo inactivos</option>
        </select>
        <button type="submit" className="boton boton--secundario">
          Buscar
        </button>
        <button type="button" className="boton boton--primario" onClick={abrirReporteSemanal}>
          Reporte semanal
        </button>
      </form>

      {cargando ? (
        <p className="mensaje-cargando">Cargando empleados…</p>
      ) : empleados.length === 0 ? (
        <p className="mensaje-vacio">No se encontraron empleados con ese criterio.</p>
      ) : (
        <div className="tabla-contenedor">
          <table className="tabla">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Tipo</th>
                <th>Departamento</th>
                <th>Pago semanal</th>
                <th>Estado</th>
                {esAdmin && <th aria-label="Acciones" />}
              </tr>
            </thead>
            <tbody>
              {empleados.map((empleado) => (
                <tr key={empleado.id}>
                  <td>{[empleado.primerNombre, empleado.apellidoPaterno].filter(Boolean).join(" ")}</td>
                  <td>{empleado.tipoDescripcion}</td>
                  <td>{empleado.departamento || "—"}</td>
                  <td>{formatoRD.format(empleado.pagoSemanal)}</td>
                  <td>
                    <span
                      className={`etiqueta-estado ${empleado.activo ? "etiqueta-estado--activo" : "etiqueta-estado--inactivo"}`}
                    >
                      {empleado.activo ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  {esAdmin && (
                    <td className="tabla__acciones">
                      <button type="button" className="enlace-accion" onClick={() => setEmpleadoEditando(empleado)}>
                        Editar
                      </button>
                      <button
                        type="button"
                        className="enlace-accion enlace-accion--peligro"
                        onClick={() => setEmpleadoAEliminar(empleado)}
                      >
                        Eliminar
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {empleadoEditando && (
        <Modal titulo="Editar empleado" onCerrar={() => setEmpleadoEditando(null)}>
          <EmpleadoForm
            valoresIniciales={empleadoEditando}
            departamentos={departamentos}
            enviando={procesando}
            onGuardar={manejarActualizar}
            onCancelar={() => setEmpleadoEditando(null)}
            textoBotonGuardar="Guardar cambios"
          />
        </Modal>
      )}

      {empleadoAEliminar && (
        <Modal titulo="Confirmar eliminación" onCerrar={() => setEmpleadoAEliminar(null)}>
          <p>
            ¿Seguro que deseas eliminar a{" "}
            <strong>{[empleadoAEliminar.primerNombre, empleadoAEliminar.apellidoPaterno].filter(Boolean).join(" ")}</strong>{" "}
            del listado? Esta acción no se puede deshacer.
          </p>
          <div className="formulario__acciones">
            <button
              type="button"
              className="boton boton--secundario"
              onClick={() => setEmpleadoAEliminar(null)}
              disabled={procesando}
            >
              Cancelar
            </button>
            <button type="button" className="boton boton--peligro" onClick={confirmarEliminacion} disabled={procesando}>
              {procesando ? "Eliminando..." : "Eliminar"}
            </button>
          </div>
        </Modal>
      )}

      {mostrandoReporte && (
        <Modal titulo="Reporte semanal de pagos" onCerrar={() => setMostrandoReporte(false)}>
          {cargandoReporte ? (
            <p className="mensaje-cargando">Calculando pagos…</p>
          ) : (
            <>
              <div className="tabla-contenedor">
                <table className="tabla">
                  <thead>
                    <tr>
                      <th>Empleado</th>
                      <th>Tipo</th>
                      <th>Cálculo</th>
                      <th>Pago</th>
                    </tr>
                  </thead>
                  <tbody>
                    {reporte?.map((fila) => (
                      <tr key={fila.empleadoId}>
                        <td>{fila.nombreCompleto}</td>
                        <td>{fila.tipoDescripcion}</td>
                        <td style={{ fontSize: "12.5px", color: "var(--texto-secundario)" }}>{fila.detalleCalculo}</td>
                        <td>{formatoRD.format(fila.pagoSemanal)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <p style={{ textAlign: "right", marginTop: 14, fontWeight: 700 }}>
                Total semana: {formatoRD.format(totalReporte)}
              </p>
            </>
          )}
        </Modal>
      )}
    </div>
  );
}
