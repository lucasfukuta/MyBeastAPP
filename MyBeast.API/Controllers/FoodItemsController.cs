using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MyBeast.API.DTOs.FoodItem.Input;
using MyBeast.API.DTOs.FoodItem.Output; // Adicionado para Select

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodItemsController : ControllerBase
    {
        private readonly IFoodItemService _foodItemService;

        public FoodItemsController(IFoodItemService foodItemService)
        {
            _foodItemService = foodItemService;
        }

        // GET /api/FoodItems - Retorna Lista de FoodItemDto
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FoodItemDto>))] // Atualizado
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItems([FromQuery] int? userId = null)
        {
            // TODO: Se userId != null, verificar permissão
            var foodItems = await _foodItemService.GetAllFoodItemsAsync(userId);

            // Mapear Model para Dto
            var foodItemDtos = foodItems.Select(MapToDto); // Usa método auxiliar

            return Ok(foodItemDtos);
        }

        // GET /api/FoodItems/user/{userId} - Retorna Lista de FoodItemDto
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FoodItemDto>))] // Atualizado
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetUserFoodItems(int userId)
        {
            // TODO: Verificar permissão
            try
            {
                var foodItems = await _foodItemService.GetCustomFoodItemsByUserIdAsync(userId);
                // Mapear Model para Dto
                var foodItemDtos = foodItems.Select(MapToDto);
                return Ok(foodItemDtos);
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

        // GET /api/FoodItems/{id} - Retorna FoodItemDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FoodItemDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FoodItemDto>> GetFoodItem(int id)
        {
            // TODO: Verificar permissão se for custom de outro user
            var foodItem = await _foodItemService.GetFoodItemByIdAsync(id);
            if (foodItem == null) return NotFound();

            // Mapear Model para Dto
            var foodItemDto = MapToDto(foodItem);
            return Ok(foodItemDto);
        }

        // POST /api/FoodItems - Retorna FoodItemDto
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FoodItemDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FoodItemDto>> CreateCustomFoodItem([FromBody] FoodItemCreateDto createDto) // DTO de Entrada
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = createDto.UserId;

            try
            {
                var foodItemToCreate = new FoodItem
                {
                    Name = createDto.Name,
                    Calories = createDto.Calories,
                    Protein = createDto.Protein,
                    Carbs = createDto.Carbs,
                    Fat = createDto.Fat
                };

                var newFoodItem = await _foodItemService.CreateCustomFoodItemAsync(foodItemToCreate, requestingUserId);
                // Mapear Model para Dto
                var newFoodItemDto = MapToDto(newFoodItem);
                return CreatedAtAction(nameof(GetFoodItem), new { id = newFoodItemDto.FoodId }, newFoodItemDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/FoodItems/{id} - Retorna FoodItemDto
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FoodItemDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FoodItemDto>> UpdateCustomFoodItem(int id, [FromBody] FoodItemUpdateDto updateDto, [FromQuery] int userId) // DTO de Entrada
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                var foodItemUpdateData = new FoodItem
                {
                    Name = updateDto.Name ?? "",
                    Calories = updateDto.Calories ?? -1,
                    Protein = updateDto.Protein ?? -1,
                    Carbs = updateDto.Carbs ?? -1,
                    Fat = updateDto.Fat ?? -1
                };

                var updatedFoodItem = await _foodItemService.UpdateCustomFoodItemAsync(id, foodItemUpdateData, requestingUserId);
                // Mapear Model para Dto
                var updatedFoodItemDto = MapToDto(updatedFoodItem);
                return Ok(updatedFoodItemDto);
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE /api/FoodItems/{id} - Sem mudança, retorna NoContent
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomFoodItem(int id, [FromQuery] int userId) // Temporário userId
        {
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                await _foodItemService.DeleteCustomFoodItemAsync(id, requestingUserId);
                return NoContent();
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private FoodItemDto MapToDto(FoodItem foodItem)
        {
            if (foodItem == null) return null; // Segurança

            return new FoodItemDto
            {
                FoodId = foodItem.FoodId,
                Name = foodItem.Name,
                Calories = foodItem.Calories,
                Protein = foodItem.Protein,
                Carbs = foodItem.Carbs,
                Fat = foodItem.Fat,
                IsCustom = foodItem.IsCustom,
                UserId = foodItem.UserId
            };
        }
    }
}