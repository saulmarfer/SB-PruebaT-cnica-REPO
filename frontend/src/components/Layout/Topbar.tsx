import { useLocation } from "react-router-dom";

const titulos: Record<string, string> = {
  "/": "Inicio",
  "/consulta": "Consulta de empleados",
  "/crear-registro": "Crear registro de empleado",
  "/entidades": "Administrar departamentos (Entidades)",
  "/login": "Iniciar sesión",
};

function tituloParaRuta(pathname: string): string {
  if (titulos[pathname]) return titulos[pathname];
  if (pathname.startsWith("/consulta")) return "Consulta de empleados";
  if (pathname.startsWith("/entidades")) return "Administrar departamentos (Entidades)";
  return "Nombre de la página";
}

export function Topbar() {
  const { pathname } = useLocation();

  return (
    <header className="topbar">
      <h1 className="topbar__titulo">{tituloParaRuta(pathname)}</h1>
    </header>
  );
}
