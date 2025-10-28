using MyBeast.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    public interface IWorkoutTemplateService
    {
        Task<IEnumerable<WorkoutTemplate>> GetAllWorkoutTemplatesAsync(); // Manter por enquanto
        Task<WorkoutTemplate?> GetWorkoutTemplateByIdAsync(int id, bool includeExercises = false);
        Task<IEnumerable<WorkoutTemplate>> GetDefaultWorkoutTemplatesAsync(bool includeExercises = false);
        Task<IEnumerable<WorkoutTemplate>> GetWorkoutTemplatesByUserIdAsync(int userId, bool includeExercises = false); // Novo
        Task<WorkoutTemplate> CreateWorkoutTemplateAsync(WorkoutTemplate template); // Novo
        Task<WorkoutTemplate> UpdateWorkoutTemplateAsync(int id, WorkoutTemplate template, int requestingUserId); // Novo + User check
        Task DeleteWorkoutTemplateAsync(int id, int requestingUserId); // Novo + User check
    }
}