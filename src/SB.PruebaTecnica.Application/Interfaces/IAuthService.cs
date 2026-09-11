using SB.PruebaTecnica.Application.DTOs;

namespace SB.PruebaTecnica.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task RegistrarAsync(RegistroUsuarioRequestDto dto);
    }
}
