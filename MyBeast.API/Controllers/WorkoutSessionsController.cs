using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MyBeast.API.DTOs.WorkoutSessions.Input;
using MyBeast.API.DTOs.WorkoutSessions.Output; // Adicionado para Select

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly IWorkoutSessionService _sessionService;

        public WorkoutSessionsController(IWorkoutSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // GET /api/WorkoutSessions/user/{userId} - Retorna Lista de WorkoutSessionDto
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutSessionDto>))]
        public async Task<ActionResult<IEnumerable<WorkoutSessionDto>>> GetSessionsByUser(int userId)
        {
            var sessions = await _sessionService.GetWorkoutSessionsByUserIdAsync(userId);

            // Mapear Model para Dto (resumido)
            var sessionDtos = sessions.Select(s => new WorkoutSessionDto
            {
                SessionId = s.SessionId,
                UserId = s.UserId,
                Date = s.Date,
                DurationMinutes = s.DurationMinutes,
                TotalVolume = s.TotalVolume,
                SetCount = s.SetLogs?.Count ?? 0 // Conta os sets (se carregados)
            });

            return Ok(sessionDtos);
        }

        // GET /api/WorkoutSessions/{id} - Retorna WorkoutSessionDetailDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutSessionDetailDto))] // Tipo de resposta atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutSessionDetailDto>> GetSessionById(int id)
        {
            // O serviço/repositório já inclui SetLogs e Exercise
            var session = await _sessionService.GetWorkoutSessionByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            // Mapear Model para Dto Detalhado
            var sessionDetailDto = new WorkoutSessionDetailDto
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                Date = session.Date,
                DurationMinutes = session.DurationMinutes,
                TotalVolume = session.TotalVolume,
                SetLogs = session.SetLogs?.Select(sl => new SetLogResponseDto // Mapeia cada SetLog
                {
                    SetLogId = sl.SetLogId,
                    ExerciseId = sl.ExerciseId,
                    ExerciseName = sl.Exercise?.Name ?? "Exercício não encontrado", // Pega o nome do exercício incluído
                    SetNumber = sl.SetNumber,
                    Weight = sl.Weight,
                    Reps = sl.Reps,
                    RestTimeSeconds = sl.RestTimeSeconds
                }).ToList() ?? new List<SetLogResponseDto>() // Garante lista vazia se não houver sets
            };

            return Ok(sessionDetailDto);
        }

        // POST /api/WorkoutSessions/start - Retorna WorkoutSessionDetailDto (inicial)
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkoutSessionDetailDto))] // Tipo de resposta atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkoutSessionDetailDto>> StartSession([FromBody] StartSessionDto startDto)
        {
            if (startDto == null || !ModelState.IsValid) return BadRequest("Dados inválidos.");
            try
            {
                var newSession = await _sessionService.StartWorkoutSessionAsync(startDto.UserId, startDto.StartTime);

                // Mapear Model para Dto Detalhado (sem sets ainda)
                var sessionDetailDto = new WorkoutSessionDetailDto
                {
                    SessionId = newSession.SessionId,
                    UserId = newSession.UserId,
                    Date = newSession.Date,
                    DurationMinutes = newSession.DurationMinutes,
                    TotalVolume = newSession.TotalVolume,
                    SetLogs = new List<SetLogResponseDto>() // Lista vazia inicialmente
                };

                // Retorna 201 Created com a nova sessão (mapeada para DTO)
                return CreatedAtAction(nameof(GetSessionById), new { id = sessionDetailDto.SessionId }, sessionDetailDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/WorkoutSessions/{id}/end - Retorna WorkoutSessionDetailDto (final)
        [HttpPut("{id}/end")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutSessionDetailDto))] // Tipo de resposta atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutSessionDetailDto>> EndSession(int id, [FromBody] EndSessionDto endDto)
        {
            if (endDto == null || !ModelState.IsValid) return BadRequest("Dados inválidos.");
            try
            {
                // Mapeia DTOs de SetLog de entrada para Modelos SetLog
                var setLogs = endDto.SetLogs?.Select(dto => new SetLog
                {
                    ExerciseId = dto.ExerciseId,
                    SetNumber = dto.SetNumber,
                    Weight = dto.Weight,
                    Reps = dto.Reps,
                    RestTimeSeconds = dto.RestTimeSeconds
                    // SessionId será definido pelo serviço
                }).ToList() ?? new List<SetLog>();

                var updatedSession = await _sessionService.EndWorkoutSessionAsync(id, endDto.EndTime, endDto.TotalVolume, setLogs);

                // Re-buscar a sessão para garantir que os includes (Exercícios nos SetLogs) estejam presentes
                var sessionWithDetails = await _sessionService.GetWorkoutSessionByIdAsync(id);
                if (sessionWithDetails == null) return NotFound(); // Segurança extra

                // Mapear Model para Dto Detalhado
                var sessionDetailDto = new WorkoutSessionDetailDto
                {
                    SessionId = sessionWithDetails.SessionId,
                    UserId = sessionWithDetails.UserId,
                    Date = sessionWithDetails.Date,
                    DurationMinutes = sessionWithDetails.DurationMinutes,
                    TotalVolume = sessionWithDetails.TotalVolume,
                    SetLogs = sessionWithDetails.SetLogs?.Select(sl => new SetLogResponseDto
                    {
                        SetLogId = sl.SetLogId,
                        ExerciseId = sl.ExerciseId,
                        ExerciseName = sl.Exercise?.Name ?? "Exercício não encontrado",
                        SetNumber = sl.SetNumber,
                        Weight = sl.Weight,
                        Reps = sl.Reps,
                        RestTimeSeconds = sl.RestTimeSeconds
                    }).ToList() ?? new List<SetLogResponseDto>()
                };

                return Ok(sessionDetailDto);
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

        // DELETE /api/WorkoutSessions/{id} - Sem mudança, retorna NoContent
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Adicionado para outros erros
        public async Task<IActionResult> DeleteSession(int id)
        {
            // TODO: Adicionar verificação de permissão
            try
            {
                await _sessionService.DeleteWorkoutSessionAsync(id);
                return NoContent();
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
    }
}