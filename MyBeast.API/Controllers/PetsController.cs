using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.DTOs.Pets.Input;
using MyBeast.API.DTOs.Pets.Output;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MyBeast.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        // GET /api/Pets/my-pet
        [HttpGet("my-pet")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> GetMyPet()
        {
            var userId = GetAuthenticatedUserId();
            var pet = await _petService.GetPetByUserIdAsync(userId);

            // O serviço retorna null se não encontrar, não lança exceção.
            // Portanto, esta verificação manual de NotFound é correta.
            if (pet == null)
            {
                return NotFound($"Pet não encontrado para o usuário {userId}.");
            }
            var petDto = MapToDto(pet);
            return Ok(petDto);
        }

        // GET /api/Pets/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> GetPetById(int id)
        {
            var pet = await _petService.GetPetByIdAsync(id);
            if (pet == null)
            {
                return NotFound($"Pet com ID {id} não encontrado.");
            }
            // (Verificação de permissão pode ser adicionada aqui se necessário)
            var petDto = MapToDto(pet);
            return Ok(petDto);
        }

        // POST /api/Pets
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PetDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PetDto>> CreatePet([FromBody] PetCreateDto petDto)
        {
            if (petDto == null || !ModelState.IsValid) return BadRequest("Dados do pet inválidos.");

            var userId = GetAuthenticatedUserId();

            // O try-catch foi REMOVIDO.
            // Se _petService.CreatePetAsync lançar uma exceção (ex: "Usuário já possui um Pet"),
            // o ErrorHandlingMiddleware irá capturá-la e retornar um 400 Bad Request.
            var petToCreate = new Pet { UserId = userId, Name = petDto.Name };
            var newPet = await _petService.CreatePetAsync(petToCreate);

            var newPetDto = MapToDto(newPet);
            return CreatedAtAction(nameof(GetPetById), new { id = newPetDto.PetId }, newPetDto);
        }

        // PUT /api/Pets/my-pet
        [HttpPut("my-pet")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> UpdatePetDetails([FromBody] PetUpdateDto updateDto)
        {
            if (updateDto == null || (!ModelState.IsValid)) return BadRequest("Dados inválidos.");

            var userId = GetAuthenticatedUserId();

            // O try-catch foi REMOVIDO.
            // Se _petService.UpdatePetDetailsAsync lançar "não encontrado",
            // o middleware retornará 404.
            var updatedPet = await _petService.UpdatePetDetailsAsync(userId, updateDto.Name);
            var updatedPetDto = MapToDto(updatedPet);
            return Ok(updatedPetDto);
        }

        // PUT /api/Pets/my-pet/status
        [HttpPut("my-pet/status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> UpdatePetStatus([FromBody] PetStatusUpdateDto statusUpdate)
        {
            if (statusUpdate == null || !ModelState.IsValid) return BadRequest("Dados de status inválidos.");

            var userId = GetAuthenticatedUserId();

            // O try-catch foi REMOVIDO.
            var updatedPet = await _petService.UpdatePetStatusAsync(
                userId, statusUpdate.Health, statusUpdate.Energy, statusUpdate.Hunger, statusUpdate.Status);
            var updatedPetDto = MapToDto(updatedPet);
            return Ok(updatedPetDto);
        }

        // DELETE /api/Pets/my-pet
        [HttpDelete("my-pet")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePet()
        {
            var userId = GetAuthenticatedUserId();

            // O try-catch foi REMOVIDO.
            await _petService.DeletePetByUserIdAsync(userId);
            return NoContent(); // Sucesso
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private PetDto MapToDto(Pet pet)
        {
            if (pet == null) return null;

            return new PetDto
            {
                PetId = pet.PetId,
                UserId = pet.UserId,
                Name = pet.Name,
                EvolutionLevel = pet.EvolutionLevel,
                Experience = pet.Experience,
                XpToNextLevel = pet.XpToNextLevel,
                Health = pet.Health,
                Energy = pet.Energy,
                Hunger = pet.Hunger,
                Status = pet.Status
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
                throw new Exception("ID de usuário não encontrado no token."); // O middleware vai pegar isso
            }
            return userId;
        }
    }
}