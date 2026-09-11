import { useCallback, useEffect, useState } from "react";
import { crearEntidad, eliminarEntidad, actualizarEntidad, listarEntidades } from "../api/entidades";
import type { EntidadGubernamental, EntidadGubernamentalInput } from "../api/types";
import { ApiError } from "../api/client";
import { EntidadForm } from "../components/Common/EntidadForm";
import { Modal } from "../components/Common/Modal";
import { useToast } from "../context/ToastContext";

/**
 * Mantenimiento del catálogo de entidades gubernamentales (Instrucciones.txt).
 * No es una pantalla del flujo principal: existe para que un admin
 * mantenga actualizado el catálogo que alimenta el filtro/selector de
 * "Departamento" en el módulo de Empleados.
 */
export function EntidadesAdminPage() {
  const { notificarExito, notificarError } = useToast();

  const [entidades, setEntidades] = useState<EntidadGubernamental[]>([]);
  const [cargando, setCargando] = useState(true);
  const [termino, setTermino] = useState("");
  const [filtroEstado, setFiltroEstado] = useState<"todas" | "activas" | "inactivas">("todas");

  const [mostrandoFormulario, setMostrandoFormulario] = useState(false);
  const [entidadEditando, setEntidadEditando] = useState<EntidadGubernamental | null>(null);
  const [entidadAEliminar, setEntidadAEliminar] = useState<EntidadGubernamental | null>(null);
  const [procesando, setProcesando] = useState(false);

  const cargar = useCallback(async (busqueda?: string) => {
    setCargando(true);
    try {
      setEntidades(await listarEntidades(busqueda));
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible cargar las entidades.");
    } finally {
      setCargando(false);
    }
  }, []);

  useEffect(() => {
    cargar();
  }, [cargar]);

  function manejarBusqueda(evento: React.FormEvent) {
    evento.preventDefault();
    cargar(termino.trim() || undefined);
  }

  const entidadesFiltradas = entidades.filter((e) => {
    if (filtroEstado === "activas") return e.activo;
    if (filtroEstado === "inactivas") return !e.activo;
    return true;
  });

  async function manejarCrear(dto: EntidadGubernamentalInput) {
    setProcesando(true);
    try {
      const creada = await crearEntidad(dto);
      setEntidades((actuales) => [...actuales, creada]);
      notificarExito(`"${creada.nombre}" se agregó al catálogo.`);
      setMostrandoFormulario(false);
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible crear la entidad.");
    } finally {
      setProcesando(false);
    }
  }

  async function manejarActualizar(dto: EntidadGubernamentalInput) {
    if (!entidadEditando) return;
    setProcesando(true);
    try {
      const actualizada = await actualizarEntidad(entidadEditando.id, dto);
      setEntidades((actuales) => actuales.map((e) => (e.id === actualizada.id ? actualizada : e)));
      notificarExito(`"${actualizada.nombre}" se actualizó correctamente.`);
      setEntidadEditando(null);
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible actualizar el registro.");
    } finally {
      setProcesando(false);
    }
  }

  async function confirmarEliminacion() {
    if (!entidadAEliminar) return;
    setProcesando(true);
    try {
      await eliminarEntidad(entidadAEliminar.id);
      setEntidades((actuales) => actuales.filter((e) => e.id !== entidadAEliminar.id));
      notificarExito(`"${entidadAEliminar.nombre}" se eliminó del catálogo.`);
      setEntidadAEliminar(null);
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible eliminar el registro.");
    } finally {
      setProcesando(false);
    }
  }

  return (
    <div>
      <p className="descripcion-pagina">
        Este catálogo alimenta el selector de "Departamento" al crear, editar y filtrar empleados. Agrega o
        desactiva entidades aquí y los cambios se reflejan automáticamente en el módulo de Empleados.
      </p>

      <form className="barra-busqueda" onSubmit={manejarBusqueda}>
        <input
          type="search"
          placeholder="Buscar por nombre, siglas o sector..."
          value={termino}
          onChange={(e) => setTermino(e.target.value)}
        />
        <select value={filtroEstado} onChange={(e) => setFiltroEstado(e.target.value as typeof filtroEstado)}>
          <option value="todas">Todos los estados</option>
          <option value="activas">Solo activas</option>
          <option value="inactivas">Solo inactivas</option>
        </select>
        <button type="submit" className="boton boton--secundario">
          Buscar
        </button>
        <button type="button" className="boton boton--primario" onClick={() => setMostrandoFormulario(true)}>
          Agregar entidad
        </button>
      </form>

      {cargando ? (
        <p className="mensaje-cargando">Cargando entidades…</p>
      ) : entidadesFiltradas.length === 0 ? (
        <p className="mensaje-vacio">No se encontraron entidades con ese criterio.</p>
      ) : (
        <div className="tabla-contenedor">
          <table className="tabla">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Siglas</th>
                <th>Sector</th>
                <th>Estado</th>
                <th aria-label="Acciones" />
              </tr>
            </thead>
            <tbody>
              {entidadesFiltradas.map((entidad) => (
                <tr key={entidad.id}>
                  <td>{entidad.nombre}</td>
                  <td>{entidad.siglas}</td>
                  <td>{entidad.sector}</td>
                  <td>
                    <span className={`etiqueta-estado ${entidad.activo ? "etiqueta-estado--activo" : "etiqueta-estado--inactivo"}`}>
                      {entidad.activo ? "Activa" : "Inactiva"}
                    </span>
                  </td>
                  <td className="tabla__acciones">
                    <button type="button" className="enlace-accion" onClick={() => setEntidadEditando(entidad)}>
                      Editar
                    </button>
                    <button
                      type="button"
                      className="enlace-accion enlace-accion--peligro"
                      onClick={() => setEntidadAEliminar(entidad)}
                    >
                      Eliminar
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {mostrandoFormulario && (
        <Modal titulo="Agregar entidad al catálogo" onCerrar={() => setMostrandoFormulario(false)}>
          <EntidadForm
            enviando={procesando}
            onGuardar={manejarCrear}
            onCancelar={() => setMostrandoFormulario(false)}
            textoBotonGuardar="Agregar"
          />
        </Modal>
      )}

      {entidadEditando && (
        <Modal titulo="Editar entidad" onCerrar={() => setEntidadEditando(null)}>
          <EntidadForm
            valoresIniciales={entidadEditando}
            enviando={procesando}
            onGuardar={manejarActualizar}
            onCancelar={() => setEntidadEditando(null)}
            textoBotonGuardar="Guardar cambios"
          />
        </Modal>
      )}

      {entidadAEliminar && (
        <Modal titulo="Confirmar eliminación" onCerrar={() => setEntidadAEliminar(null)}>
          <p>
            ¿Seguro que deseas eliminar <strong>{entidadAEliminar.nombre}</strong> del catálogo? Si algún empleado
            tiene este departamento asignado, el texto se conserva en su registro, pero ya no aparecerá como
            opción para nuevos empleados.
          </p>
          <div className="formulario__acciones">
            <button
              type="button"
              className="boton boton--secundario"
              onClick={() => setEntidadAEliminar(null)}
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
    </div>
  );
}
