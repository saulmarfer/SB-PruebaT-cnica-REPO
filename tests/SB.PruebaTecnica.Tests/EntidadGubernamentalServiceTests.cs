using Microsoft.Extensions.Logging.Abstractions;
using SB.PruebaTecnica.Application.DTOs;
using SB.PruebaTecnica.Application.Exceptions;
using SB.PruebaTecnica.Application.Services;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Interfaces;
using Xunit;

namespace SB.PruebaTecnica.Tests
{
    /// <summary>
    /// Repositorio en memoria usado solo para pruebas: evita depender de EF Core
    /// o del sistema de archivos real, manteniendo las pruebas rápidas y aisladas.
    /// </summary>
    public class EntidadGubernamentalRepositoryEnMemoria : IEntidadGubernamentalRepository
    {
        private readonly List<EntidadGubernamental> _datos = new();
        private int _siguienteId = 1;

        public Task<EntidadGubernamental?> ObtenerPorIdAsync(int id) =>
            Task.FromResult(_datos.FirstOrDefault(e => e.Id == id));

        public Task<IEnumerable<EntidadGubernamental>> ObtenerTodosAsync() =>
            Task.FromResult<IEnumerable<EntidadGubernamental>>(_datos);

        public Task<IEnumerable<EntidadGubernamental>> BuscarAsync(string? termino) =>
            Task.FromResult(_datos.Where(e =>
                termino == null || e.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase)));

        public Task<EntidadGubernamental> CrearAsync(EntidadGubernamental entidad)
        {
            entidad.Id = _siguienteId++;
            _datos.Add(entidad);
            return Task.FromResult(entidad);
        }

        public Task ActualizarAsync(EntidadGubernamental entidad) => Task.CompletedTask;

        public Task EliminarAsync(int id)
        {
            _datos.RemoveAll(e => e.Id == id);
            return Task.CompletedTask;
        }
    }

    public class EntidadGubernamentalServiceTests
    {
        private static EntidadGubernamentalService CrearServicio(out IEntidadGubernamentalRepository repo)
        {
            repo = new EntidadGubernamentalRepositoryEnMemoria();
            return new EntidadGubernamentalService(repo, NullLogger<EntidadGubernamentalService>.Instance);
        }

        [Fact]
        public async Task CrearAsync_ConDatosValidos_CreaLaEntidad()
        {
            // Arrange
            var servicio = CrearServicio(out _);
            var dto = new EntidadGubernamentalRequestDto
            {
                Nombre = "Superintendencia de Bancos",
                Siglas = "sb",
                Sector = "Financiero"
            };

            // Act
            var creada = await servicio.CrearAsync(dto);

            // Assert
            Assert.True(creada.Id > 0);
            Assert.Equal("SB", creada.Siglas); // se normaliza a mayúsculas
        }

        [Fact]
        public async Task CrearAsync_SinNombre_LanzaValidationAppException()
        {
            // Arrange
            var servicio = CrearServicio(out _);
            var dto = new EntidadGubernamentalRequestDto
            {
                Nombre = "",
                Siglas = "X",
                Sector = "Y"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationAppException>(() => servicio.CrearAsync(dto));
        }

        [Fact]
        public async Task ObtenerPorIdAsync_IdInexistente_LanzaNotFoundException()
        {
            // Arrange
            var servicio = CrearServicio(out _);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => servicio.ObtenerPorIdAsync(999));
        }
    }
}
