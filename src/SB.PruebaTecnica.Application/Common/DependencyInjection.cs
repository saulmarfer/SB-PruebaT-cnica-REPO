using Microsoft.Extensions.DependencyInjection;
using SB.PruebaTecnica.Application.Interfaces;
using SB.PruebaTecnica.Application.Services;
using SB.PruebaTecnica.Application.Services.CalculadoraPago;

namespace SB.PruebaTecnica.Application.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Servicios de negocio
            services.AddScoped<IEmpleadoService, EmpleadoService>();
            services.AddScoped<IEntidadGubernamentalService, EntidadGubernamentalService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ICalculadoraPagoStrategy, CalculadoraPagoAsalariado>();
            services.AddScoped<ICalculadoraPagoStrategy, CalculadoraPagoPorHoras>();
            services.AddScoped<ICalculadoraPagoStrategy, CalculadoraPagoPorComision>();
            services.AddScoped<ICalculadoraPagoStrategy, CalculadoraPagoAsalariadoPorComision>();
            services.AddScoped<CalculadoraPagoFactory>();

            return services;
        }
    }
}
