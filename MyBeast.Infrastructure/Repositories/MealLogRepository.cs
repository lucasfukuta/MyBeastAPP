using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class MealLogRepository : IMealLogRepository
    {
        private readonly ApiDbContext _context;

        public MealLogRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<MealLog?> GetByIdAsync(int mealLogId)
        {
            // Inclui os itens e os detalhes dos alimentos
            return await _context.MealLogs
                       .Include(ml => ml.MealLogItems)
                           .ThenInclude(mli => mli.FoodItem)
                       .AsNoTracking()
                       .FirstOrDefaultAsync(ml => ml.MealLogId == mealLogId);
        }

        public async Task<IEnumerable<MealLog>> GetByUserIdAsync(int userId)
        {
            return await _context.MealLogs
                       .Where(ml => ml.UserId == userId)
                       .Include(ml => ml.MealLogItems) // Inclui itens para mostrar na lista
                           .ThenInclude(mli => mli.FoodItem)
                       .OrderByDescending(ml => ml.Date)
                       .AsNoTracking()
                       .ToListAsync();
        }

        public async Task<IEnumerable<MealLog>> GetByUserIdAndDateAsync(int userId, DateTime date)
        {
            // Compara apenas a parte da Data (ignora a hora)
            return await _context.MealLogs
                       .Where(ml => ml.UserId == userId && ml.Date.Date == date.Date)
                       .Include(ml => ml.MealLogItems)
                           .ThenInclude(mli => mli.FoodItem)
                       .OrderBy(ml => ml.Date) // Ordena pela hora dentro do dia
                       .AsNoTracking()
                       .ToListAsync();
        }


        public async Task<MealLog> AddAsync(MealLog mealLog)
        {
            await _context.MealLogs.AddAsync(mealLog);
            await _context.SaveChangesAsync();
            return mealLog; // O ID é preenchido
        }

        public async Task<MealLog> UpdateAsync(MealLog mealLog)
        {
            _context.Entry(mealLog).State = EntityState.Modified;
            _context.Entry(mealLog).Property(ml => ml.UserId).IsModified = false;
            await _context.SaveChangesAsync();
            return mealLog;
        }

        public async Task DeleteAsync(int mealLogId)
        {
            var mealLog = await _context.MealLogs.FindAsync(mealLogId);
            if (mealLog != null)
            {
                _context.MealLogs.Remove(mealLog);
                await _context.SaveChangesAsync(); // A cascata (se definida) remove os MealLogItems
            }
        }
    }
}