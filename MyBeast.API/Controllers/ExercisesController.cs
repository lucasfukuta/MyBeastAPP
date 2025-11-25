using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.DTOs.Exercise.Input;   // DTOs de Entrada
using MyBeast.Domain.DTOs.Exercise.Output;  // DTOs de Saída
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using System.Security.Claims; // Para Claims
using System.IdentityModel.Tokens.Jwt;
using MyBeast.Domain.Entities; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA TODOS OS ENDPOINTS
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        // GET /api/Exercises (Busca templates + customizados do usuário logado)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExerciseDto>))]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetExercises()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token
            var exercises = await _exerciseService.GetAllExercisesAsync(userId);
            var exerciseDtos = exercises.Select(MapToDto);
            return Ok(exerciseDtos);
        }

        // GET /api/Exercises/me (Busca APENAS customizados do usuário logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExerciseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Middleware cuidará disso
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetMyCustomExercises()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            // O Middleware capturará "Usuário não encontrado" (404) do serviço.
            var exercises = await _exerciseService.GetCustomExercisesByUserIdAsync(userId);
            var exerciseDtos = exercises.Select(MapToDto);
            return Ok(exerciseDtos);
        }

        // GET /api/Exercises/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExerciseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ExerciseDto>> GetExercise(int id)
        {
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise == null) return NotFound();

            // Verificação de permissão (manual)
            if (exercise.IsCustom && exercise.UserId != null)
            {
                var requestingUserId = GetAuthenticatedUserId();
                if (exercise.UserId != requestingUserId)
                {
                    return Forbid(); // 403 Forbidden
                }
            }

            var exerciseDto = MapToDto(exercise);
            return Ok(exerciseDto);
        }

        // POST /api/Exercises (Criar exercício customizado para o usuário logado)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ExerciseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // User não encontrado
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Nome duplicado
        public async Task<ActionResult<ExerciseDto>> CreateCustomExercise([FromBody] ExerciseCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            var exerciseToCreate = new Exercise
            {
                Name = createDto.Name,
                MuscleGroup = createDto.MuscleGroup,
                Instructions = createDto.Instructions
            };

            var newExercise = await _exerciseService.CreateCustomExerciseAsync(exerciseToCreate, requestingUserId);
            var newExerciseDto = MapToDto(newExercise);
            return CreatedAtAction(nameof(GetExercise), new { id = newExerciseDto.ExerciseId }, newExerciseDto);
        }

        // PUT /api/Exercises/{id} (Atualizar exercício customizado do usuário logado)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExerciseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Nome duplicado
        public async Task<ActionResult<ExerciseDto>> UpdateCustomExercise(int id, [FromBody] ExerciseUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            var exerciseUpdateData = new Exercise
            {
                Name = updateDto.Name ?? "",
                MuscleGroup = updateDto.MuscleGroup ?? "",
                Instructions = updateDto.Instructions
            };

            var updatedExercise = await _exerciseService.UpdateCustomExerciseAsync(id, exerciseUpdateData, requestingUserId);
            var updatedExerciseDto = MapToDto(updatedExercise);
            return Ok(updatedExerciseDto);
        }

        // DELETE /api/Exercises/{id} (Deletar exercício customizado do usuário logado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomExercise(int id)
        {
            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO.
            await _exerciseService.DeleteCustomExerciseAsync(id, requestingUserId);
            return NoContent();
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private ExerciseDto MapToDto(Exercise exercise)
        {
            if (exercise == null) return null;

            return new ExerciseDto
            {
                ExerciseId = exercise.ExerciseId,
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup,
                Instructions = exercise.Instructions,
                IsCustom = exercise.IsCustom,
                UserId = exercise.UserId
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