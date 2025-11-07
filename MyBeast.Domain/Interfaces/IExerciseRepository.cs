using MyBeast.Domain.Entities;
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetAllAsync(); // Templates + Custom? Ou só templates? Decidiremos no Serviço.
        Task<Exercise?> GetByIdAsync(int id);
        Task<IEnumerable<Exercise>> GetByUserIdAsync(int userId); // Novo: Buscar customizados do usuário
        Task<Exercise> AddAsync(Exercise exercise); // Novo
        Task<Exercise> UpdateAsync(Exercise exercise); // Novo
        Task DeleteAsync(int id); // Novo
        Task<Exercise?> GetByNameAndUserIdAsync(string name, int? userId); // Novo: Para verificar duplicatas
    }
}