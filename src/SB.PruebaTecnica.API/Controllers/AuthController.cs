using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.PruebaTecnica.Application.DTOs;
using SB.PruebaTecnica.Application.Interfaces;

namespace SB.PruebaTecnica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            var resultado = await _authService.LoginAsync(dto);
            return Ok(resultado);
        }

        [HttpPost("registro")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioRequestDto dto)
        {
            await _authService.RegistrarAsync(dto);
            return StatusCode(201, new { exitoso = true, mensaje = "Usuario registrado correctamente." });
        }
    }
}
