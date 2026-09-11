using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Interfaces;
using SB.PruebaTecnica.Infrastructure.Settings;

namespace SB.PruebaTecnica.Infrastructure.Repositories
{
    public class EntidadGubernamentalRepository : IEntidadGubernamentalRepository
    {
        private readonly string _rutaArchivo;
        private readonly ILogger<EntidadGubernamentalRepository> _logger;
        private static readonly SemaphoreSlim _lock = new(1, 1);

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false
        };

        public EntidadGubernamentalRepository(
            IOptions<ArchivoDatosSettings> settings,
            ILogger<EntidadGubernamentalRepository> logger)
        {
            _logger = logger;

            var directorio = Path.Combine(AppContext.BaseDirectory, settings.Value.Carpeta);
            Directory.CreateDirectory(directorio);

            _rutaArchivo = Path.Combine(directorio, settings.Value.ArchivoEntidadesGubernamentales);

            if (!File.Exists(_rutaArchivo))
            {
                File.Create(_rutaArchivo).Dispose();
                _logger.LogInformation("Archivo de entidades gubernamentales creado en {Ruta}", _rutaArchivo);
            }
        }

        public async Task<EntidadGubernamental?> ObtenerPorIdAsync(int id)
        {
            var entidades = await LeerTodasAsync();
            return entidades.FirstOrDefault(e => e.Id == id);
        }

        public async Task<IEnumerable<EntidadGubernamental>> ObtenerTodosAsync() =>
            await LeerTodasAsync();

        public async Task<IEnumerable<EntidadGubernamental>> BuscarAsync(string? termino)
        {
            var entidades = await LeerTodasAsync();

            if (string.IsNullOrWhiteSpace(termino))
                return entidades;

            termino = termino.Trim();

            return entidades.Where(e =>
                e.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                e.Siglas.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                e.Sector.Contains(termino, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<EntidadGubernamental> CrearAsync(EntidadGubernamental entidad)
        {
            await _lock.WaitAsync();
            try
            {
                var entidades = await LeerTodasSinBloqueoAsync();

                entidad.Id = entidades.Count == 0 ? 1 : entidades.Max(e => e.Id) + 1;
                entidad.FechaCreacion = DateTime.UtcNow;

                entidades.Add(entidad);
                await EscribirTodasSinBloqueoAsync(entidades);

                return entidad;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task ActualizarAsync(EntidadGubernamental entidad)
        {
            await _lock.WaitAsync();
            try
            {
                var entidades = await LeerTodasSinBloqueoAsync();
                var index = entidades.FindIndex(e => e.Id == entidad.Id);

                if (index == -1)
                {
                    _logger.LogWarning("Intento de actualizar entidad gubernamental inexistente. Id={Id}", entidad.Id);
                    return;
                }

                entidades[index] = entidad;
                await EscribirTodasSinBloqueoAsync(entidades);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task EliminarAsync(int id)
        {
            await _lock.WaitAsync();
            try
            {
                var entidades = await LeerTodasSinBloqueoAsync();
                var eliminadas = entidades.RemoveAll(e => e.Id == id);

                if (eliminadas > 0)
                {
                    await EscribirTodasSinBloqueoAsync(entidades);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        // ---------- Acceso a archivo ----------

        private async Task<List<EntidadGubernamental>> LeerTodasAsync()
        {
            await _lock.WaitAsync();
            try
            {
                return await LeerTodasSinBloqueoAsync();
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<List<EntidadGubernamental>> LeerTodasSinBloqueoAsync()
        {
            var lineas = await File.ReadAllLinesAsync(_rutaArchivo);
            var resultado = new List<EntidadGubernamental>();

            foreach (var linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                try
                {
                    var entidad = JsonSerializer.Deserialize<EntidadGubernamental>(linea, _jsonOptions);
                    if (entidad is not null) resultado.Add(entidad);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Línea corrupta en el archivo de entidades gubernamentales: {Linea}", linea);
                }
            }

            return resultado;
        }

        private async Task EscribirTodasSinBloqueoAsync(List<EntidadGubernamental> entidades)
        {
            var lineas = entidades.Select(e => JsonSerializer.Serialize(e, _jsonOptions));
            await File.WriteAllLinesAsync(_rutaArchivo, lineas);
        }
    }
}
