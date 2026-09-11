using Microsoft.Extensions.Logging;
using SB.PruebaTecnica.Application.DTOs;
using SB.PruebaTecnica.Application.Exceptions;
using SB.PruebaTecnica.Application.Interfaces;
using SB.PruebaTecnica.Application.Services.CalculadoraPago;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Enums;
using SB.PruebaTecnica.Domain.Interfaces;

namespace SB.PruebaTecnica.Application.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repositorio;
        private readonly CalculadoraPagoFactory _calculadoraFactory;
        private readonly ILogger<EmpleadoService> _logger;

        public EmpleadoService(
            IEmpleadoRepository repositorio,
            CalculadoraPagoFactory calculadoraFactory,
            ILogger<EmpleadoService> logger)
        {
            _repositorio = repositorio;
            _calculadoraFactory = calculadoraFactory;
            _logger = logger;
        }

        public async Task<EmpleadoResponseDto> ObtenerPorIdAsync(int id)
        {
            var empleado = await _repositorio.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException(nameof(Empleado), id);

            return MapearAResponse(empleado);
        }

        public async Task<IEnumerable<EmpleadoResponseDto>> ObtenerTodosAsync()
        {
            var empleados = await _repositorio.ObtenerTodosAsync();
            return empleados.Select(MapearAResponse);
        }

        public async Task<IEnumerable<EmpleadoResponseDto>> FiltrarAsync(EmpleadoFiltroDto filtro)
        {
            _logger.LogInformation(
                "Filtrando empleados. Nombre={Nombre}, Departamento={Departamento}, Activo={Activo}",
                filtro.Nombre, filtro.Departamento, filtro.Activo);

            var empleados = await _repositorio.FiltrarAsync(filtro.Nombre, filtro.Departamento, filtro.Activo);
            return empleados.Select(MapearAResponse);
        }

        public async Task<EmpleadoResponseDto> CrearAsync(EmpleadoRequestDto dto)
        {
            ValidarDto(dto);

            var empleado = MapearAEntidad(dto);

            var creado = await _repositorio.CrearAsync(empleado);
            _logger.LogInformation("Empleado creado. Id={Id}, Tipo={Tipo}", creado.Id, creado.Tipo);

            return MapearAResponse(creado);
        }

        public async Task<EmpleadoResponseDto> ActualizarAsync(int id, EmpleadoRequestDto dto)
        {
            var existente = await _repositorio.ObtenerRastreadoPorIdAsync(id)
                ?? throw new NotFoundException(nameof(Empleado), id);

            ValidarDto(dto);

            Empleado resultado;

            if (existente.Tipo == dto.Tipo)
            {
                // Mismo tipo: mutamos las propiedades directamente sobre la
                // instancia que EF Core ya está rastreando. Al tratarse del
                // MISMO objeto (no una copia), el change tracker detecta los
                // cambios automáticamente al llamar GuardarCambiosAsync().
                AplicarValores(existente, dto);
                existente.FechaModificacion = DateTime.UtcNow;

                await _repositorio.GuardarCambiosAsync();
                resultado = existente;
            }
            else
            {
                // El tipo cambió: en TPH no se puede "convertir" una fila de un
                // tipo a otro in-place, así que se reemplaza por completo.
                var reemplazo = MapearAEntidad(dto);
                reemplazo.Id = existente.Id;
                reemplazo.FechaCreacion = existente.FechaCreacion;
                reemplazo.FechaModificacion = DateTime.UtcNow;

                await _repositorio.ReemplazarAsync(existente, reemplazo);
                resultado = reemplazo;
            }

            _logger.LogInformation("Empleado actualizado y pago recalculado. Id={Id}", id);

            return MapearAResponse(resultado);
        }

        public async Task EliminarAsync(int id)
        {
            var existente = await _repositorio.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException(nameof(Empleado), id);

            await _repositorio.EliminarAsync(id);
            _logger.LogInformation("Empleado eliminado. Id={Id}", id);
        }

        public async Task<IEnumerable<ReportePagoDto>> GenerarReporteSemanalAsync()
        {
            var empleados = await _repositorio.FiltrarAsync(null, null, true);

            return empleados.Select(e =>
            {
                var estrategia = _calculadoraFactory.Obtener(e.Tipo);
                return new ReportePagoDto
                {
                    EmpleadoId = e.Id,
                    NombreCompleto = $"{e.PrimerNombre} {e.ApellidoPaterno}".Trim(),
                    TipoDescripcion = e.Tipo.ToString(),
                    DetalleCalculo = estrategia.ExplicarCalculo(e),
                    PagoSemanal = estrategia.Calcular(e)
                };
            });
        }

        // ---------- Helpers privados ----------

        private void ValidarDto(EmpleadoRequestDto dto)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                errores.Add("El apellido paterno es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.NumeroSeguroSocial))
                errores.Add("El número de seguro social es obligatorio.");

            switch (dto.Tipo)
            {
                case TipoEmpleado.Asalariado:
                    if (string.IsNullOrWhiteSpace(dto.PrimerNombre))
                        errores.Add("El primer nombre es obligatorio para empleado asalariado.");
                    if (dto.SalarioSemanal is null or <= 0)
                        errores.Add("El salario semanal debe ser mayor a cero.");
                    break;

                case TipoEmpleado.PorHoras:
                    if (dto.SueldoPorHora is null or <= 0)
                        errores.Add("El sueldo por hora debe ser mayor a cero.");
                    if (dto.HorasTrabajadas is null or < 0)
                        errores.Add("Las horas trabajadas no pueden ser negativas.");
                    break;

                case TipoEmpleado.PorComision:
                    if (string.IsNullOrWhiteSpace(dto.PrimerNombre))
                        errores.Add("El primer nombre es obligatorio para empleado por comisión.");
                    if (dto.VentasBrutas is null or < 0)
                        errores.Add("Las ventas brutas no pueden ser negativas.");
                    if (dto.TarifaComision is null or <= 0)
                        errores.Add("La tarifa de comisión debe ser mayor a cero.");
                    break;

                case TipoEmpleado.AsalariadoPorComision:
                    if (string.IsNullOrWhiteSpace(dto.PrimerNombre))
                        errores.Add("El primer nombre es obligatorio para empleado asalariado por comisión.");
                    if (dto.VentasBrutas is null or < 0)
                        errores.Add("Las ventas brutas no pueden ser negativas.");
                    if (dto.TarifaComision is null or <= 0)
                        errores.Add("La tarifa de comisión debe ser mayor a cero.");
                    if (dto.SalarioBase is null or <= 0)
                        errores.Add("El salario base debe ser mayor a cero.");
                    break;

                default:
                    errores.Add("Tipo de empleado no reconocido.");
                    break;
            }

            if (errores.Count > 0)
                throw new ValidationAppException(errores);
        }

        private static void AplicarValores(Empleado empleado, EmpleadoRequestDto dto)
        {
            empleado.PrimerNombre = dto.PrimerNombre;
            empleado.ApellidoPaterno = dto.ApellidoPaterno;
            empleado.NumeroSeguroSocial = dto.NumeroSeguroSocial;
            empleado.Departamento = dto.Departamento;
            empleado.Activo = dto.Activo;

            switch (empleado)
            {
                case EmpleadoAsalariado a:
                    a.SalarioSemanal = dto.SalarioSemanal!.Value;
                    break;
                case EmpleadoPorHoras h:
                    h.SueldoPorHora = dto.SueldoPorHora!.Value;
                    h.HorasTrabajadas = dto.HorasTrabajadas!.Value;
                    break;
                case EmpleadoPorComision c:
                    c.VentasBrutas = dto.VentasBrutas!.Value;
                    c.TarifaComision = dto.TarifaComision!.Value;
                    break;
                case EmpleadoAsalariadoPorComision ac:
                    ac.VentasBrutas = dto.VentasBrutas!.Value;
                    ac.TarifaComision = dto.TarifaComision!.Value;
                    ac.SalarioBase = dto.SalarioBase!.Value;
                    break;
            }
        }

        private static Empleado MapearAEntidad(EmpleadoRequestDto dto)
        {
            Empleado empleado = dto.Tipo switch
            {
                TipoEmpleado.Asalariado => new EmpleadoAsalariado
                {
                    SalarioSemanal = dto.SalarioSemanal!.Value
                },
                TipoEmpleado.PorHoras => new EmpleadoPorHoras
                {
                    SueldoPorHora = dto.SueldoPorHora!.Value,
                    HorasTrabajadas = dto.HorasTrabajadas!.Value
                },
                TipoEmpleado.PorComision => new EmpleadoPorComision
                {
                    VentasBrutas = dto.VentasBrutas!.Value,
                    TarifaComision = dto.TarifaComision!.Value
                },
                TipoEmpleado.AsalariadoPorComision => new EmpleadoAsalariadoPorComision
                {
                    VentasBrutas = dto.VentasBrutas!.Value,
                    TarifaComision = dto.TarifaComision!.Value,
                    SalarioBase = dto.SalarioBase!.Value
                },
                _ => throw new ValidationAppException("Tipo de empleado no reconocido.")
            };

            empleado.PrimerNombre = dto.PrimerNombre;
            empleado.ApellidoPaterno = dto.ApellidoPaterno;
            empleado.NumeroSeguroSocial = dto.NumeroSeguroSocial;
            empleado.Departamento = dto.Departamento;
            empleado.Activo = dto.Activo;

            return empleado;
        }

        private EmpleadoResponseDto MapearAResponse(Empleado empleado)
        {
            var estrategia = _calculadoraFactory.Obtener(empleado.Tipo);

            var response = new EmpleadoResponseDto
            {
                Id = empleado.Id,
                Tipo = empleado.Tipo,
                TipoDescripcion = empleado.Tipo.ToString(),
                PrimerNombre = empleado.PrimerNombre,
                ApellidoPaterno = empleado.ApellidoPaterno,
                NumeroSeguroSocial = empleado.NumeroSeguroSocial,
                Departamento = empleado.Departamento,
                Activo = empleado.Activo,
                PagoSemanal = estrategia.Calcular(empleado)
            };

            switch (empleado)
            {
                case EmpleadoAsalariado a:
                    response.SalarioSemanal = a.SalarioSemanal;
                    break;
                case EmpleadoPorHoras h:
                    response.SueldoPorHora = h.SueldoPorHora;
                    response.HorasTrabajadas = h.HorasTrabajadas;
                    break;
                case EmpleadoPorComision c:
                    response.VentasBrutas = c.VentasBrutas;
                    response.TarifaComision = c.TarifaComision;
                    break;
                case EmpleadoAsalariadoPorComision ac:
                    response.VentasBrutas = ac.VentasBrutas;
                    response.TarifaComision = ac.TarifaComision;
                    response.SalarioBase = ac.SalarioBase;
                    break;
            }

            return response;
        }
    }
}