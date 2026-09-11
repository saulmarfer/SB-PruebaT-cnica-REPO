using SB.PruebaTecnica.Domain.Common;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
        public bool Activo { get; set; } = true;
    }
}
