using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class MealLogItemRepository : IMealLogItemRepository
    {
        private readonly ApiDbContext _context;

        public MealLogItemRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MealLogItem>> GetByMealLogIdAsync(int mealLogId)
        {
            return await _context.MealLogItems
                       .Where(mli => mli.MealLogId == mealLogId)
                       .Include(mli => mli.FoodItem) // Inclui detalhes do alimento
                       .AsNoTracking()
                       .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<MealLogItem> mealLogItems)
        {
            await _context.MealLogItems.AddRangeAsync(mealLogItems);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemsByMealLogIdAsync(int mealLogId)
        {
            var itemsToDelete = await _context.MealLogItems
                                    .Where(mli => mli.MealLogId == mealLogId)
                                    .ToListAsync();
            if (itemsToDelete.Any())
            {
                _context.MealLogItems.RemoveRange(itemsToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}