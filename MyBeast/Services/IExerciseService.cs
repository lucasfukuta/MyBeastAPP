using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    public interface IExerciseService
    {
        // Retorna todos os exercícios (sistema + usuário)
        Task<IEnumerable<Exercise>> GetAllExercisesAsync();

        // Retorna apenas exercícios criados pelo usuário
        Task<IEnumerable<Exercise>> GetCustomExercisesAsync();

        // Retorna um exercício específico
        Task<Exercise?> GetExerciseByIdAsync(int id);

        // Cria um novo exercício
        Task<Exercise> AddExerciseAsync(Exercise exercise);

        // Remove um exercício
        Task DeleteExerciseAsync(int id);
    }
}