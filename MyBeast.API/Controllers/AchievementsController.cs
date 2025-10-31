using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MyBeast.API.DTOs.Achievement.Output; // Adicionado para Select

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        // GET /api/Achievements/user/{userId} - Retorna Lista de AchievementDto
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AchievementDto>))] // Atualizado
        public async Task<ActionResult<IEnumerable<AchievementDto>>> GetAchievementsByUser(int userId)
        {
            // TODO: Adicionar verificação se o usuário logado pode ver as conquistas deste userId
            var achievements = await _achievementService.GetAchievementsByUserIdAsync(userId);

            // Mapear Model para Dto
            var achievementDtos = achievements.Select(MapToDto); // Usa método auxiliar

            return Ok(achievementDtos);
        }

        // GET /api/Achievements/{id} - Retorna AchievementDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AchievementDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AchievementDto>> GetAchievementById(int id)
        {
            var achievement = await _achievementService.GetAchievementByIdAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            // Mapear Model para Dto
            var achievementDto = MapToDto(achievement);
            return Ok(achievementDto);
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private AchievementDto MapToDto(Achievement achievement)
        {
            if (achievement == null) return null; // Segurança

            return new AchievementDto
            {
                AchievementId = achievement.AchievementId,
                UserId = achievement.UserId,
                Name = achievement.Name,
                Description = achievement.Description ?? string.Empty, // Garante string vazia se Description for nulo
                DateAchieved = achievement.DateAchieved
            };
        }
    }
}