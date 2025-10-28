using MyBeast.Domain.Models;
using System; // Para DateTime
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para acessar dados de MealLog
    public interface IMealLogRepository
    {
        Task<MealLog?> GetByIdAsync(int mealLogId);
        Task<IEnumerable<MealLog>> GetByUserIdAsync(int userId);
        Task<IEnumerable<MealLog>> GetByUserIdAndDateAsync(int userId, DateTime date); // Para buscar refeições de um dia
        Task<MealLog> AddAsync(MealLog mealLog);
        Task<MealLog> UpdateAsync(MealLog mealLog); // Geralmente não é muito usado para logs
        Task DeleteAsync(int mealLogId);
    }
}