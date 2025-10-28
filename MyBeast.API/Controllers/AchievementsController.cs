using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
// using MyBeast.API.Dtos.Achievement; // Não precisamos mais do DTO aqui
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota: /api/Achievements
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        // GET /api/Achievements/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Achievement>))]
        public async Task<ActionResult<IEnumerable<Achievement>>> GetAchievementsByUser(int userId)
        {
            // TODO: Adicionar verificação se o usuário logado pode ver as conquistas deste userId
            var achievements = await _achievementService.GetAchievementsByUserIdAsync(userId);
            return Ok(achievements);
        }

        // GET /api/Achievements/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Achievement))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Achievement>> GetAchievementById(int id)
        {
            var achievement = await _achievementService.GetAchievementByIdAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }
            return Ok(achievement);
        }

        // --- MÉTODO GrantAchievement REMOVIDO DAQUI ---

    }
}