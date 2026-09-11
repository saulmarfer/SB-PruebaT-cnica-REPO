using Microsoft.Extensions.Logging;
using SB.PruebaTecnica.Application.DTOs;
using SB.PruebaTecnica.Application.Exceptions;
using SB.PruebaTecnica.Application.Interfaces;
using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Interfaces;

namespace SB.PruebaTecnica.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepositorio;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUsuarioRepository usuarioRepositorio,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<AuthService> logger)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorNombreUsuarioAsync(dto.NombreUsuario);

            if (usuario is null || !usuario.Activo || !_passwordHasher.Verificar(dto.Password, usuario.PasswordHash))
            {
                _logger.LogWarning("Intento de login fallido para usuario {NombreUsuario}", dto.NombreUsuario);
                throw new UnauthorizedAppException();
            }

            var (token, expiraEn) = _jwtTokenGenerator.GenerarToken(usuario);
            _logger.LogInformation("Login exitoso para usuario {NombreUsuario}", dto.NombreUsuario);

            return new LoginResponseDto
            {
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                ExpiraEn = expiraEn
            };
        }

        public async Task RegistrarAsync(RegistroUsuarioRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationAppException("Nombre de usuario y contraseña son obligatorios.");

            if (dto.Password.Length < 6)
                throw new ValidationAppException("La contraseña debe tener al menos 6 caracteres.");

            var existente = await _usuarioRepositorio.ObtenerPorNombreUsuarioAsync(dto.NombreUsuario);
            if (existente is not null)
                throw new ValidationAppException("El nombre de usuario ya está en uso.");

            var usuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario.Trim(),
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Rol = dto.Rol,
                Activo = true
            };

            await _usuarioRepositorio.CrearAsync(usuario);
            _logger.LogInformation("Usuario registrado: {NombreUsuario}, Rol={Rol}", usuario.NombreUsuario, usuario.Rol);
        }
    }
}
