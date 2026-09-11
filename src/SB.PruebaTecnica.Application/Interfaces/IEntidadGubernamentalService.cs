using SB.PruebaTecnica.Application.DTOs;

namespace SB.PruebaTecnica.Application.Interfaces
{
    public interface IEntidadGubernamentalService
    {
        Task<EntidadGubernamentalResponseDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<EntidadGubernamentalResponseDto>> ObtenerTodosAsync();
        Task<IEnumerable<EntidadGubernamentalResponseDto>> BuscarAsync(string? termino);
        Task<EntidadGubernamentalResponseDto> CrearAsync(EntidadGubernamentalRequestDto dto);
        Task<EntidadGubernamentalResponseDto> ActualizarAsync(int id, EntidadGubernamentalRequestDto dto);
        Task EliminarAsync(int id);
    }
}
