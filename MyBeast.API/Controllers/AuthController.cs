using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.DTOs.Auth.Input;     // DTO de Entrada
using MyBeast.Domain.DTOs.Auth.Output;    // DTO de Saída
using MyBeast.Domain.DTOs.User.Output;

namespace MyBeast.API.Controllers
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

        // POST /api/auth/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // 1. Chamar o serviço, que retorna a tupla (Token, Expiration, User model)
                var (token, expiration, userModel) = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

                // 2. Mapear o User model para um UserDto (Lógica de Mapeamento VIVE no Controller)
                var userDto = new UserDto
                {
                    UserId = userModel.UserId,
                    Username = userModel.Username,
                    Email = userModel.Email,
                    PlanType = userModel.PlanType,
                    IsModerator = userModel.IsModerator,
                    CreatedAt = userModel.CreatedAt
                };

                // 3. Criar o DTO de Resposta Final
                var loginResponse = new LoginResponseDto
                {
                    Token = token,
                    Expiration = expiration,
                    User = userDto
                };

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // "Email ou senha inválidos."
            }
        }
    }
}