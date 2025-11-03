using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.DTOs.MealLog.Input;   // DTOs de Entrada
using MyBeast.API.DTOs.MealLog.Output;  // DTOs de Saída
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using System.Security.Claims; // Para Claims
using System.IdentityModel.Tokens.Jwt; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA TODOS OS ENDPOINTS
    [ApiController]
    [Route("api/[controller]")]
    public class MealLogsController : ControllerBase
    {
        private readonly IMealLogService _mealLogService;

        public MealLogsController(IMealLogService mealLogService)
        {
            _mealLogService = mealLogService;
        }

        // GET /api/MealLogs/me (Busca logs do usuário logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealLogDto>))]
        public async Task<ActionResult<IEnumerable<MealLogDto>>> GetMyLogs()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token
            var logs = await _mealLogService.GetMealLogsByUserIdAsync(userId);
            var logDtos = logs.Select(MapToDto);
            return Ok(logDtos);
        }

        // GET /api/MealLogs/me/date/{dateString} (Busca logs do usuário logado por data)
        [HttpGet("me/date/{dateString}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealLogDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MealLogDto>>> GetMyLogsByDate(string dateString)
        {
            if (!DateOnly.TryParse(dateString, out var date))
            {
                return BadRequest("Formato de data inválido. Use YYYY-MM-DD.");
            }

            var userId = GetAuthenticatedUserId(); // Obtém ID do token
            var logs = await _mealLogService.GetMealLogsByUserIdAndDateAsync(userId, date.ToDateTime(TimeOnly.MinValue));
            var logDtos = logs.Select(MapToDto);
            return Ok(logDtos);
        }

        // GET /api/MealLogs/{id} (Busca um log específico)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealLogDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MealLogDto>> GetLogById(int id)
        {
            var log = await _mealLogService.GetMealLogByIdAsync(id);
            if (log == null) return NotFound();

            // --- VERIFICAÇÃO DE AUTORIZAÇÃO ---
            var userId = GetAuthenticatedUserId();
            if (log.UserId != userId)
            {
                return Forbid(); // 403 Forbidden - Este log não é seu
            }
            // --- FIM DA VERIFICAÇÃO ---

            var logDto = MapToDto(log);
            return Ok(logDto);
        }

        // POST /api/MealLogs (Registra uma refeição para o usuário logado)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MealLogDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Se Alimento/User não for encontrado
        public async Task<ActionResult<MealLogDto>> LogMeal([FromBody] LogMealDto logDto)
        {
            if (logDto == null || !ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "Usuário não encontrado" (404) ou "Alimento não encontrado" (404).
            var items = logDto.Items.Select(dto => new MealLogItem
            {
                FoodId = dto.FoodId,
                Quantity = dto.Quantity
            }).ToList();

            var newLog = await _mealLogService.LogMealAsync(userId, logDto.Date, logDto.MealType, items); // Passa o ID do token

            var createdLogWithDetails = await _mealLogService.GetMealLogByIdAsync(newLog.MealLogId);
            if (createdLogWithDetails == null) return BadRequest("Erro ao buscar log criado.");

            var newLogDto = MapToDto(createdLogWithDetails);
            return CreatedAtAction(nameof(GetLogById), new { id = newLogDto.MealLogId }, newLogDto);
        }

        // PUT /api/MealLogs/{id} (Atualiza um log do usuário logado)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealLogDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealLogDto>> UpdateMealLog(int id, [FromBody] MealLogUpdateDto updateDto)
        {
            if (updateDto == null || !ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "não encontrado" (404), "permissão" (403) ou "ArgumentException" (400).
            List<MealLogItem>? itemsToUpdate = null;
            if (updateDto.Items != null)
            {
                itemsToUpdate = updateDto.Items.Select(dto => new MealLogItem
                {
                    FoodId = dto.FoodId,
                    Quantity = dto.Quantity
                }).ToList();
            }

            var updatedLog = await _mealLogService.UpdateMealLogAsync(
                id, requestingUserId, updateDto.Date, updateDto.MealType, itemsToUpdate);

            var updatedLogDto = MapToDto(updatedLog);
            return Ok(updatedLogDto);
        }

        // DELETE /api/MealLogs/{id} (Deleta um log do usuário logado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "não encontrado" (404) ou "permissão" (403).
            await _mealLogService.DeleteMealLogAsync(id, requestingUserId); // Passa o ID do token
            return NoContent();
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private MealLogDto MapToDto(MealLog log)
        {
            if (log == null) return null;
            return new MealLogDto
            {
                MealLogId = log.MealLogId,
                UserId = log.UserId,
                Date = log.Date,
                MealType = log.MealType,
                Items = log.MealLogItems?.Select(item => new MealItemResponseDto
                {
                    FoodId = item.FoodId,
                    FoodName = item.FoodItem?.Name ?? "Nome não carregado",
                    Quantity = item.Quantity
                }).ToList() ?? new List<MealItemResponseDto>()
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
                throw new Exception("ID de usuário não encontrado no token.");
            }
            return userId;
        }
    }
}