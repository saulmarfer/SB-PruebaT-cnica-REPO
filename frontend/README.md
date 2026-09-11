# SB - Prueba Técnica (Frontend)

Interfaz en **React 19 + TypeScript + Vite** para el mantenimiento (CRUD) de entidades
reguladas por la Superintendencia de Bancos, siguiendo `Maqueta.jpg`: sidebar azul,
contenido en tarjeta blanca sobre fondo gris, logo oficial de la SB.

## Prerrequisitos

- Node.js 20+ y npm
- El backend (`SB.PruebaTecnica.API`) corriendo — ver `../README.md`

## Cómo ejecutar

```bash
cd frontend
npm install

# Copia el ejemplo y ajusta la URL si tu API corre en otro puerto
cp .env.example .env

npm run dev
```

Abre `http://localhost:5173`. Inicia sesión con el usuario sembrado por el backend:

| Usuario | Contraseña  |
|---------|-------------|
| `admin` | `Admin123!` |

> Si usas un usuario con rol `Usuario` (no admin), la app oculta automáticamente
> "Crear registro" y los botones de Editar/Eliminar — esos roles solo pueden consultar.

## Variables de entorno

| Variable | Descripción | Valor por defecto |
|---|---|---|
| `VITE_API_URL` | Base de la API (incluye `/api`) | `https://localhost:7099/api` |

## Estructura

```
src/
├── api/          → cliente HTTP tipado (fetch + manejo de errores + JWT), un archivo por recurso
├── context/       → AuthContext (sesión/JWT) y ToastContext (notificaciones)
├── components/
│   ├── Layout/     → Sidebar, Topbar, AppLayout
│   └── Common/     → EmpleadoForm, EntidadForm, Modal
├── pages/
│   ├── LoginPage.tsx
│   ├── InicioPage.tsx           → resumen de nómina semanal y empleados por tipo
│   ├── ConsultaPage.tsx         → Empleados: filtros por nombre/departamento/estado + reporte semanal
│   ├── CrearRegistroPage.tsx    → alta de empleado (formulario dinámico según tipo)
│   └── EntidadesAdminPage.tsx   → catálogo de Entidades Gubernamentales (alimenta "Departamento")
└── index.css      → tokens de marca + estilos de toda la app
```

**Empleados es el módulo protagonista** ("Consulta" y "Crear registro" del sidebar, tal como pide
la Sección 3 del PDF principal). **Entidades Gubernamentales** es un catálogo de referencia con su
propia pantalla de administración ("Entidades" en el sidebar, solo Admin) — su propósito es
alimentar el selector de "Departamento" al gestionar empleados, no ser una pantalla protagonista.
El razonamiento completo de esta decisión está en `../docs/Bitacora-Cambios-y-Decisiones.txt`.

## Decisiones de diseño

- **Colores exactos de la especificación**: `--azul: rgba(13,48,72,.9)` y
  `--gris-fondo: rgba(237,240,247)` como variables CSS en `index.css`, usadas en
  sidebar/topbar y fondo respectivamente.
- **El ícono es el logo de la SB** (confirmado por el cliente): se usa como marca en
  el sidebar y en el login. Los íconos de cada enlace de navegación (Inicio, Consulta,
  Crear registro) son SVG propios en el color de acento, para que se lean bien a un
  tamaño pequeño sin deformar el logo.
- **CRUD 100% desde la interfaz**: Consulta (Empleados) permite buscar por nombre, filtrar por
  departamento y estado, editar (modal, recalcula el pago) y eliminar (con confirmación); Crear
  registro da de alta nuevos empleados con un formulario que cambia según el tipo seleccionado.
  "Entidades" (solo Admin) gestiona el catálogo que alimenta el selector de Departamento.
- **Manejo de sesión**: token JWT en `localStorage`; un 401 de la API cierra la sesión
  y redirige a `/login` automáticamente (interceptado en `api/client.ts`).

## Compilar para producción

```bash
npm run build
```

Genera `dist/` (verificado: compila sin errores de TypeScript ni del linter).
Sírvelo con cualquier servidor estático, o ajusta el `Cors:OrigenesPermitidos` del
backend para apuntar a donde lo despliegues.
