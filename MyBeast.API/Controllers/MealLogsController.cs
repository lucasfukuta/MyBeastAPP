using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.Dtos.MealLog; // Importa os DTOs
using System;
using System.Collections.Generic;
using System.Linq; // Para Select
using System.Threading.Tasks;

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota: /api/MealLogs
    public class MealLogsController : ControllerBase
    {
        private readonly IMealLogService _mealLogService;

        public MealLogsController(IMealLogService mealLogService)
        {
            _mealLogService = mealLogService;
        }

        // GET /api/MealLogs/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealLog>))]
        public async Task<ActionResult<IEnumerable<MealLog>>> GetLogsByUser(int userId)
        {
            var logs = await _mealLogService.GetMealLogsByUserIdAsync(userId);
            return Ok(logs);
        }

        // GET /api/MealLogs/user/{userId}/date/{dateString} (Formato: YYYY-MM-DD)
        [HttpGet("user/{userId}/date/{dateString}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealLog>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MealLog>>> GetLogsByUserAndDate(int userId, string dateString)
        {
            if (!DateOnly.TryParse(dateString, out var date))
            {
                return BadRequest("Formato de data inválido. Use YYYY-MM-DD.");
            }
            var logs = await _mealLogService.GetMealLogsByUserIdAndDateAsync(userId, date.ToDateTime(TimeOnly.MinValue));
            return Ok(logs);
        }


        // GET /api/MealLogs/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealLog))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealLog>> GetLogById(int id)
        {
            var log = await _mealLogService.GetMealLogByIdAsync(id); // Já inclui itens
            if (log == null)
            {
                return NotFound();
            }
            return Ok(log);
        }

        // POST /api/MealLogs
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MealLog))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MealLog>> LogMeal([FromBody] LogMealDto logDto)
        {
            if (logDto == null || !ModelState.IsValid) // Verifica validações do DTO
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Mapear DTOs de Item para Modelos MealLogItem
                var items = logDto.Items.Select(dto => new MealLogItem
                {
                    FoodId = dto.FoodId,
                    Quantity = dto.Quantity
                    // MealLogId será definido pelo serviço
                }).ToList();

                var newLog = await _mealLogService.LogMealAsync(logDto.UserId, logDto.Date, logDto.MealType, items);
                // Retorna 201 Created com o log e seus itens
                return CreatedAtAction(nameof(GetLogById), new { id = newLog.MealLogId }, newLog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ex: Usuário/Alimento não encontrado, dados inválidos
            }
        }

        // PUT
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealLog))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealLog>> UpdateMealLog(int id, [FromBody] MealLogUpdateDto updateDto, [FromQuery] int userId) // Temporário userId
        {
            if (updateDto == null || !ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = userId; // Substituir

            try
            {
                // Mapear DTOs de Item para Modelos MealLogItem (se presentes)
                List<MealLogItem>? itemsToUpdate = null;
                if (updateDto.Items != null) // Só mapeia se a lista foi enviada
                {
                    itemsToUpdate = updateDto.Items.Select(dto => new MealLogItem
                    {
                        FoodId = dto.FoodId,
                        Quantity = dto.Quantity
                        // MealLogId será definido pelo serviço/repositório
                    }).ToList();
                }

                var updatedLog = await _mealLogService.UpdateMealLogAsync(
                    id,
                    requestingUserId,
                    updateDto.Date,
                    updateDto.MealType,
                    itemsToUpdate // Passa a lista mapeada (pode ser nula)
                );

                return Ok(updatedLog); // Retorna o MealLog atualizado (com itens)
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message); // MealLog, User ou FoodItem não encontrado
            }
            catch (Exception ex) when (ex.Message.Contains("permissão"))
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (ArgumentException ex) // Captura erros de validação (ex: quantidade inválida)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) // Outros erros
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/MealLogs/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLog(int id)
        {
            try
            {
                await _mealLogService.DeleteMealLogAsync(id);
                return NoContent();
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
    }
}