using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    public interface IFoodItemService
    {
        Task<IEnumerable<FoodItem>> GetAllFoodItemsAsync(int? userId = null); // Opcional: filtrar
        Task<FoodItem?> GetFoodItemByIdAsync(int id);
        Task<IEnumerable<FoodItem>> GetCustomFoodItemsByUserIdAsync(int userId); // Novo
        Task<FoodItem> CreateCustomFoodItemAsync(FoodItem foodItem, int userId); // Novo
        Task<FoodItem> UpdateCustomFoodItemAsync(int id, FoodItem foodItemUpdateData, int requestingUserId); // Novo
        Task DeleteCustomFoodItemAsync(int id, int requestingUserId); // Novo
    }
}