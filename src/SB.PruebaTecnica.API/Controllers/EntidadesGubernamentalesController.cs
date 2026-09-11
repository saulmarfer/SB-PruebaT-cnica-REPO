using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.PruebaTecnica.Application.DTOs;
using SB.PruebaTecnica.Application.Interfaces;

namespace SB.PruebaTecnica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EntidadesGubernamentalesController : ControllerBase
    {
        private readonly IEntidadGubernamentalService _service;

        public EntidadesGubernamentalesController(IEntidadGubernamentalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntidadGubernamentalResponseDto>>> ObtenerTodos(
            [FromQuery] string? termino)
        {
            var entidades = string.IsNullOrWhiteSpace(termino)
                ? await _service.ObtenerTodosAsync()
                : await _service.BuscarAsync(termino);

            return Ok(entidades);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EntidadGubernamentalResponseDto>> ObtenerPorId(int id)
        {
            var entidad = await _service.ObtenerPorIdAsync(id);
            return Ok(entidad);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EntidadGubernamentalResponseDto>> Crear(
            [FromBody] EntidadGubernamentalRequestDto dto)
        {
            var creada = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EntidadGubernamentalResponseDto>> Actualizar(
            int id, [FromBody] EntidadGubernamentalRequestDto dto)
        {
            var actualizada = await _service.ActualizarAsync(id, dto);
            return Ok(actualizada);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
