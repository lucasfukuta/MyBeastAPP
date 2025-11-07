using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Entities;
using MyBeast.API.DTOs.FoodItem.Input;   // DTOs de Entrada
using MyBeast.API.DTOs.FoodItem.Output;  // DTOs de Saída
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Adicionado para Select
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using System.Security.Claims; // Para Claims
using System.IdentityModel.Tokens.Jwt; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA TODOS OS ENDPOINTS
    [ApiController]
    [Route("api/[controller]")]
    public class FoodItemsController : ControllerBase
    {
        private readonly IFoodItemService _foodItemService;

        public FoodItemsController(IFoodItemService foodItemService)
        {
            _foodItemService = foodItemService;
        }

        // GET /api/FoodItems (Busca templates + customizados do usuário logado)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FoodItemDto>))]
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItems()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token
            var foodItems = await _foodItemService.GetAllFoodItemsAsync(userId);
            var foodItemDtos = foodItems.Select(MapToDto);
            return Ok(foodItemDtos);
        }

        // GET /api/FoodItems/me (Busca APENAS customizados do usuário logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FoodItemDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Middleware cuidará disso
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetMyCustomFoodItems()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "Usuário não encontrado" (404) do serviço.
            var foodItems = await _foodItemService.GetCustomFoodItemsByUserIdAsync(userId);
            var foodItemDtos = foodItems.Select(MapToDto);
            return Ok(foodItemDtos);
        }

        // GET /api/FoodItems/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FoodItemDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<FoodItemDto>> GetFoodItem(int id)
        {
            var foodItem = await _foodItemService.GetFoodItemByIdAsync(id);
            if (foodItem == null) return NotFound();

            // Verificação de permissão (manual)
            if (foodItem.IsCustom && foodItem.UserId != null)
            {
                var requestingUserId = GetAuthenticatedUserId();
                if (foodItem.UserId != requestingUserId)
                {
                    return Forbid(); // 403 Forbidden
                }
            }

            var foodItemDto = MapToDto(foodItem);
            return Ok(foodItemDto);
        }

        // POST /api/FoodItems (Criar alimento customizado para o usuário logado)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FoodItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // User não encontrado
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Nome duplicado
        public async Task<ActionResult<FoodItemDto>> CreateCustomFoodItem([FromBody] FoodItemCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            var foodItemToCreate = new FoodItem
            {
                Name = createDto.Name,
                Calories = createDto.Calories,
                Protein = createDto.Protein,
                Carbs = createDto.Carbs,
                Fat = createDto.Fat
            };

            var newFoodItem = await _foodItemService.CreateCustomFoodItemAsync(foodItemToCreate, requestingUserId);
            var newFoodItemDto = MapToDto(newFoodItem);
            return CreatedAtAction(nameof(GetFoodItem), new { id = newFoodItemDto.FoodId }, newFoodItemDto);
        }

        // PUT /api/FoodItems/{id} (Atualizar alimento customizado do usuário logado)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FoodItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Nome duplicado
        public async Task<ActionResult<FoodItemDto>> UpdateCustomFoodItem(int id, [FromBody] FoodItemUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            var foodItemUpdateData = new FoodItem
            {
                Name = updateDto.Name ?? "",
                Calories = updateDto.Calories ?? -1,
                Protein = updateDto.Protein ?? -1,
                Carbs = updateDto.Carbs ?? -1,
                Fat = updateDto.Fat ?? -1
            };

            var updatedFoodItem = await _foodItemService.UpdateCustomFoodItemAsync(id, foodItemUpdateData, requestingUserId);
            var updatedFoodItemDto = MapToDto(updatedFoodItem);
            return Ok(updatedFoodItemDto);
        }

        // DELETE /api/FoodItems/{id} (Deletar alimento customizado do usuário logado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomFoodItem(int id)
        {
            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            await _foodItemService.DeleteCustomFoodItemAsync(id, requestingUserId);
            return NoContent();
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private FoodItemDto MapToDto(FoodItem foodItem)
        {
            if (foodItem == null) return null;

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