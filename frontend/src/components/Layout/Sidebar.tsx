import { NavLink } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

const IconoInicio = () => (
  <svg viewBox="0 0 24 24" width="18" height="18" fill="none" aria-hidden="true">
    <path
      d="M4 11.5 12 4l8 7.5M6 10v9a1 1 0 0 0 1 1h3.5v-5.5h3V20H17a1 1 0 0 0 1-1v-9"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    />
  </svg>
);

const IconoConsulta = () => (
  <svg viewBox="0 0 24 24" width="18" height="18" fill="none" aria-hidden="true">
    <circle cx="10.5" cy="10.5" r="6" stroke="currentColor" strokeWidth="1.8" />
    <path d="m20 20-4.6-4.6" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" />
  </svg>
);

const IconoCrear = () => (
  <svg viewBox="0 0 24 24" width="18" height="18" fill="none" aria-hidden="true">
    <path d="M12 5v14M5 12h14" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" />
  </svg>
);

const IconoEntidades = () => (
  <svg viewBox="0 0 24 24" width="18" height="18" fill="none" aria-hidden="true">
    <path
      d="M4 21V9l8-5 8 5v12M9 21v-6h6v6M4 21h16"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    />
  </svg>
);

const enlaces = [
  { to: "/", etiqueta: "Inicio", fin: true, Icono: IconoInicio, soloAdmin: false },
  { to: "/consulta", etiqueta: "Consulta", fin: false, Icono: IconoConsulta, soloAdmin: false },
  { to: "/crear-registro", etiqueta: "Crear registro", fin: false, Icono: IconoCrear, soloAdmin: true },
  { to: "/entidades", etiqueta: "Entidades", fin: false, Icono: IconoEntidades, soloAdmin: true },
];

export function Sidebar() {
  const { usuario, esAdmin, cerrarSesion } = useAuth();

  return (
    <aside className="sidebar">
      <div className="sidebar__marca">
        <img src="/assets/logo-sb.svg" alt="Superintendencia de Bancos" className="sidebar__logo" />
      </div>

      <nav className="sidebar__nav" aria-label="Navegación principal">
        {enlaces
          .filter((enlace) => !enlace.soloAdmin || esAdmin)
          .map(({ to, etiqueta, fin, Icono }) => (
            <NavLink
              key={to}
              to={to}
              end={fin}
              className={({ isActive }) => `sidebar__enlace${isActive ? " sidebar__enlace--activo" : ""}`}
            >
              <span className="sidebar__icono">
                <Icono />
              </span>
              <span>{etiqueta}</span>
            </NavLink>
          ))}
      </nav>

      <div className="sidebar__pie">
        <div className="sidebar__usuario">
          <span className="sidebar__usuario-nombre">{usuario?.nombreUsuario}</span>
          <span className="sidebar__usuario-rol">{usuario?.rol}</span>
        </div>
        <button type="button" className="sidebar__salir" onClick={cerrarSesion}>
          Cerrar sesión
        </button>
      </div>
    </aside>
  );
}
