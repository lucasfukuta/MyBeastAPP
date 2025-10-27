using MyBeast.Domain.Models;

namespace MyBeast.Application.Interfaces
{
    // A lógica de negócio que o Controller irá chamar
    public interface IExerciseService
    {
        Task<IEnumerable<Exercise>> GetAllExercisesAsync();
    }
}