using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.API.DTOs.Achievement.Output; // Importa o DTO de Saída
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para o .Select()
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using System.Security.Claims; // Para Claims
using System.IdentityModel.Tokens.Jwt;
using MyBeast.Domain.Entities; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA TODOS OS ENDPOINTS
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        // GET /api/Achievements/me (Busca as conquistas do usuário logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AchievementDto>))]
        public async Task<ActionResult<IEnumerable<AchievementDto>>> GetMyAchievements()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            var achievements = await _achievementService.GetAchievementsByUserIdAsync(userId);

            // Mapear Model para Dto
            var achievementDtos = achievements.Select(MapToDto);
            return Ok(achievementDtos);
        }

        // GET /api/Achievements/{id} (Busca uma conquista específica por ID)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AchievementDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Adicionado para permissão
        public async Task<ActionResult<AchievementDto>> GetAchievementById(int id)
        {
            var achievement = await _achievementService.GetAchievementByIdAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            // --- VERIFICAÇÃO DE AUTORIZAÇÃO ---
            // Garante que o usuário logado só pode ver suas próprias conquistas
            var userId = GetAuthenticatedUserId();
            if (achievement.UserId != userId)
            {
                // (A menos que seja um Admin, mas não implementamos isso ainda)
                return Forbid(); // Retorna 403 Forbidden
            }
            // --- FIM DA VERIFICAÇÃO ---

            // Mapear Model para Dto
            var achievementDto = MapToDto(achievement);
            return Ok(achievementDto);
        }


        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private AchievementDto MapToDto(Achievement achievement)
        {
            if (achievement == null) return null;

            return new AchievementDto
            {
                AchievementId = achievement.AchievementId,
                UserId = achievement.UserId,
                Name = achievement.Name,
                Description = achievement.Description ?? string.Empty,
                DateAchieved = achievement.DateAchieved
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
                // Este erro só acontece se [Authorize] estiver ausente ou o token for inválido
                throw new Exception("ID de usuário não encontrado no token.");
            }
            return userId;
        }
    }
}