using MyBeast.Domain.Models;

namespace MyBeast.Domain.Interfaces
{
    // Este é o "contrato" que diz o que queremos fazer com os exercícios
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetAllAsync();
        Task<Exercise?> GetByIdAsync(int id);
        // (Adicionaremos Add, Update, Delete depois)
    }
}