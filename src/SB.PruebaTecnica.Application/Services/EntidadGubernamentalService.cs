using Microsoft.Extensions.Logging;
using SB.PruebaTecnica.Application.DTOs;
using SB.PruebaTecnica.Application.Exceptions;
using SB.PruebaTecnica.Application.Interfaces;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Interfaces;

namespace SB.PruebaTecnica.Application.Services
{
    public class EntidadGubernamentalService : IEntidadGubernamentalService
    {
        private readonly IEntidadGubernamentalRepository _repositorio;
        private readonly ILogger<EntidadGubernamentalService> _logger;

        public EntidadGubernamentalService(
            IEntidadGubernamentalRepository repositorio,
            ILogger<EntidadGubernamentalService> logger)
        {
            _repositorio = repositorio;
            _logger = logger;
        }

        public async Task<EntidadGubernamentalResponseDto> ObtenerPorIdAsync(int id)
        {
            var entidad = await _repositorio.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException(nameof(EntidadGubernamental), id);

            return Mapear(entidad);
        }

        public async Task<IEnumerable<EntidadGubernamentalResponseDto>> ObtenerTodosAsync()
        {
            var entidades = await _repositorio.ObtenerTodosAsync();
            return entidades.Select(Mapear);
        }

        public async Task<IEnumerable<EntidadGubernamentalResponseDto>> BuscarAsync(string? termino)
        {
            var entidades = await _repositorio.BuscarAsync(termino);
            return entidades.Select(Mapear);
        }

        public async Task<EntidadGubernamentalResponseDto> CrearAsync(EntidadGubernamentalRequestDto dto)
        {
            Validar(dto);

            var entidad = new EntidadGubernamental
            {
                Nombre = dto.Nombre.Trim(),
                Siglas = dto.Siglas.Trim().ToUpperInvariant(),
                Sector = dto.Sector.Trim(),
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                SitioWeb = dto.SitioWeb,
                Activo = dto.Activo
            };

            var creada = await _repositorio.CrearAsync(entidad);
            _logger.LogInformation("Entidad gubernamental creada. Id={Id}, Siglas={Siglas}", creada.Id, creada.Siglas);

            return Mapear(creada);
        }

        public async Task<EntidadGubernamentalResponseDto> ActualizarAsync(int id, EntidadGubernamentalRequestDto dto)
        {
            var existente = await _repositorio.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException(nameof(EntidadGubernamental), id);

            Validar(dto);

            existente.Nombre = dto.Nombre.Trim();
            existente.Siglas = dto.Siglas.Trim().ToUpperInvariant();
            existente.Sector = dto.Sector.Trim();
            existente.Direccion = dto.Direccion;
            existente.Telefono = dto.Telefono;
            existente.SitioWeb = dto.SitioWeb;
            existente.Activo = dto.Activo;
            existente.FechaModificacion = DateTime.UtcNow;

            await _repositorio.ActualizarAsync(existente);
            _logger.LogInformation("Entidad gubernamental actualizada. Id={Id}", id);

            return Mapear(existente);
        }

        public async Task EliminarAsync(int id)
        {
            var existente = await _repositorio.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException(nameof(EntidadGubernamental), id);

            await _repositorio.EliminarAsync(id);
            _logger.LogInformation("Entidad gubernamental eliminada. Id={Id}", id);
        }

        private static void Validar(EntidadGubernamentalRequestDto dto)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                errores.Add("El nombre de la entidad es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Siglas))
                errores.Add("Las siglas de la entidad son obligatorias.");

            if (string.IsNullOrWhiteSpace(dto.Sector))
                errores.Add("El sector es obligatorio.");

            if (errores.Count > 0)
                throw new ValidationAppException(errores);
        }

        private static EntidadGubernamentalResponseDto Mapear(EntidadGubernamental e) => new()
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Siglas = e.Siglas,
            Sector = e.Sector,
            Direccion = e.Direccion,
            Telefono = e.Telefono,
            SitioWeb = e.SitioWeb,
            Activo = e.Activo
        };
    }
}
