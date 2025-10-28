using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.Dtos.FoodItem; // Importa DTOs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // GET /api/FoodItems (Pode filtrar por usuário ou retornar só templates)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItems([FromQuery] int? userId = null)
        {
            // TODO: Se userId != null, verificar permissão
            var foodItems = await _foodItemService.GetAllFoodItemsAsync(userId);
            // TODO: Mapear para FoodItemDto?
            return Ok(foodItems);
        }

        // GET /api/FoodItems/user/{userId} (Busca customizados de um usuário)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetUserFoodItems(int userId)
        {
            // TODO: Verificar permissão
            try
            {
                var foodItems = await _foodItemService.GetCustomFoodItemsByUserIdAsync(userId);
                // TODO: Mapear para FoodItemDto
                return Ok(foodItems);
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

        // GET /api/FoodItems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItem>> GetFoodItem(int id)
        {
            // TODO: Verificar permissão se for custom de outro user
            var foodItem = await _foodItemService.GetFoodItemByIdAsync(id);
            if (foodItem == null) return NotFound();
            // TODO: Mapear para FoodItemDto
            return Ok(foodItem);
        }

        // POST /api/FoodItems (Criar alimento customizado)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FoodItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FoodItem>> CreateCustomFoodItem([FromBody] FoodItemCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = createDto.UserId; // Substituir

            try
            {
                var foodItemToCreate = new FoodItem
                {
                    Name = createDto.Name,
                    Calories = createDto.Calories,
                    Protein = createDto.Protein,
                    Carbs = createDto.Carbs,
                    Fat = createDto.Fat
                    // UserId e IsCustom serão definidos pelo serviço
                };

                var newFoodItem = await _foodItemService.CreateCustomFoodItemAsync(foodItemToCreate, requestingUserId);
                // TODO: Mapear para FoodItemDto
                return CreatedAtAction(nameof(GetFoodItem), new { id = newFoodItem.FoodId }, newFoodItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ex: Nome duplicado, User não encontrado
            }
        }

        // PUT /api/FoodItems/{id} (Atualizar alimento customizado)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FoodItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FoodItem>> UpdateCustomFoodItem(int id, [FromBody] FoodItemUpdateDto updateDto, [FromQuery] int userId) // Temporário userId
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = userId; // Substituir

            try
            {
                // Mapear apenas os campos que podem ser atualizados
                var foodItemUpdateData = new FoodItem
                {
                    Name = updateDto.Name ?? "", // Serviço ignora se vazio
                    Calories = updateDto.Calories ?? -1, // Usa -1 para indicar "não atualizar"
                    Protein = updateDto.Protein ?? -1,
                    Carbs = updateDto.Carbs ?? -1,
                    Fat = updateDto.Fat ?? -1
                };


                var updatedFoodItem = await _foodItemService.UpdateCustomFoodItemAsync(id, foodItemUpdateData, requestingUserId);
                // TODO: Mapear para FoodItemDto
                return Ok(updatedFoodItem);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template"))
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ex: Nome duplicado
            }
        }

        // DELETE /api/FoodItems/{id} (Deletar alimento customizado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomFoodItem(int id, [FromQuery] int userId) // Temporário userId
        {
            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = userId; // Substituir

            try
            {
                await _foodItemService.DeleteCustomFoodItemAsync(id, requestingUserId);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template"))
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (Exception ex)
            {
                // Pode ser erro 500 se o item estiver em uso e a FK impedir
                return BadRequest(ex.Message);
            }
        }
    }
}