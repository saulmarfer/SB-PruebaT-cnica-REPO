using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.DTOs
{
    public class LoginRequestDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
        public DateTime ExpiraEn { get; set; }
    }

    public class RegistroUsuarioRequestDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; } = RolUsuario.Usuario;
    }
}
