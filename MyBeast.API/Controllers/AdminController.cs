using Microsoft.AspNetCore.Mvc;
using MyBeast.API.Services;

namespace MyBeast.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataSeederService _seeder;

        public AdminController(DataSeederService seeder)
        {
            _seeder = seeder;
        }

        [HttpPost("seed-exercises")]
        public async Task<IActionResult> SeedExercises()
        {
            var result = await _seeder.SeedExercisesAsync();
            return Ok(result);
        }

        [HttpPost("seed-foods")]
        public async Task<IActionResult> SeedFoods()
        {
            var result = await _seeder.SeedFoodsAsync();
            return Ok(result);
        }
    }
}