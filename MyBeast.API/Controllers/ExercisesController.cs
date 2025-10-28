using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.Dtos.Exercise; // Importa DTOs
using System; // Para Exception
using System.Collections.Generic;
using System.Linq; // Para Select
using System.Threading.Tasks;

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

        // GET /api/Exercises (Pode filtrar por usuário ou retornar só templates)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises([FromQuery] int? userId = null)
        {
            // TODO: Se userId != null, verificar permissão do usuário logado
            var exercises = await _exerciseService.GetAllExercisesAsync(userId);
            // TODO: Mapear para ExerciseDto se necessário (para omitir UserId?)
            return Ok(exercises);
        }

        // GET /api/Exercises/user/{userId} (Busca customizados de um usuário)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetUserExercises(int userId)
        {
            // TODO: Verificar permissão do usuário logado
            try
            {
                var exercises = await _exerciseService.GetCustomExercisesByUserIdAsync(userId);
                // TODO: Mapear para ExerciseDto
                return Ok(exercises);
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

        // GET /api/Exercises/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(int id)
        {
            // TODO: Verificar permissão se for customizado de outro usuário
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise == null) return NotFound();
            // TODO: Mapear para ExerciseDto
            return Ok(exercise);
        }

        // POST /api/Exercises (Criar exercício customizado)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Exercise))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Exercise>> CreateCustomExercise([FromBody] ExerciseCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Obter userId do usuário logado (autenticação) em vez de confiar no DTO
            int requestingUserId = createDto.UserId; // Substituir pela ID do usuário autenticado

            try
            {
                var exerciseToCreate = new Exercise
                {
                    Name = createDto.Name,
                    MuscleGroup = createDto.MuscleGroup,
                    Instructions = createDto.Instructions
                    // UserId e IsCustom serão definidos pelo serviço
                };

                var newExercise = await _exerciseService.CreateCustomExerciseAsync(exerciseToCreate, requestingUserId);
                // TODO: Mapear para ExerciseDto
                return CreatedAtAction(nameof(GetExercise), new { id = newExercise.ExerciseId }, newExercise);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ex: Nome duplicado, Usuário não encontrado
            }
        }

        // PUT /api/Exercises/{id} (Atualizar exercício customizado)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Exercise))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Exercise>> UpdateCustomExercise(int id, [FromBody] ExerciseUpdateDto updateDto, [FromQuery] int userId) // Temporário userId
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = userId; // Substituir

            try
            {
                var exerciseUpdateData = new Exercise
                {
                    Name = updateDto.Name ?? "", // Serviço ignora se vazio
                    MuscleGroup = updateDto.MuscleGroup ?? "", // Serviço ignora se vazio
                    Instructions = updateDto.Instructions // Serviço atualiza
                };

                var updatedExercise = await _exerciseService.UpdateCustomExerciseAsync(id, exerciseUpdateData, requestingUserId);
                // TODO: Mapear para ExerciseDto
                return Ok(updatedExercise);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template"))
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ex: Nome duplicado
            }
        }

        // DELETE /api/Exercises/{id} (Deletar exercício customizado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomExercise(int id, [FromQuery] int userId) // Temporário userId
        {
            // TODO: Obter userId do usuário logado (autenticação)
            int requestingUserId = userId; // Substituir

            try
            {
                await _exerciseService.DeleteCustomExerciseAsync(id, requestingUserId);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão") || ex.Message.Contains("template"))
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (Exception ex)
            {
                // Pode ser erro 500 se o exercício estiver em uso e a FK impedir
                return BadRequest(ex.Message);
            }
        }
    }
}