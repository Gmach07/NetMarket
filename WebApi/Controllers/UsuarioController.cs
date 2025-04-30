using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Dtos;
using WebApi.Errors;

namespace WebApi.Controllers
{
    public class UsuarioController : BaseApiController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ITokenService _tokenService;

        public UsuarioController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> Login(LoginDto loginDto)
        {
            var u = await _userManager.FindByEmailAsync(loginDto.Email);
            if (u == null) return Unauthorized(new CodeErrorResponse(401, "Credenciales inválidas"));

            var res = await _signInManager.CheckPasswordSignInAsync(u, loginDto.Password, false);
            if (!res.Succeeded) return Unauthorized(new CodeErrorResponse(401, "Credenciales inválidas"));

            return Ok(new UsuarioDto
            {
                Email = u.Email,
                Username = u.UserName,
                Token = _tokenService.CreateToken(u),
                Nombre = u.Nombre,
                Apellido = u.Apellido
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UsuarioDto>> GetUsuario()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var u = await _userManager.FindByEmailAsync(email);
            if (u == null) return NotFound(new CodeErrorResponse(404, "Usuario no encontrado"));

            return Ok(new UsuarioDto
            {
                Email = u.Email,
                Username = u.UserName,
                Token = _tokenService.CreateToken(u),
                Nombre = u.Nombre,
                Apellido = u.Apellido
            });
        }
    }
}
