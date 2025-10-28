using MyBeast.Domain.Models;
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    public interface IWorkoutTemplateRepository
    {
        Task<IEnumerable<WorkoutTemplate>> GetAllAsync(); // Maybe remove later? Or paginate?
        Task<WorkoutTemplate?> GetByIdAsync(int id, bool includeExercises = false); // Flag para incluir
        Task<IEnumerable<WorkoutTemplate>> GetDefaultsAsync(bool includeExercises = false);
        Task<IEnumerable<WorkoutTemplate>> GetByUserIdAsync(int userId, bool includeExercises = false); // Novo
        Task<WorkoutTemplate> AddAsync(WorkoutTemplate template); // Novo
        Task<WorkoutTemplate> UpdateAsync(WorkoutTemplate template); // Novo
        Task DeleteAsync(int id); // Novo
    }
}