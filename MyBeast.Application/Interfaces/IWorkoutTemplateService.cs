using MyBeast.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    public interface IWorkoutTemplateService
    {
        Task<IEnumerable<WorkoutTemplate>> GetAllWorkoutTemplatesAsync();
        Task<WorkoutTemplate?> GetWorkoutTemplateByIdAsync(int id, int requestingUserId, bool includeExercises = false); // Adicionado requestingUserId
        Task<IEnumerable<WorkoutTemplate>> GetDefaultWorkoutTemplatesAsync(bool includeExercises = false);
        Task<IEnumerable<WorkoutTemplate>> GetWorkoutTemplatesByUserIdAsync(int userId, bool includeExercises = false);
        Task<WorkoutTemplate> CreateWorkoutTemplateAsync(WorkoutTemplate template, int requestingUserId); // Adicionado requestingUserId
        Task<WorkoutTemplate> UpdateWorkoutTemplateAsync(int id, WorkoutTemplate template, int requestingUserId); // Já estava correto
        Task DeleteWorkoutTemplateAsync(int id, int requestingUserId); // Já estava correto
    }
}