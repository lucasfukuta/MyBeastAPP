using MyBeast.Domain.Models;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para os alimentos
    public interface IFoodItemRepository
    {
        Task<IEnumerable<FoodItem>> GetAllAsync();
        Task<FoodItem?> GetByIdAsync(int id);
    }
}