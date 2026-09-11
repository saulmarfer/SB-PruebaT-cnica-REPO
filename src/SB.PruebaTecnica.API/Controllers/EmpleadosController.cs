using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.PruebaTecnica.Application.DTOs;
using SB.PruebaTecnica.Application.Interfaces;

namespace SB.PruebaTecnica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadosController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        /// <summary>Lista todos los empleados con su pago semanal calculado.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpleadoResponseDto>>> ObtenerTodos()
        {
            var empleados = await _empleadoService.ObtenerTodosAsync();
            return Ok(empleados);
        }

        /// <summary>Filtra empleados por nombre, departamento y estado.</summary>
        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<EmpleadoResponseDto>>> Filtrar(
            [FromQuery] string? nombre,
            [FromQuery] string? departamento,
            [FromQuery] bool? activo)
        {
            var filtro = new EmpleadoFiltroDto { Nombre = nombre, Departamento = departamento, Activo = activo };
            var empleados = await _empleadoService.FiltrarAsync(filtro);
            return Ok(empleados);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmpleadoResponseDto>> ObtenerPorId(int id)
        {
            var empleado = await _empleadoService.ObtenerPorIdAsync(id);
            return Ok(empleado);
        }

        /// <summary>Reporte semanal de pagos de todos los empleados activos.</summary>
        [HttpGet("reporte-semanal")]
        public async Task<ActionResult<IEnumerable<ReportePagoDto>>> ReporteSemanal()
        {
            var reporte = await _empleadoService.GenerarReporteSemanalAsync();
            return Ok(reporte);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmpleadoResponseDto>> Crear([FromBody] EmpleadoRequestDto dto)
        {
            var creado = await _empleadoService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmpleadoResponseDto>> Actualizar(int id, [FromBody] EmpleadoRequestDto dto)
        {
            var actualizado = await _empleadoService.ActualizarAsync(id, dto);
            return Ok(actualizado);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _empleadoService.EliminarAsync(id);
            return NoContent();
        }
    }
}
