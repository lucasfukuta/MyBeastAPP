using MyBeast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    // Contrato para a lógica de negócio de MealLog
    public interface IMealLogService
    {
        Task<MealLog?> GetMealLogByIdAsync(int mealLogId);
        Task<IEnumerable<MealLog>> GetMealLogsByUserIdAsync(int userId);
        Task<IEnumerable<MealLog>> GetMealLogsByUserIdAndDateAsync(int userId, DateTime date);
        Task<MealLog> LogMealAsync(int userId, DateTime date, string mealType, List<MealLogItem> items); // Registrar uma refeição
        Task<MealLog> UpdateMealLogAsync(int mealLogId, int requestingUserId, DateTime? date, string? mealType, List<MealLogItem>? items);
        Task DeleteMealLogAsync(int mealLogId, int requestingUserId);
    }
}