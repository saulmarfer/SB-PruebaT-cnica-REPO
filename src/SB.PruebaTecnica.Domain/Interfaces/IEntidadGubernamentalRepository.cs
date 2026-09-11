using SB.PruebaTecnica.Domain.Entities;

namespace SB.PruebaTecnica.Domain.Interfaces
{
    public interface IEntidadGubernamentalRepository : IGenericRepository<EntidadGubernamental>
    {
        Task<IEnumerable<EntidadGubernamental>> BuscarAsync(string? termino);
    }
}
