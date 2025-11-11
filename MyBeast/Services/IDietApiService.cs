using MyBeast.Models.DTOs.Diet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Services
{
    public interface IDietApiService
    {
        Task<List<MealLogDto>> GetMealLogsByDateAsync(DateTime date);
        Task<List<FoodItemDto>> GetFoodItemsAsync();
    }
}
