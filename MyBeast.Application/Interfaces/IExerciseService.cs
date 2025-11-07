using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    public interface IExerciseService
    {
        Task<IEnumerable<Exercise>> GetAllExercisesAsync(int? userId = null); // Opcional: filtrar por usuário
        Task<Exercise?> GetExerciseByIdAsync(int id);
        Task<IEnumerable<Exercise>> GetCustomExercisesByUserIdAsync(int userId); // Novo
        Task<Exercise> CreateCustomExerciseAsync(Exercise exercise, int userId); // Novo
        Task<Exercise> UpdateCustomExerciseAsync(int id, Exercise exerciseUpdateData, int requestingUserId); // Novo
        Task DeleteCustomExerciseAsync(int id, int requestingUserId); // Novo
    }
}