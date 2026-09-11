using Microsoft.EntityFrameworkCore;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Interfaces;
using SB.PruebaTecnica.Infrastructure.Persistence;

namespace SB.PruebaTecnica.Infrastructure.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly AppDbContext _context;

        public EmpleadoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Empleado?> ObtenerPorIdAsync(int id) =>
            await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Empleado?> ObtenerRastreadoPorIdAsync(int id) =>
            await _context.Empleados.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IEnumerable<Empleado>> ObtenerTodosAsync() =>
            await _context.Empleados.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Empleado>> FiltrarAsync(string? nombre, string? departamento, bool? activo)
        {
            var query = _context.Empleados.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(e =>
                    (e.PrimerNombre != null && e.PrimerNombre.Contains(nombre)) ||
                    e.ApellidoPaterno.Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(departamento))
            {
                query = query.Where(e => e.Departamento != null && e.Departamento.Contains(departamento));
            }

            if (activo.HasValue)
            {
                query = query.Where(e => e.Activo == activo.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Empleado> CrearAsync(Empleado entidad)
        {
            _context.Empleados.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        public async Task ActualizarAsync(Empleado entidad)
        {
            _context.Empleados.Update(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync() =>
            await _context.SaveChangesAsync();

        public async Task ReemplazarAsync(Empleado existente, Empleado nuevo)
        {
            _context.Empleados.Remove(existente);
            _context.Empleados.Add(nuevo);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var entidad = await _context.Empleados.FindAsync(id);
            if (entidad is null) return;

            _context.Empleados.Remove(entidad);
            await _context.SaveChangesAsync();
        }
    }
}