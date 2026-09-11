using System.Net;
using System.Text.Json;
using SB.PruebaTecnica.Application.Exceptions;

namespace SB.PruebaTecnica.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private async Task ManejarExcepcionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, mensaje) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, ex.Message),
                ValidationAppException validationEx => (HttpStatusCode.BadRequest, string.Join(" | ", validationEx.Errores)),
                UnauthorizedAppException => (HttpStatusCode.Unauthorized, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado. Intente nuevamente más tarde.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(ex, "Error no controlado en {Path}", context.Request.Path);
            }
            else
            {
                _logger.LogWarning("Excepción de negocio en {Path}: {Mensaje}", context.Request.Path, mensaje);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var respuesta = new
            {
                exitoso = false,
                mensaje,
                codigo = (int)statusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(respuesta));
        }
    }
}
