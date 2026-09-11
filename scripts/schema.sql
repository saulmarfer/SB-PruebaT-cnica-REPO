IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SB_PruebaTecnica')
BEGIN
    CREATE DATABASE SB_PruebaTecnica;
END
GO

USE SB_PruebaTecnica;
GO

IF OBJECT_ID('dbo.Empleados', 'U') IS NOT NULL DROP TABLE dbo.Empleados;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
GO

-- Tabla para los 4 tipos de empleado.
-- TipoEmpleado indica qué subtipo representa cada fila
CREATE TABLE dbo.Empleados (
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    TipoEmpleado         INT             NOT NULL,
    PrimerNombre         NVARCHAR(100)   NULL,
    ApellidoPaterno      NVARCHAR(100)   NOT NULL,
    NumeroSeguroSocial   NVARCHAR(20)    NOT NULL,
    Departamento         NVARCHAR(100)   NULL,
    Activo               BIT             NOT NULL DEFAULT 1,

    SalarioSemanal       DECIMAL(18,2)   NULL,  -- Asalariado
    SueldoPorHora        DECIMAL(18,2)   NULL,  -- PorHoras
    HorasTrabajadas      DECIMAL(18,2)   NULL,  -- PorHoras
    VentasBrutas         DECIMAL(18,2)   NULL,  -- PorComision / AsalariadoPorComision
    TarifaComision       DECIMAL(18,4)   NULL,  -- PorComision / AsalariadoPorComision
    SalarioBase          DECIMAL(18,2)   NULL,  -- AsalariadoPorComision

    FechaCreacion        DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    FechaModificacion    DATETIME2       NULL
);
GO

CREATE TABLE dbo.Usuarios (
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario        NVARCHAR(100)   NOT NULL UNIQUE,
    PasswordHash         NVARCHAR(200)   NOT NULL,
    Rol                  INT             NOT NULL, -- 1 = Admin, 2 = Usuario
    Activo               BIT             NOT NULL DEFAULT 1,
    FechaCreacion        DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    FechaModificacion    DATETIME2       NULL
);
GO
