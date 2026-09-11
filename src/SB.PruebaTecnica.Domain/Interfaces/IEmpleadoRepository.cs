using SB.PruebaTecnica.Domain.Entities;

namespace SB.PruebaTecnica.Domain.Interfaces
{
    public interface IEmpleadoRepository : IGenericRepository<Empleado>
    {
        Task<IEnumerable<Empleado>> FiltrarAsync(string? nombre, string? departamento, bool? activo);

        Task<Empleado?> ObtenerRastreadoPorIdAsync(int id);

        Task GuardarCambiosAsync();

        Task ReemplazarAsync(Empleado existente, Empleado nuevo);
    }
}