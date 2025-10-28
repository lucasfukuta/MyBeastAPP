using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.Dtos.Pet; // Importa DTOs
using System;
using System.Threading.Tasks;

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

        // ... (GET /user/{userId}, GET /{id}, POST como antes) ...


        // PUT /api/Pets/user/{userId} (Atualização Geral) - Novo
        [HttpPut("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pet))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pet>> UpdatePetDetails(int userId, [FromBody] PetUpdateDto updateDto)
        {
            if (updateDto == null || (!ModelState.IsValid)) return BadRequest("Dados inválidos.");
            // TODO: Adicionar verificação se o usuário logado == userId

            try
            {
                var updatedPet = await _petService.UpdatePetDetailsAsync(userId, updateDto.Name);
                return Ok(updatedPet);
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


        // PUT /api/Pets/user/{userId}/status (Atualização de Status) - Como antes
        [HttpPut("user/{userId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pet))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pet>> UpdatePetStatus(int userId, [FromBody] PetStatusUpdateDto statusUpdate)
        {
            // ... (Código como antes) ...
            if (statusUpdate == null) return BadRequest("Dados de status inválidos.");
            // TODO: Adicionar verificação se o usuário logado == userId
            try
            {
                var updatedPet = await _petService.UpdatePetStatusAsync(
                    userId, statusUpdate.Health, statusUpdate.Energy, statusUpdate.Hunger, statusUpdate.Status);
                return Ok(updatedPet);
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

        // DELETE /api/Pets/user/{userId} (Novo)
        [HttpDelete("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Para erros gerais
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Se Pet não existe
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
                // Considerar retornar NoContent mesmo se o pet já não existia?
                // Ou NotFound como está agora. Depende da semântica desejada.
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Outros erros
            }
        }
    }
}