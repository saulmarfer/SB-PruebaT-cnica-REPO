using SB.PruebaTecnica.Domain.Entities;

namespace SB.PruebaTecnica.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario);
    }
}
