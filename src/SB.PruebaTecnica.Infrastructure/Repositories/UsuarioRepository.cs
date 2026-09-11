using Microsoft.EntityFrameworkCore;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Interfaces;
using SB.PruebaTecnica.Infrastructure.Persistence;

namespace SB.PruebaTecnica.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id) =>
            await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync() =>
            await _context.Usuarios.AsNoTracking().ToListAsync();

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario) =>
            await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

        public async Task<Usuario> CrearAsync(Usuario entidad)
        {
            _context.Usuarios.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        public async Task ActualizarAsync(Usuario entidad)
        {
            _context.Usuarios.Update(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var entidad = await _context.Usuarios.FindAsync(id);
            if (entidad is null) return;

            _context.Usuarios.Remove(entidad);
            await _context.SaveChangesAsync();
        }
    }
}
