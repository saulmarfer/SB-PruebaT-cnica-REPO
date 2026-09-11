using SB.PruebaTecnica.Domain.Entities;

namespace SB.PruebaTecnica.Domain.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
    }
}
