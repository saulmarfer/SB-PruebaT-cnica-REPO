using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SB.PruebaTecnica.Application.Interfaces;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Enums;
using SB.PruebaTecnica.Domain.Interfaces;
using SB.PruebaTecnica.Infrastructure.Persistence;

namespace SB.PruebaTecnica.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            await context.Database.MigrateAsync();

            await SembrarUsuarioAdminAsync(scope.ServiceProvider, logger);
            await SembrarEntidadesGubernamentalesAsync(scope.ServiceProvider, logger);
        }

        private static async Task SembrarUsuarioAdminAsync(IServiceProvider sp, ILogger logger)
        {
            var usuarioRepo = sp.GetRequiredService<IUsuarioRepository>();
            var hasher = sp.GetRequiredService<IPasswordHasher>();

            var existente = await usuarioRepo.ObtenerPorNombreUsuarioAsync("admin");
            if (existente is not null) return;

            var admin = new Usuario
            {
                NombreUsuario = "admin",
                PasswordHash = hasher.Hash("Admin123!"),
                Rol = RolUsuario.Admin,
                Activo = true
            };

            await usuarioRepo.CrearAsync(admin);
            logger.LogInformation("Usuario administrador sembrado (admin / Admin123!). Cámbielo en producción.");
        }

        private static async Task SembrarEntidadesGubernamentalesAsync(IServiceProvider sp, ILogger logger)
        {
            var repo = sp.GetRequiredService<IEntidadGubernamentalRepository>();

            var existentes = await repo.ObtenerTodosAsync();
            if (existentes.Any()) return;

            var entidadesBase = new List<EntidadGubernamental>
            {
                new() { Nombre = "Superintendencia de Bancos de la República Dominicana", Siglas = "SB", Sector = "Financiero", SitioWeb = "https://www.sb.gob.do" },
                new() { Nombre = "Banco Central de la República Dominicana", Siglas = "BCRD", Sector = "Financiero", SitioWeb = "https://www.bancentral.gov.do" },
                new() { Nombre = "Dirección General de Impuestos Internos", Siglas = "DGII", Sector = "Hacienda", SitioWeb = "https://dgii.gov.do" },
                new() { Nombre = "Dirección General de Aduanas", Siglas = "DGA", Sector = "Hacienda", SitioWeb = "https://www.aduanas.gob.do" },
                new() { Nombre = "Ministerio de Hacienda", Siglas = "MH", Sector = "Hacienda", SitioWeb = "https://www.hacienda.gob.do" },
                new() { Nombre = "Superintendencia de Valores de la República Dominicana", Siglas = "SIVAL", Sector = "Financiero", SitioWeb = "https://www.simv.gob.do" },
                new() { Nombre = "Superintendencia de Pensiones", Siglas = "SIPEN", Sector = "Seguridad Social", SitioWeb = "https://www.sipen.gov.do" },
                new() { Nombre = "Superintendencia de Seguros", Siglas = "SIS", Sector = "Financiero", SitioWeb = "https://www.sis.gob.do" },
                new() { Nombre = "Ministerio de Educación", Siglas = "MINERD", Sector = "Educación", SitioWeb = "https://www.ministeriodeeducacion.gob.do" },
                new() { Nombre = "Ministerio de Salud Pública", Siglas = "MSP", Sector = "Salud", SitioWeb = "https://www.msp.gob.do" },
            };

            foreach (var entidad in entidadesBase)
            {
                await repo.CrearAsync(entidad);
            }

            logger.LogInformation("Listado inicial de entidades gubernamentales sembrado ({Cantidad} registros).", entidadesBase.Count);
        }
    }
}
