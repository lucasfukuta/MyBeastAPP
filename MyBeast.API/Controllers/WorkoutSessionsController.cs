using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyBeast.API.DTOs.WorkoutSessions.Input;  // DTOs de Entrada
using MyBeast.API.DTOs.WorkoutSessions.Output; // DTOs de Saída
using System.Linq; // Para Select
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using System.Security.Claims; // Para Claims
using System.IdentityModel.Tokens.Jwt; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA TODOS OS ENDPOINTS
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly IWorkoutSessionService _sessionService;

        public WorkoutSessionsController(IWorkoutSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // GET /api/WorkoutSessions/me (Busca sessões do usuário logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutSessionDto>))]
        public async Task<ActionResult<IEnumerable<WorkoutSessionDto>>> GetMySessions()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token
            var sessions = await _sessionService.GetWorkoutSessionsByUserIdAsync(userId);
            var sessionDtos = sessions.Select(MapToSummaryDto);
            return Ok(sessionDtos);
        }

        // GET /api/WorkoutSessions/{id} (Busca uma sessão específica)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutSessionDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<WorkoutSessionDetailDto>> GetSessionById(int id)
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará a exceção "permissão" (403).
            var session = await _sessionService.GetWorkoutSessionByIdAsync(id, userId); // Passa ID para verificação de permissão

            // Verificação manual de nulo é mantida
            if (session == null)
            {
                return NotFound();
            }

            var sessionDetailDto = MapToDetailDto(session);
            return Ok(sessionDetailDto);
        }

        // POST /api/WorkoutSessions/start (Inicia sessão para usuário logado)
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkoutSessionDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkoutSessionDetailDto>> StartSession([FromBody] StartSessionDto startDto)
        {
            if (startDto == null || !ModelState.IsValid) return BadRequest("Dados inválidos.");

            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "Usuário não encontrado" (404) ou outros erros (400).
            var newSession = await _sessionService.StartWorkoutSessionAsync(userId, startDto.StartTime); // Passa o ID do token
            var sessionDetailDto = MapToDetailDto(newSession);
            return CreatedAtAction(nameof(GetSessionById), new { id = sessionDetailDto.SessionId }, sessionDetailDto);
        }

        // PUT /api/WorkoutSessions/{id}/end (Finaliza sessão do usuário logado)
        [HttpPut("{id}/end")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutSessionDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutSessionDetailDto>> EndSession(int id, [FromBody] EndSessionDto endDto)
        {
            if (endDto == null || !ModelState.IsValid) return BadRequest("Dados inválidos.");

            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "não encontrada" (404), "permissão" (403) ou outros BQ (400).
            var setLogs = endDto.SetLogs?.Select(dto => new SetLog
            {
                ExerciseId = dto.ExerciseId,
                SetNumber = dto.SetNumber,
                Weight = dto.Weight,
                Reps = dto.Reps,
                RestTimeSeconds = dto.RestTimeSeconds
            }).ToList() ?? new List<SetLog>();

            var updatedSession = await _sessionService.EndWorkoutSessionAsync(id, userId, endDto.EndTime, endDto.TotalVolume, setLogs); // Passa o ID do token

            var sessionWithDetails = await _sessionService.GetWorkoutSessionByIdAsync(id, userId); // Re-busca com permissão
            if (sessionWithDetails == null) return NotFound(); // Verificação de segurança

            var sessionDetailDto = MapToDetailDto(sessionWithDetails);
            return Ok(sessionDetailDto);
        }

        // DELETE /api/WorkoutSessions/{id} (Deleta sessão do usuário logado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "não encontrada" (404) ou "permissão" (403).
            await _sessionService.DeleteWorkoutSessionAsync(id, userId); // Passa o ID do token
            return NoContent();
        }

        // --- MÉTODOS AUXILIARES DE MAPEAMENTO ---
        private WorkoutSessionDto MapToSummaryDto(WorkoutSession s)
        {
            return new WorkoutSessionDto
            {
                SessionId = s.SessionId,
                UserId = s.UserId,
                Date = s.Date,
                DurationMinutes = s.DurationMinutes,
                TotalVolume = s.TotalVolume,
                SetCount = s.SetLogs?.Count ?? 0
            };
        }

        private WorkoutSessionDetailDto MapToDetailDto(WorkoutSession session)
        {
            if (session == null) return null; // Segurança
            return new WorkoutSessionDetailDto
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                Date = session.Date,
                DurationMinutes = session.DurationMinutes,
                TotalVolume = session.TotalVolume,
                SetLogs = session.SetLogs?.Select(sl => new SetLogResponseDto
                {
                    SetLogId = sl.SetLogId,
                    ExerciseId = sl.ExerciseId,
                    ExerciseName = sl.Exercise?.Name ?? "Nome não carregado",
                    SetNumber = sl.SetNumber,
                    Weight = sl.Weight,
                    Reps = sl.Reps,
                    RestTimeSeconds = sl.RestTimeSeconds
                }).ToList() ?? new List<SetLogResponseDto>()
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