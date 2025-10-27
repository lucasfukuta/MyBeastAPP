using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces; // Importa o serviço

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        // Pedimos a interface do Serviço, não o repositório
        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExercises()
        {
            var exercises = await _exerciseService.GetAllExercisesAsync();
            return Ok(exercises);
        }
    }
}