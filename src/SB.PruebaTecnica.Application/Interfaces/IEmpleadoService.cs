using SB.PruebaTecnica.Application.DTOs;

namespace SB.PruebaTecnica.Application.Interfaces
{
    public interface IEmpleadoService
    {
        Task<EmpleadoResponseDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<EmpleadoResponseDto>> ObtenerTodosAsync();
        Task<IEnumerable<EmpleadoResponseDto>> FiltrarAsync(EmpleadoFiltroDto filtro);
        Task<EmpleadoResponseDto> CrearAsync(EmpleadoRequestDto dto);
        Task<EmpleadoResponseDto> ActualizarAsync(int id, EmpleadoRequestDto dto);
        Task EliminarAsync(int id);

        /// <summary>
        /// Genera el reporte semanal de pagos de todos los empleados activos.
        /// </summary>
        Task<IEnumerable<ReportePagoDto>> GenerarReporteSemanalAsync();
    }
}
