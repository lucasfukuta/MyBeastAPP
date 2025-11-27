using MyBeast.Domain.Entities;

namespace MyBeast.Domain.Interfaces
{
    public interface IFoodItemRepository
    {
        // Métodos CRUD 
        Task<FoodItem> GetByIdAsync(int id);
        Task<FoodItem> AddAsync(FoodItem entity);
        Task<FoodItem> UpdateAsync(FoodItem entity);
        Task DeleteAsync(int id);

        // Métodos Específicos
        Task<IEnumerable<FoodItem>> GetAllAccessibleAsync(int userId);
        Task<FoodItem?> GetByNameAndUserIdAsync(string name, int? userId);
        Task<IEnumerable<FoodItem>> GetByUserIdAsync(int userId);
    }
}