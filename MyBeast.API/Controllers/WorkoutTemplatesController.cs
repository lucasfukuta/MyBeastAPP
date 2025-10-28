using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.Dtos.WorkoutTemplate; // Importa DTOs
using System; // Para Exception
using System.Collections.Generic;
using System.Linq; // Para Select
using System.Threading.Tasks;

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

        // GET /api/WorkoutTemplates (Busca Todos - talvez remover/paginar)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutTemplate>>> GetWorkoutTemplates()
        {
            // Não inclui exercícios por padrão
            var templates = await _templateService.GetAllWorkoutTemplatesAsync();
            return Ok(templates);
        }

        // GET /api/WorkoutTemplates/defaults
        [HttpGet("defaults")]
        public async Task<ActionResult<IEnumerable<WorkoutTemplate>>> GetDefaultWorkoutTemplates([FromQuery] bool includeExercises = false)
        {
            var templates = await _templateService.GetDefaultWorkoutTemplatesAsync(includeExercises);
            return Ok(templates);
        }

        // GET /api/WorkoutTemplates/user/{userId} (Novo)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<WorkoutTemplate>>> GetWorkoutTemplatesByUser(int userId, [FromQuery] bool includeExercises = false)
        {
            try
            {
                // TODO: Verificar se usuário logado pode ver templates deste userId
                var templates = await _templateService.GetWorkoutTemplatesByUserIdAsync(userId, includeExercises);
                return Ok(templates);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message); // Usuário não encontrado
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET /api/WorkoutTemplates/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutTemplate>> GetWorkoutTemplate(int id, [FromQuery] bool includeExercises = true) // Inclui por padrão
        {
            var template = await _templateService.GetWorkoutTemplateByIdAsync(id, includeExercises);
            if (template == null)
            {
                return NotFound();
            }
            // TODO: Mapear para DTO de resposta se necessário
            return Ok(template);
        }

        // POST /api/WorkoutTemplates (Novo)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkoutTemplate))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkoutTemplate>> CreateWorkoutTemplate([FromBody] WorkoutTemplateCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // TODO: Obter userId do usuário logado (autenticação) em vez de confiar no DTO
                // if (createDto.UserId != loggedInUserId) return Forbid();

                // Mapear DTO para Modelo
                var templateToCreate = new WorkoutTemplate
                {
                    UserId = createDto.UserId,
                    Name = createDto.Name,
                    Difficulty = createDto.Difficulty,
                    TemplateExercises = createDto.Exercises?.Select(e => new TemplateExercise
                    {
                        ExerciseId = e.ExerciseId,
                        OrderIndex = e.OrderIndex
                    }).ToList() ?? new List<TemplateExercise>()
                };

                var newTemplate = await _templateService.CreateWorkoutTemplateAsync(templateToCreate);
                return CreatedAtAction(nameof(GetWorkoutTemplate), new { id = newTemplate.TemplateId }, newTemplate);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ex: Usuário/Exercício não encontrado
            }
        }

        // PUT /api/WorkoutTemplates/{id} (Novo)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutTemplate))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutTemplate>> UpdateWorkoutTemplate(int id, [FromBody] WorkoutTemplateUpdateDto updateDto, [FromQuery] int userId) // Temporário userId
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = userId; // Substituir pela ID do usuário autenticado

            try
            {
                // Mapear DTO para Modelo Parcial
                var templateUpdateData = new WorkoutTemplate
                {
                    Name = updateDto.Name ?? "", // Usa Name se não for nulo, senão string vazia (serviço ignora)
                    Difficulty = updateDto.Difficulty, // Pode ser nulo
                    TemplateExercises = updateDto.Exercises?.Select(e => new TemplateExercise
                    {
                        ExerciseId = e.ExerciseId,
                        OrderIndex = e.OrderIndex
                    }).ToList() // Pode ser nulo se não for enviado
                };

                var updatedTemplate = await _templateService.UpdateWorkoutTemplateAsync(id, templateUpdateData, requestingUserId);
                return Ok(updatedTemplate);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("editar templates padrão"))
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/WorkoutTemplates/{id} (Novo)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkoutTemplate(int id, [FromQuery] int userId) // Temporário userId
        {
            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = userId; // Substituir

            try
            {
                await _templateService.DeleteWorkoutTemplateAsync(id, requestingUserId);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("deletar templates padrão"))
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}