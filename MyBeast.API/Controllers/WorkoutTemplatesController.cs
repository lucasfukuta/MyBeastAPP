using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq; // Para Select
using System.Threading.Tasks;
using MyBeast.API.DTOs.WorkoutTemplate.Input;
using MyBeast.API.DTOs.WorkoutTemplate.Output;

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutTemplatesController : ControllerBase
    {
        private readonly IWorkoutTemplateService _templateService;

        public WorkoutTemplatesController(IWorkoutTemplateService templateService)
        {
            _templateService = templateService;
        }

        // GET /api/WorkoutTemplates - Retorna Lista de WorkoutTemplateDto
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutTemplateDto>))]
        public async Task<ActionResult<IEnumerable<WorkoutTemplateDto>>> GetWorkoutTemplates()
        {
            // Não inclui exercícios por padrão no serviço/repositório
            var templates = await _templateService.GetAllWorkoutTemplatesAsync();

            // Mapear Model para Dto (resumido)
            var templateDtos = templates.Select(t => new WorkoutTemplateDto
            {
                TemplateId = t.TemplateId,
                UserId = t.UserId,
                Name = t.Name,
                Difficulty = t.Difficulty,
                IsPremium = t.IsPremium,
                ExerciseCount = t.TemplateExercises?.Count ?? 0 // Conta exercícios (se carregados, senão 0)
            });
            return Ok(templateDtos);
        }

        // GET /api/WorkoutTemplates/defaults - Retorna Lista de WorkoutTemplateDto ou DetailDto
        [HttpGet("defaults")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutTemplateDto>))] // Ou DetailDto se includeExercises=true
        public async Task<ActionResult<IEnumerable<object>>> GetDefaultWorkoutTemplates([FromQuery] bool includeExercises = false)
        {
            var templates = await _templateService.GetDefaultWorkoutTemplatesAsync(includeExercises);

            if (includeExercises)
            {
                // Mapear para Dto Detalhado
                var detailDtos = templates.Select(MapToDetailDto);
                return Ok(detailDtos);
            }
            else
            {
                // Mapear para Dto Resumido
                var summaryDtos = templates.Select(MapToSummaryDto);
                return Ok(summaryDtos);
            }
        }

        // GET /api/WorkoutTemplates/user/{userId} - Retorna Lista de WorkoutTemplateDto ou DetailDto
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutTemplateDto>))] // Ou DetailDto se includeExercises=true
        public async Task<ActionResult<IEnumerable<object>>> GetWorkoutTemplatesByUser(int userId, [FromQuery] bool includeExercises = false)
        {
            try
            {
                // TODO: Verificar permissão
                var templates = await _templateService.GetWorkoutTemplatesByUserIdAsync(userId, includeExercises);

                if (includeExercises)
                {
                    var detailDtos = templates.Select(MapToDetailDto);
                    return Ok(detailDtos);
                }
                else
                {
                    var summaryDtos = templates.Select(MapToSummaryDto);
                    return Ok(summaryDtos);
                }
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }


        // GET /api/WorkoutTemplates/{id} - Retorna WorkoutTemplateDetailDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutTemplateDetailDto))] // Tipo de resposta atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutTemplateDetailDto>> GetWorkoutTemplate(int id, [FromQuery] bool includeExercises = true) // Inclui por padrão
        {
            // Serviço/Repositório busca com includes se includeExercises = true
            var template = await _templateService.GetWorkoutTemplateByIdAsync(id, includeExercises);
            if (template == null)
            {
                return NotFound();
            }

            // Mapear Model para Dto Detalhado
            var detailDto = MapToDetailDto(template);
            return Ok(detailDto);
        }

        // POST /api/WorkoutTemplates - Retorna WorkoutTemplateDetailDto
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkoutTemplateDetailDto))] // Tipo de resposta atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkoutTemplateDetailDto>> CreateWorkoutTemplate([FromBody] WorkoutTemplateCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // TODO: Obter userId autenticado
                int requestingUserId = createDto.UserId;

                var templateToCreate = new WorkoutTemplate
                {
                    UserId = requestingUserId, // Usar ID autenticado
                    Name = createDto.Name,
                    Difficulty = createDto.Difficulty,
                    TemplateExercises = createDto.Exercises?.Select(e => new TemplateExercise
                    {
                        ExerciseId = e.ExerciseId,
                        OrderIndex = e.OrderIndex
                    }).ToList() ?? new List<TemplateExercise>()
                };

                var newTemplate = await _templateService.CreateWorkoutTemplateAsync(templateToCreate);

                // Re-buscar com includes para retornar o DTO completo
                var createdTemplateWithDetails = await _templateService.GetWorkoutTemplateByIdAsync(newTemplate.TemplateId, true);
                if (createdTemplateWithDetails == null) return BadRequest("Erro ao buscar template criado."); // Segurança

                var detailDto = MapToDetailDto(createdTemplateWithDetails);
                return CreatedAtAction(nameof(GetWorkoutTemplate), new { id = detailDto.TemplateId }, detailDto);
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // PUT /api/WorkoutTemplates/{id} - Retorna WorkoutTemplateDetailDto
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutTemplateDetailDto))] // Tipo de resposta atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutTemplateDetailDto>> UpdateWorkoutTemplate(int id, [FromBody] WorkoutTemplateUpdateDto updateDto, [FromQuery] int userId) // Temporário userId
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                var templateUpdateData = new WorkoutTemplate
                {
                    Name = updateDto.Name ?? "",
                    Difficulty = updateDto.Difficulty,
                    TemplateExercises = updateDto.Exercises?.Select(e => new TemplateExercise
                    {
                        ExerciseId = e.ExerciseId,
                        OrderIndex = e.OrderIndex
                    }).ToList() // Pode ser nulo
                };

                var updatedTemplate = await _templateService.UpdateWorkoutTemplateAsync(id, templateUpdateData, requestingUserId);

                // Re-buscar com includes para retornar o DTO completo
                var updatedTemplateWithDetails = await _templateService.GetWorkoutTemplateByIdAsync(id, true);
                if (updatedTemplateWithDetails == null) return NotFound(); // Segurança

                var detailDto = MapToDetailDto(updatedTemplateWithDetails);
                return Ok(detailDto);
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("editar templates padrão")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE /api/WorkoutTemplates/{id} - Sem mudança, retorna NoContent
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkoutTemplate(int id, [FromQuery] int userId) // Temporário userId
        {
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                await _templateService.DeleteWorkoutTemplateAsync(id, requestingUserId);
                return NoContent();
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("deletar templates padrão")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- MÉTODOS AUXILIARES DE MAPEAMENTO ---
        private WorkoutTemplateDto MapToSummaryDto(WorkoutTemplate template)
        {
            return new WorkoutTemplateDto
            {
                TemplateId = template.TemplateId,
                UserId = template.UserId,
                Name = template.Name,
                Difficulty = template.Difficulty,
                IsPremium = template.IsPremium,
                ExerciseCount = template.TemplateExercises?.Count ?? 0
            };
        }

        private WorkoutTemplateDetailDto MapToDetailDto(WorkoutTemplate template)
        {
            return new WorkoutTemplateDetailDto
            {
                TemplateId = template.TemplateId,
                UserId = template.UserId,
                Name = template.Name,
                Difficulty = template.Difficulty,
                IsPremium = template.IsPremium,
                Exercises = template.TemplateExercises?.Select(te => new TemplateExerciseResponseDto
                {
                    ExerciseId = te.ExerciseId,
                    ExerciseName = te.Exercise?.Name ?? "Nome não carregado", // Pega nome do Exercise incluído
                    OrderIndex = te.OrderIndex
                    // MuscleGroup = te.Exercise?.MuscleGroup ?? "" // Exemplo se quisesse adicionar
                }).ToList() ?? new List<TemplateExerciseResponseDto>()
            };
        }
    }
}