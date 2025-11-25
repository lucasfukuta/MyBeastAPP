using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Entities;
using MyBeast.Domain.DTOs.WorkoutTemplate.Input;   // DTOs de Entrada
using MyBeast.Domain.DTOs.WorkoutTemplate.Output;  // DTOs de Saída
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [Authorize] e [AllowAnonymous]
using System.Security.Claims; // Para Claims
using System.IdentityModel.Tokens.Jwt; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA A MAIORIA (exceto defaults)
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutTemplatesController : ControllerBase
    {
        private readonly IWorkoutTemplateService _templateService;

        public WorkoutTemplatesController(IWorkoutTemplateService templateService)
        {
            _templateService = templateService;
        }

        // GET /api/WorkoutTemplates (Busca templates padrão + customizados do usuário logado)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutTemplateDto>))]
        public async Task<ActionResult<IEnumerable<WorkoutTemplateDto>>> GetWorkoutTemplates()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            var defaultTemplates = await _templateService.GetDefaultWorkoutTemplatesAsync(false);
            var userTemplates = await _templateService.GetWorkoutTemplatesByUserIdAsync(userId, false);

            var allTemplates = defaultTemplates.Concat(userTemplates);
            var templateDtos = allTemplates.Select(MapToSummaryDto);

            return Ok(templateDtos);
        }

        // GET /api/WorkoutTemplates/defaults (Templates Padrão - Público)
        [AllowAnonymous] // PERMITE ACESSO PÚBLICO
        [HttpGet("defaults")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutTemplateDto>))]
        public async Task<ActionResult<IEnumerable<object>>> GetDefaultWorkoutTemplates([FromQuery] bool includeExercises = false)
        {
            var templates = await _templateService.GetDefaultWorkoutTemplatesAsync(includeExercises);
            if (includeExercises)
            {
                var detailDtos = templates.Select(MapToDetailDto);
                return Ok(detailDtos);
            }
            var summaryDtos = templates.Select(MapToSummaryDto);
            return Ok(summaryDtos);
        }

        // GET /api/WorkoutTemplates/me (Templates do Usuário Logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WorkoutTemplateDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Middleware cuidará disso
        public async Task<ActionResult<IEnumerable<object>>> GetMyWorkoutTemplates([FromQuery] bool includeExercises = false)
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            var templates = await _templateService.GetWorkoutTemplatesByUserIdAsync(userId, includeExercises);

            if (includeExercises)
            {
                var detailDtos = templates.Select(MapToDetailDto);
                return Ok(detailDtos);
            }
            var summaryDtos = templates.Select(MapToSummaryDto);
            return Ok(summaryDtos);
        }

        // GET /api/WorkoutTemplates/{id} (Busca um template específico)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutTemplateDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<WorkoutTemplateDetailDto>> GetWorkoutTemplate(int id, [FromQuery] bool includeExercises = true)
        {
            int requestingUserId = 0; // 0 para usuário anônimo
            try
            {
                // Tenta pegar o ID do usuário se ele estiver logado
                requestingUserId = GetAuthenticatedUserId();
            }
            catch { /* Ignora - usuário não está logado */ }

            // Bloco try-catch REMOVIDO (exceto o que pega o userId)
            // O Middleware capturará "permissão" (403) do serviço.
            var template = await _templateService.GetWorkoutTemplateByIdAsync(id, requestingUserId, includeExercises);

            if (template == null) return NotFound();

            var detailDto = MapToDetailDto(template);
            return Ok(detailDto);
        }

        // POST /api/WorkoutTemplates (Cria template para usuário logado)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkoutTemplateDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkoutTemplateDetailDto>> CreateWorkoutTemplate([FromBody] WorkoutTemplateCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            var templateToCreate = new WorkoutTemplate
            {
                // UserId é definido no serviço usando o ID do token
                Name = createDto.Name,
                Difficulty = createDto.Difficulty,
                TemplateExercises = createDto.Exercises?.Select(e => new TemplateExercise
                {
                    ExerciseId = e.ExerciseId,
                    OrderIndex = e.OrderIndex
                }).ToList() ?? new List<TemplateExercise>()
            };

            var newTemplate = await _templateService.CreateWorkoutTemplateAsync(templateToCreate, requestingUserId);

            var createdTemplateWithDetails = await _templateService.GetWorkoutTemplateByIdAsync(newTemplate.TemplateId, requestingUserId, true);
            if (createdTemplateWithDetails == null) return BadRequest("Erro ao buscar template criado.");

            var detailDto = MapToDetailDto(createdTemplateWithDetails);
            return CreatedAtAction(nameof(GetWorkoutTemplate), new { id = detailDto.TemplateId }, detailDto);
        }

        // PUT /api/WorkoutTemplates/{id} (Atualiza template do usuário logado)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkoutTemplateDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutTemplateDetailDto>> UpdateWorkoutTemplate(int id, [FromBody] WorkoutTemplateUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            var templateUpdateData = new WorkoutTemplate
            {
                Name = updateDto.Name ?? "",
                Difficulty = updateDto.Difficulty,
                TemplateExercises = updateDto.Exercises?.Select(e => new TemplateExercise
                {
                    ExerciseId = e.ExerciseId,
                    OrderIndex = e.OrderIndex
                }).ToList()
            };

            await _templateService.UpdateWorkoutTemplateAsync(id, templateUpdateData, requestingUserId);

            var updatedTemplateWithDetails = await _templateService.GetWorkoutTemplateByIdAsync(id, requestingUserId, true);
            if (updatedTemplateWithDetails == null) return NotFound();

            var detailDto = MapToDetailDto(updatedTemplateWithDetails);
            return Ok(detailDto);
        }

        // DELETE /api/WorkoutTemplates/{id} (Deleta template do usuário logado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkoutTemplate(int id)
        {
            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            await _templateService.DeleteWorkoutTemplateAsync(id, requestingUserId);
            return NoContent();
        }

        // --- MÉTODOS AUXILIARES DE MAPEAMENTO ---
        private WorkoutTemplateDto MapToSummaryDto(WorkoutTemplate template)
        {
            if (template == null) return null;
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
            if (template == null) return null;
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
                    ExerciseName = te.Exercise?.Name ?? "Nome não carregado",
                    OrderIndex = te.OrderIndex
                }).ToList() ?? new List<TemplateExerciseResponseDto>()
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