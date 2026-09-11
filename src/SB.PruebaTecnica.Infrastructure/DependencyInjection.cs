using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SB.PruebaTecnica.Application.Interfaces;
using SB.PruebaTecnica.Domain.Interfaces;
using SB.PruebaTecnica.Infrastructure.Persistence;
using SB.PruebaTecnica.Infrastructure.Repositories;
using SB.PruebaTecnica.Infrastructure.Security;
using SB.PruebaTecnica.Infrastructure.Settings;

namespace SB.PruebaTecnica.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SqlServerConnection"),
                    sqlOptions => sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SeccionConfiguracion));
            services.Configure<ArchivoDatosSettings>(configuration.GetSection(ArchivoDatosSettings.SeccionConfiguracion));

            services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            services.AddSingleton<IEntidadGubernamentalRepository, EntidadGubernamentalRepository>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}