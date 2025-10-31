using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq; // Para Select
using System.Threading.Tasks;
using MyBeast.API.DTOs.User.Input;
using MyBeast.API.DTOs.User.Output;

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/Users - Mapeia para UserDto
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            // Mapear Model para Dto para garantir que o hash não vaze
            var userDtos = users.Select(u => new UserDto { /* Mapeie os campos */ UserId = u.UserId, Username = u.Username, Email = u.Email, PlanType = u.PlanType, IsModerator = u.IsModerator, CreatedAt = u.CreatedAt });
            return Ok(userDtos);
        }

        // GET /api/Users/{id} - Mapeia para UserDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id); // Serviço já limpa hash
            if (user == null) return NotFound();

            // Mapear Model para Dto
            var userDto = new UserDto { /* Mapeie os campos */ UserId = user.UserId, Username = user.Username, Email = user.Email, PlanType = user.PlanType, IsModerator = user.IsModerator, CreatedAt = user.CreatedAt };
            return Ok(userDto);
        }

        // POST /api/Users/register - Usa UserRegisterDto
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))] // Retorna UserDto
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> RegisterUser([FromBody] UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Mapear DTO para Modelo
                var userToRegister = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = registerDto.Password, // Passa a senha pura para o serviço fazer o hash
                    PlanType = registerDto.PlanType
                };

                var newUser = await _userService.RegisterUserAsync(userToRegister); // Serviço retorna sem hash

                // Mapear Modelo de resposta para Dto
                var newUserDto = new UserDto { /* Mapeie os campos */ UserId = newUser.UserId, Username = newUser.Username, Email = newUser.Email, PlanType = newUser.PlanType, IsModerator = newUser.IsModerator, CreatedAt = newUser.CreatedAt };

                return CreatedAtAction(nameof(GetUser), new { id = newUserDto.UserId }, newUserDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/Users/{id} (Novo) - Usa UserUpdateDto
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))] // Retorna UserDto
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UserUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Adicionar verificação de permissão (usuário só pode editar a si mesmo, ou admin)
            // int requestingUserId = ObterUserIdDoToken();
            // if (id != requestingUserId && !User.IsInRole("Admin")) return Forbid();

            try
            {
                var updatedUser = await _userService.UpdateUserProfileAsync(id, updateDto.Username, updateDto.Email); // Serviço retorna sem hash

                // Mapear Modelo de resposta para Dto
                var updatedUserDto = new UserDto { /* Mapeie os campos */ UserId = updatedUser.UserId, Username = updatedUser.Username, Email = updatedUser.Email, PlanType = updatedUser.PlanType, IsModerator = updatedUser.IsModerator, CreatedAt = updatedUser.CreatedAt };
                return Ok(updatedUserDto);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/Users/{id} (Novo)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Para erro de permissão
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // TODO: Adicionar verificação de permissão (usuário só pode deletar a si mesmo, ou admin)
            // int requestingUserId = ObterUserIdDoToken();
            // if (id != requestingUserId && !User.IsInRole("Admin")) return Forbid();

            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Poderia ser um erro genérico 500 ou um 400 se a regra de negócio impedir
                return BadRequest(ex.Message);
            }
        }
        // (Adicionaremos Login [HttpPost("login")] depois)
    }
}