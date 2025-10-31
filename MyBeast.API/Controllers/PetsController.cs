using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using MyBeast.API.DTOs.Pets.Input;
using MyBeast.API.DTOs.Pets.Output; // Necessário para Select se usarmos listas

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        // GET /api/Pets/user/{userId} - Retorna PetDto
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> GetPetByUserId(int userId)
        {
            var pet = await _petService.GetPetByUserIdAsync(userId);
            if (pet == null)
            {
                return NotFound($"Pet não encontrado para o usuário {userId}.");
            }
            // Mapear Model para Dto
            var petDto = MapToDto(pet);
            return Ok(petDto);
        }

        // GET /api/Pets/{id} - Retorna PetDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> GetPetById(int id)
        {
            var pet = await _petService.GetPetByIdAsync(id);
            if (pet == null)
            {
                return NotFound($"Pet com ID {id} não encontrado.");
            }
            // Mapear Model para Dto
            var petDto = MapToDto(pet);
            return Ok(petDto);
        }

        // POST /api/Pets - Retorna PetDto
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PetDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PetDto>> CreatePet([FromBody] PetCreateDto petDto)
        {
            if (petDto == null || !ModelState.IsValid) return BadRequest("Dados do pet inválidos.");

            try
            {
                var petToCreate = new Pet
                {
                    UserId = petDto.UserId,
                    Name = petDto.Name
                };

                var newPet = await _petService.CreatePetAsync(petToCreate);
                // Mapear Model para Dto
                var newPetDto = MapToDto(newPet);
                // Retorna 201 Created com o link e o DTO criado
                return CreatedAtAction(nameof(GetPetById), new { id = newPetDto.PetId }, newPetDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/Pets/user/{userId} (Atualização Geral) - Retorna PetDto
        [HttpPut("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> UpdatePetDetails(int userId, [FromBody] PetUpdateDto updateDto)
        {
            if (updateDto == null || (!ModelState.IsValid)) return BadRequest("Dados inválidos.");
            // TODO: Adicionar verificação se o usuário logado == userId

            try
            {
                var updatedPet = await _petService.UpdatePetDetailsAsync(userId, updateDto.Name);
                // Mapear Model para Dto
                var updatedPetDto = MapToDto(updatedPet);
                return Ok(updatedPetDto);
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

        // PUT /api/Pets/user/{userId}/status (Atualização de Status) - Retorna PetDto
        [HttpPut("user/{userId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PetDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> UpdatePetStatus(int userId, [FromBody] PetStatusUpdateDto statusUpdate)
        {
            if (statusUpdate == null || !ModelState.IsValid) return BadRequest("Dados de status inválidos.");
            // TODO: Adicionar verificação se o usuário logado == userId
            try
            {
                var updatedPet = await _petService.UpdatePetStatusAsync(
                    userId, statusUpdate.Health, statusUpdate.Energy, statusUpdate.Hunger, statusUpdate.Status);
                // Mapear Model para Dto
                var updatedPetDto = MapToDto(updatedPet);
                return Ok(updatedPetDto);
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

        // DELETE /api/Pets/user/{userId} - Sem mudança, retorna NoContent
        [HttpDelete("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePet(int userId)
        {
            // TODO: Adicionar verificação se o usuário logado == userId
            try
            {
                await _petService.DeletePetByUserIdAsync(userId);
                return NoContent(); // Sucesso
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Outros erros
            }
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private PetDto MapToDto(Pet pet)
        {
            if (pet == null) return null; // Segurança extra

            return new PetDto
            {
                PetId = pet.PetId,
                UserId = pet.UserId,
                Name = pet.Name,
                EvolutionLevel = pet.EvolutionLevel,
                Health = pet.Health,
                Energy = pet.Energy,
                Hunger = pet.Hunger,
                Status = pet.Status
            };
        }
    }
}