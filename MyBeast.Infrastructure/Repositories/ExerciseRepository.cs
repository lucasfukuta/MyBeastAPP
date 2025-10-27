using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;

namespace MyBeast.Infrastructure.Repositories
{
    // Esta classe implementa o "contrato" do Domain
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApiDbContext _context;

        public ExerciseRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync()
        {
            // Esta é a lógica de dados real
            return await _context.Exercises.AsNoTracking().ToListAsync();
        }

        public async Task<Exercise?> GetByIdAsync(int id)
        {
            return await _context.Exercises.AsNoTracking()
                .FirstOrDefaultAsync(e => e.ExerciseId == id);
        }
    }
}