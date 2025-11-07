using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para acessar dados de MealLogItem
    // Geralmente manipulado através do MealLog
    public interface IMealLogItemRepository
    {
        Task<IEnumerable<MealLogItem>> GetByMealLogIdAsync(int mealLogId);
        Task AddRangeAsync(IEnumerable<MealLogItem> mealLogItems);
        Task DeleteItemsByMealLogIdAsync(int mealLogId); // Para remover itens se a refeição for deletada
    }
}