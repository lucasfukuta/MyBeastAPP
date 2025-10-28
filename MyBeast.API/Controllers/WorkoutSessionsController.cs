using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyBeast.API.Dtos.WorkoutSession;

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota: /api/WorkoutSessions
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly IWorkoutSessionService _sessionService;

        public WorkoutSessionsController(IWorkoutSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // GET /api/WorkoutSessions/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutSession>))]
        public async Task<ActionResult<IEnumerable<WorkoutSession>>> GetSessionsByUser(int userId)
        {
            var sessions = await _sessionService.GetWorkoutSessionsByUserIdAsync(userId);
            return Ok(sessions);
        }

        // GET /api/WorkoutSessions/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutSession))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutSession>> GetSessionById(int id)
        {
            var session = await _sessionService.GetWorkoutSessionByIdAsync(id); // Já inclui SetLogs
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session);
        }

        // POST /api/WorkoutSessions/start
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkoutSession))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkoutSession>> StartSession([FromBody] StartSessionDto startDto)
        {
            if (startDto == null) return BadRequest("Dados inválidos.");
            try
            {
                var newSession = await _sessionService.StartWorkoutSessionAsync(startDto.UserId, startDto.StartTime);
                // Retorna 201 Created com a nova sessão (ainda sem duração/volume)
                return CreatedAtAction(nameof(GetSessionById), new { id = newSession.SessionId }, newSession);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/WorkoutSessions/{id}/end
        [HttpPut("{id}/end")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutSession))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutSession>> EndSession(int id, [FromBody] EndSessionDto endDto)
        {
            if (endDto == null) return BadRequest("Dados inválidos.");
            try
            {
                // Mapeia DTOs de SetLog para Modelos SetLog
                var setLogs = endDto.SetLogs?.Select(dto => new SetLog
                {
                    // SessionId será definido pelo serviço
                    ExerciseId = dto.ExerciseId,
                    SetNumber = dto.SetNumber,
                    Weight = dto.Weight,
                    Reps = dto.Reps,
                    RestTimeSeconds = dto.RestTimeSeconds
                }).ToList() ?? new List<SetLog>(); // Garante que não é nulo

                var updatedSession = await _sessionService.EndWorkoutSessionAsync(id, endDto.EndTime, endDto.TotalVolume, setLogs);
                return Ok(updatedSession);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/WorkoutSessions/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSession(int id)
        {
            try
            {
                await _sessionService.DeleteWorkoutSessionAsync(id);
                return NoContent(); // 204 No Content = Sucesso sem corpo de resposta
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Outros erros
            }
        }
    }
}