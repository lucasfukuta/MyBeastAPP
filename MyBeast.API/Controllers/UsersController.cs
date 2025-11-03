using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.DTOs.User.Input;     // DTOs de Entrada
using MyBeast.API.DTOs.User.Output;    // DTOs de Saída
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [Authorize] e [AllowAnonymous]
using System.Security.Claims; // Para User.Claims
using System.IdentityModel.Tokens.Jwt; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA A CLASSE INTEIRA
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/Users - Apenas para Administradores
        [HttpGet]
        [Authorize(Roles = "Admin")] // SÓ ADMINS PODEM VER TODOS OS USUÁRIOS
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(MapToDto);
            return Ok(userDtos);
        }

        // GET /api/Users/me (Busca o perfil do usuário logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token
            var user = await _userService.GetUserByIdAsync(userId);

            // Verificação manual de nulo é mantida (Serviço retorna nulo, não lança exceção)
            if (user == null) return NotFound("Usuário não encontrado.");

            var userDto = MapToDto(user);
            return Ok(userDto);
        }


        // GET /api/Users/{id} - (Para buscar outros usuários, se permitido)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            // TODO: Adicionar lógica se um usuário pode ver o perfil de outro
            var user = await _userService.GetUserByIdAsync(id);

            // Verificação manual de nulo é mantida
            if (user == null) return NotFound();

            var userDto = MapToDto(user);
            return Ok(userDto);
        }

        // POST /api/Users/register - Aberto ao público
        [AllowAnonymous] // PERMITE ACESSO PÚBLICO
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Middleware cuidará disso
        public async Task<ActionResult<UserDto>> RegisterUser([FromBody] UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará a exceção "Email já cadastrado."
            // e a converterá para 409 Conflict.
            var userToRegister = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = registerDto.Password, // Serviço fará o hash
                PlanType = registerDto.PlanType
            };

            var newUser = await _userService.RegisterUserAsync(userToRegister);
            var newUserDto = MapToDto(newUser);

            return CreatedAtAction(nameof(GetUser), new { id = newUserDto.UserId }, newUserDto);
        }

        // PUT /api/Users/me (Atualiza o perfil do usuário logado)
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Middleware cuidará disso
        public async Task<ActionResult<UserDto>> UpdateMyProfile([FromBody] UserUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetAuthenticatedUserId(); // Pega o ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "não encontrado" (404) ou
            // "Este email já está sendo usado" (409).
            var updatedUser = await _userService.UpdateUserProfileAsync(userId, updateDto.Username, updateDto.Email);
            var updatedUserDto = MapToDto(updatedUser);
            return Ok(updatedUserDto);
        }

        // DELETE /api/Users/me (Deleta a conta do usuário logado)
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userId = GetAuthenticatedUserId(); // Pega o ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "não encontrado" (404).
            await _userService.DeleteUserAsync(userId);
            return NoContent();
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private UserDto MapToDto(User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PlanType = user.PlanType,
                IsModerator = user.IsModerator,
                CreatedAt = user.CreatedAt
            };
        }

        // --- MÉTODO AUXILIAR DE AUTORIZAÇÃO ---
        private int GetAuthenticatedUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // Este erro será capturado pelo Middleware e retornado como 500
                throw new Exception("ID de usuário não encontrado no token. O endpoint está protegido com [Authorize]?");
            }
            return userId;
        }
    }
}