using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SB.PruebaTecnica.Application.Interfaces;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Infrastructure.Settings;

namespace SB.PruebaTecnica.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario)
        {
            var claves = Encoding.UTF8.GetBytes(_settings.Secreto);
            var expiraEn = DateTime.UtcNow.AddMinutes(_settings.MinutosExpiracion);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.NombreUsuario),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(claves), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Emisor,
                audience: _settings.Audiencia,
                claims: claims,
                expires: expiraEn,
                signingCredentials: credenciales);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
        }
    }
}
