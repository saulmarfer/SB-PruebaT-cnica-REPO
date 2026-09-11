# SB - Prueba Técnica

Solución completa (backend + frontend) para la prueba técnica de la Superintendencia
de Bancos de la República Dominicana.

## Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express o completo) — o Docker con `mcr.microsoft.com/mssql/server`
- (Opcional) [dotnet-ef tool](https://learn.microsoft.com/ef/core/cli/dotnet):
  `dotnet tool install --global dotnet-ef`


## Para realizar un inicio rápido (backend + frontend juntos)

```bash
# Terminal 1 — Backend
dotnet restore
cd src/SB.PruebaTecnica.API
dotnet ef migrations add InitialCreate --project ../SB.PruebaTecnica.Infrastructure --startup-project .
dotnet run

# Terminal 2 — Frontend
cd frontend
npm install
cp .env.example .env
npm run dev
```

La API queda disponible en `http://localhost:5099` (o el puerto que asigne tu entorno),
con Swagger en `/swagger`. 

Frontend en `http://localhost:5173`.

### Usuario administrador de prueba (sembrado automáticamente)

| Usuario | Contraseña  |
|---------|-------------|
| `admin` | `Admin123!` |

---


API RESTful en .NET 8 con Onion Architecture, interfaz en React + TypeScript siguiendo `Maqueta.jpg`, y las respuestas de conceptualización listas para el correo.

```
SB-PruebaTecnica/
├── src/                              → Backend (.NET 8 / C#) — ver detalle más abajo
├── tests/                            → Pruebas unitarias del backend (xUnit)
├── scripts/schema.sql                → Script SQL alternativo
├── frontend/                         → App React + TypeScript (Vite) — ver frontend/README.md
├── docs/
│   ├── Respuestas-Conceptualizacion.md  → Las 8 respuestas de la Sección 4, listas para el correo
│   └── recursos-originales/             → Excel y logo originales, archivados como referencia
└── README.md                         → Este archivo (backend)
```

# Backend

API RESTful desarrollada en **.NET 8 / C#** siguiendo **Onion Architecture**, para la
Superintendencia de Bancos de la República Dominicana. Cubre los dos módulos exigidos:

1. **Gestión de pagos de empleados** (Prueba_tecnica-1.pdf): 4 tipos de empleado,
   cálculo de pago semanal, filtros, reportes, roles admin/usuario con JWT.
2. **Mantenimiento de entidades gubernamentales** (API - Especificaciones Técnicas.pdf):
   CRUD persistido en **archivo de texto plano** dentro del propio proyecto.

## Arquitectura

```
SB.PruebaTecnica.sln
├── src/
│   ├── SB.PruebaTecnica.Domain          → Entidades (sin dependencias externas)
│   ├── SB.PruebaTecnica.Application     → Casos de uso, DTOs, patrón Strategy (cálculo de pago), validaciones
│   ├── SB.PruebaTecnica.Infrastructure  → Entity Framework Core (SQL Server), repositorio de archivo de texto, JWT, logging
│   └── SB.PruebaTecnica.API             → Controllers, Program.cs, Swagger, middleware de excepciones
├── tests/
│   └── SB.PruebaTecnica.Tests           → Pruebas unitarias (xUnit)
└── scripts/
    └── schema.sql                       → Script SQL alternativo
```

La regla de dependencias de Onion se respeta en todo momento: 
1. `Domain` no depende de nada.
2. `Application` solo depende de `Domain`.
3. *Infrastructure* implementa las interfaces definidas en `Domain`/`Application`.
4. `API` orquesta todo mediante inyección de dependencias.

## Patrones de diseño usados

- **Strategy + Factory**: cada tipo de empleado (`Asalariado`, `PorHoras`, `PorComision`,
  `AsalariadoPorComision`) tiene su propia clase de cálculo (`ICalculadoraPagoStrategy`).
  `CalculadoraPagoFactory` selecciona la estrategia correcta en tiempo de ejecución.
  Agregar un nuevo tipo de empleado solo requiere una nueva clase + registro en DI,
  sin tocar código existente (Open/Closed Principle).
- **Repository Pattern**: `IEmpleadoRepository`, `IUsuarioRepository` (Entity Framework Core) e
  `IEntidadGubernamentalRepository` (archivo de texto) — la capa de Aplicación no sabe
  ni le importa cómo se persisten los datos.
- **TPH (Table-Per-Hierarchy)** en Entity Framework Core para los 4 subtipos de `Empleado`.

## Entidades gubernamentales / reguladas — datos reales

El archivo `Infrastructure/Data/entidades-gubernamentales.txt` ya viene poblado con
289 entidades reales, extraídas de `listado-de-entidades-autorizadas-a-operar-2018-2026.xlsx`
(el Excel original queda además archivado en `docs/recursos-originales/` como referencia).

Notas sobre la transformación del Excel (11,183 filas = fotos mensuales 2018–2026)
a este archivo (289 filas = una por entidad):

- **`Sector`** = columna `TIPO DE ENTIDAD` del Excel (Bancos Múltiples, Bancos de
  Ahorro y Crédito, Asociaciones de Ahorros y Préstamos, Corporaciones de Crédito,
  Entidades Fiduciarias, Entidades Públicas de Intermediación Financiera, Agentes
  de Cambio, Agentes de Remesas y Cambio).
- **`Activo`** = `true` si la entidad todavía aparece en el período más reciente del
  reporte (junio 2026); `false` si su última aparición es en un mes anterior (es
  decir, dejó de figurar como autorizada). 89 activas / 200 inactivas.
- **`Siglas`** se generó automáticamente a partir de las iniciales del nombre (el
  Excel no trae una columna de siglas oficiales) — son un valor de conveniencia
  para el listado, no la sigla legal/regulatoria de cada entidad.
- **`Direccion`**, **`Telefono`** y **`SitioWeb`** quedan en `null`: esos datos no
  vienen en el Excel. Los campos los acabé añadiendo en el modelo y en el CRUD por
  si se quieren completar manualmente desde la interfaz.

El formato del archivo es **JSON Lines** (un objeto JSON por línea), texto plano
dentro del propio proyecto, tal como exige la especificación técnica.

## Logo de la Superintendencia de Bancos

El archivo que se compartió (`logo-superintendencia-de-bancos.png`) es en realidad
un **SVG**. Se guardó en:

```
src/SB.PruebaTecnica.API/wwwroot/assets/logo-sb.svg
```

y se sirve como archivo estático en `GET /assets/logo-sb.svg` (por ejemplo,
`https://localhost:7099/assets/logo-sb.svg`) para que el frontend lo use
directamente en el header, como indica la maqueta.

## Endpoints principales

| Método | Ruta | Rol requerido |
|---|---|---|
| POST | `/api/auth/login` | Público |
| POST | `/api/auth/registro` | Admin |
| GET | `/api/empleados` | Autenticado |
| GET | `/api/empleados/filtrar?nombre=&departamento=&activo=` | Autenticado |
| GET | `/api/empleados/reporte-semanal` | Autenticado |
| POST/PUT/DELETE | `/api/empleados/{id}` | Admin |
| GET | `/api/entidadesgubernamentales?termino=` | Autenticado |
| POST/PUT/DELETE | `/api/entidadesgubernamentales/{id}` | Admin |

## Pruebas unitarias

```bash
dotnet test
```

Incluye pruebas de las 4 fórmulas de cálculo de pago (incluyendo el caso de horas
extra) y de las validaciones del servicio de entidades gubernamentales.

Serilog escribe a consola y a `src/SB.PruebaTecnica.API/Logs/log-YYYYMMDD.txt`. 
El middleware `ExceptionMiddleware` registratoda excepción no controlada antes 
de devolver una respuesta HTTP consistente.
