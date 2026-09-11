import { useState, type FormEvent } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export function LoginPage() {
  const { iniciarSesion, estaAutenticado, cargando, error } = useAuth();
  const [nombreUsuario, setNombreUsuario] = useState("");
  const [password, setPassword] = useState("");

  if (estaAutenticado) return <Navigate to="/" replace />;

  async function manejarEnvio(evento: FormEvent) {
    evento.preventDefault();
    try {
      await iniciarSesion(nombreUsuario, password);
    } catch {
      // el mensaje de error ya queda expuesto por el contexto de auth
    }
  }

  return (
    <div className="pantalla-login">
      <div className="pantalla-login__panel">
        <img src="/assets/logo-sb.svg" alt="Superintendencia de Bancos" className="pantalla-login__logo" />
        <h1>Mantenimiento de Entidades</h1>
        <p className="pantalla-login__subtitulo">Inicia sesión para continuar</p>

        <form onSubmit={manejarEnvio} className="pantalla-login__form">
          <label className="campo">
            <span>Usuario</span>
            <input
              type="text"
              value={nombreUsuario}
              onChange={(e) => setNombreUsuario(e.target.value)}
              autoFocus
              autoComplete="username"
              required
            />
          </label>

          <label className="campo">
            <span>Contraseña</span>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
              required
            />
          </label>

          {error && <p className="pantalla-login__error">{error}</p>}

          <button type="submit" className="boton boton--primario boton--ancho" disabled={cargando}>
            {cargando ? "Ingresando..." : "Ingresar"}
          </button>
        </form>

        <p className="pantalla-login__ayuda">
          Usuario de prueba: <code>admin</code> · Contraseña: <code>Admin123!</code>
        </p>
      </div>
    </div>
  );
}
