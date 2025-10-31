using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq; // Adicionado para Select
using System.Threading.Tasks;
using MyBeast.API.DTOs.MealLog.Output;
using MyBeast.API.DTOs.MealLog.Input;

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealLogsController : ControllerBase
    {
        private readonly IMealLogService _mealLogService;

        public MealLogsController(IMealLogService mealLogService)
        {
            _mealLogService = mealLogService;
        }

        // GET /api/MealLogs/user/{userId} - Retorna Lista de MealLogDto
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealLogDto>))] // Atualizado
        public async Task<ActionResult<IEnumerable<MealLogDto>>> GetLogsByUser(int userId)
        {
            // O serviço/repositório já deve incluir Itens e FoodItem
            var logs = await _mealLogService.GetMealLogsByUserIdAsync(userId);

            // Mapear Model para Dto
            var logDtos = logs.Select(MapToDto); // Usa método auxiliar

            return Ok(logDtos);
        }

        // GET /api/MealLogs/user/{userId}/date/{dateString} - Retorna Lista de MealLogDto
        [HttpGet("user/{userId}/date/{dateString}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealLogDto>))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MealLogDto>>> GetLogsByUserAndDate(int userId, string dateString)
        {
            if (!DateOnly.TryParse(dateString, out var date))
            {
                return BadRequest("Formato de data inválido. Use YYYY-MM-DD.");
            }
            // Serviço/Repositório já inclui Itens e FoodItem
            var logs = await _mealLogService.GetMealLogsByUserIdAndDateAsync(userId, date.ToDateTime(TimeOnly.MinValue));

            // Mapear Model para Dto
            var logDtos = logs.Select(MapToDto);

            return Ok(logDtos);
        }


        // GET /api/MealLogs/{id} - Retorna MealLogDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealLogDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealLogDto>> GetLogById(int id)
        {
            // Serviço/Repositório já inclui Itens e FoodItem
            var log = await _mealLogService.GetMealLogByIdAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            // Mapear Model para Dto
            var logDto = MapToDto(log);
            return Ok(logDto);
        }

        // POST /api/MealLogs - Retorna MealLogDto
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MealLogDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MealLogDto>> LogMeal([FromBody] LogMealDto logDto) // DTO de Entrada
        {
            if (logDto == null || !ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var items = logDto.Items.Select(dto => new MealLogItem
                {
                    FoodId = dto.FoodId,
                    Quantity = dto.Quantity
                }).ToList();

                var newLog = await _mealLogService.LogMealAsync(logDto.UserId, logDto.Date, logDto.MealType, items);

                // Re-buscar para garantir includes corretos para o DTO de resposta
                var createdLogWithDetails = await _mealLogService.GetMealLogByIdAsync(newLog.MealLogId);
                if (createdLogWithDetails == null) return BadRequest("Erro ao buscar log criado."); // Segurança

                // Mapear para DTO de resposta
                var newLogDto = MapToDto(createdLogWithDetails);

                return CreatedAtAction(nameof(GetLogById), new { id = newLogDto.MealLogId }, newLogDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/MealLogs/{id} - Retorna MealLogDto
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealLogDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealLogDto>> UpdateMealLog(int id, [FromBody] MealLogUpdateDto updateDto, [FromQuery] int userId) // DTO de Entrada
        {
            if (updateDto == null || !ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
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
                    id, requestingUserId, updateDto.Date, updateDto.MealType, itemsToUpdate
                );

                // Mapear para DTO de resposta
                var updatedLogDto = MapToDto(updatedLog);

                return Ok(updatedLogDto);
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão")) { return Forbid(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE /api/MealLogs/{id} - Sem mudança, retorna NoContent
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Adicionado
        public async Task<IActionResult> DeleteLog(int id)
        {
            // TODO: Adicionar verificação de permissão
            try
            {
                await _mealLogService.DeleteMealLogAsync(id);
                return NoContent();
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private MealLogDto MapToDto(MealLog log)
        {
            if (log == null) return null; // Segurança

            return new MealLogDto
            {
                MealLogId = log.MealLogId,
                UserId = log.UserId,
                Date = log.Date,
                MealType = log.MealType,
                Items = log.MealLogItems?.Select(item => new MealItemResponseDto
                {
                    FoodId = item.FoodId,
                    FoodName = item.FoodItem?.Name ?? "Nome não carregado", // Pega nome do FoodItem incluído
                    Quantity = item.Quantity
                    // Calcular macros aqui se desejar
                }).ToList() ?? new List<MealItemResponseDto>()
                // Calcular totais aqui se desejar
            };
        }
    }
}