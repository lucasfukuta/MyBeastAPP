using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MyBeast.API.DTOs.Exercise.Input;
using MyBeast.API.DTOs.Exercise.Output; // Adicionado para Select

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        // GET /api/Exercises - Retorna Lista de ExerciseDto
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExerciseDto>))] // Atualizado
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetExercises([FromQuery] int? userId = null)
        {
            // TODO: Se userId != null, verificar permissão
            var exercises = await _exerciseService.GetAllExercisesAsync(userId);

            // Mapear Model para Dto
            var exerciseDtos = exercises.Select(MapToDto); // Usa método auxiliar

            return Ok(exerciseDtos);
        }

        // GET /api/Exercises/user/{userId} - Retorna Lista de ExerciseDto
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExerciseDto>))] // Atualizado
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetUserExercises(int userId)
        {
            // TODO: Verificar permissão
            try
            {
                var exercises = await _exerciseService.GetCustomExercisesByUserIdAsync(userId);
                // Mapear Model para Dto
                var exerciseDtos = exercises.Select(MapToDto);
                return Ok(exerciseDtos);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/Exercises/{id} - Retorna ExerciseDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExerciseDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExerciseDto>> GetExercise(int id)
        {
            // TODO: Verificar permissão se for custom de outro user
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise == null) return NotFound();

            // Mapear Model para Dto
            var exerciseDto = MapToDto(exercise);
            return Ok(exerciseDto);
        }

        // POST /api/Exercises - Retorna ExerciseDto
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ExerciseDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExerciseDto>> CreateCustomExercise([FromBody] ExerciseCreateDto createDto) // DTO de Entrada
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = createDto.UserId;

            try
            {
                var exerciseToCreate = new Exercise
                {
                    Name = createDto.Name,
                    MuscleGroup = createDto.MuscleGroup,
                    Instructions = createDto.Instructions
                };

                var newExercise = await _exerciseService.CreateCustomExerciseAsync(exerciseToCreate, requestingUserId);
                // Mapear Model para Dto
                var newExerciseDto = MapToDto(newExercise);
                return CreatedAtAction(nameof(GetExercise), new { id = newExerciseDto.ExerciseId }, newExerciseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/Exercises/{id} - Retorna ExerciseDto
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExerciseDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExerciseDto>> UpdateCustomExercise(int id, [FromBody] ExerciseUpdateDto updateDto, [FromQuery] int userId) // DTO de Entrada
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                var exerciseUpdateData = new Exercise
                {
                    Name = updateDto.Name ?? "",
                    MuscleGroup = updateDto.MuscleGroup ?? "",
                    Instructions = updateDto.Instructions
                };

                var updatedExercise = await _exerciseService.UpdateCustomExerciseAsync(id, exerciseUpdateData, requestingUserId);
                // Mapear Model para Dto
                var updatedExerciseDto = MapToDto(updatedExercise);
                return Ok(updatedExerciseDto);
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE /api/Exercises/{id} - Sem mudança, retorna NoContent
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomExercise(int id, [FromQuery] int userId) // Temporário userId
        {
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                await _exerciseService.DeleteCustomExerciseAsync(id, requestingUserId);
                return NoContent();
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private ExerciseDto MapToDto(Exercise exercise)
        {
            if (exercise == null) return null; // Segurança

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
    }
}