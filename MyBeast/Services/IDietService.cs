using MyBeast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Services
{
    public interface IDietService
    {
        // Simula GET /api/meallogs/{date}
        Task<IEnumerable<MealLog>> GetMealsByDateAsync(DateTime date);

        // Simula POST /api/meallogs (ou similar para adicionar item)
        Task AddFoodItemAsync(DateTime date, string mealType, FoodItem foodItem, double quantity);

        // Simula DELETE /api/meallogs/items/{id}
        Task RemoveFoodItemAsync(int mealLogItemId);
    }
}
