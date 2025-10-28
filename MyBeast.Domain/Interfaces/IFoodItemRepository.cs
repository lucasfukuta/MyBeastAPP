using MyBeast.Domain.Models;
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    public interface IFoodItemRepository
    {
        Task<IEnumerable<FoodItem>> GetAllAsync(); // Templates + Custom? Decidiremos no Serviço.
        Task<FoodItem?> GetByIdAsync(int id);
        Task<IEnumerable<FoodItem>> GetByUserIdAsync(int userId); // Novo: Buscar customizados do usuário
        Task<FoodItem> AddAsync(FoodItem foodItem); // Novo
        Task<FoodItem> UpdateAsync(FoodItem foodItem); // Novo
        Task DeleteAsync(int id); // Novo
        Task<FoodItem?> GetByNameAndUserIdAsync(string name, int? userId); // Novo: Para verificar duplicatas
    }
}