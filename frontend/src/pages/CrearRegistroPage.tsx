import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { crearEmpleado } from "../api/empleados";
import { listarEntidades } from "../api/entidades";
import { EmpleadoForm } from "../components/Common/EmpleadoForm";
import { useToast } from "../context/ToastContext";
import type { EmpleadoRequestDto } from "../api/types";
import { ApiError } from "../api/client";

export function CrearRegistroPage() {
  const [departamentos, setDepartamentos] = useState<string[]>([]);
  const [enviando, setEnviando] = useState(false);
  const { notificarExito, notificarError } = useToast();
  const navigate = useNavigate();

  useEffect(() => {
    listarEntidades()
      .then((entidades) => setDepartamentos(entidades.filter((e) => e.activo).map((e) => e.nombre)))
      .catch(() => {
        /* si falla, el select de departamento simplemente queda sin opciones */
      });
  }, []);

  async function manejarGuardar(dto: EmpleadoRequestDto) {
    setEnviando(true);
    try {
      const creado = await crearEmpleado(dto);
      notificarExito(`"${creado.apellidoPaterno}" se registró correctamente.`);
      navigate("/consulta");
    } catch (err) {
      notificarError(err instanceof ApiError ? err.message : "No fue posible crear el registro.");
    } finally {
      setEnviando(false);
    }
  }

  return (
    <div>
      <p className="descripcion-pagina">
        Registra un nuevo empleado. Los campos disponibles cambian según el tipo que selecciones, siguiendo
        exactamente la captura de datos definida para cada uno. Los campos marcados con * son obligatorios.
      </p>
      <EmpleadoForm
        departamentos={departamentos}
        enviando={enviando}
        onGuardar={manejarGuardar}
        textoBotonGuardar="Crear registro"
      />
    </div>
  );
}
