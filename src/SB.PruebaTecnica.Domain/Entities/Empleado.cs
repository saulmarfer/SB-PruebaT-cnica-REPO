using SB.PruebaTecnica.Domain.Common;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Domain.Entities
{
    /// <summary>
    /// Entidad base para todos los tipos de empleado.
    /// Se persiste mediante Table-Per-Hierarchy (TPH) con EF Core, usando
    /// la propiedad Tipo como discriminador.
    /// </summary>
    public abstract class Empleado : BaseEntity
    {
        public string? PrimerNombre { get; set; }
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string NumeroSeguroSocial { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public bool Activo { get; set; } = true;

        public abstract TipoEmpleado Tipo { get; }
    }
}
