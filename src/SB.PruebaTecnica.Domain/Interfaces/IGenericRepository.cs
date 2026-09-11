namespace SB.PruebaTecnica.Domain.Interfaces
{
    /// <summary>
    /// Contrato genérico de repositorio, independiente del mecanismo de persistencia
    /// (EF Core / SQL Server o archivo de texto plano).
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<T> CrearAsync(T entidad);
        Task ActualizarAsync(T entidad);
        Task EliminarAsync(int id);
    }
}
