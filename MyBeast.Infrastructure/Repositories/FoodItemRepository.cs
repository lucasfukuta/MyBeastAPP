using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Entities;
using MyBeast.Domain.Interfaces;
using MyBeast.Infrastructure.Data;

namespace MyBeast.Infrastructure.Repositories
{
    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly ApiDbContext _context;

        public FoodItemRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<FoodItem> GetByIdAsync(int id)
        {
            return await _context.FoodItems.FindAsync(id);
        }

        public async Task<FoodItem> AddAsync(FoodItem entity)
        {
            _context.FoodItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<FoodItem> UpdateAsync(FoodItem entity)
        {
            _context.FoodItems.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.FoodItems.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // --- Métodos Específicos ---

        public async Task<IEnumerable<FoodItem>> GetAllAccessibleAsync(int userId)
        {
            // Traz do banco e converte se necessário
            var items = await _context.FoodItems
                .Where(f => f.UserId == userId || f.UserId == null || f.IsCustom == false)
                // O Select força a conversão caso o banco esteja retornando decimal
                .Select(f => new FoodItem
                {
                    FoodId = f.FoodId,
                    Name = f.Name,
                    // O erro diz que vem Decimal e queremos Int. O cast (int) resolve.
                    Calories = (int)f.Calories,
                    Protein = (int)f.Protein,
                    Carbs = (int)f.Carbs,
                    Fat = (int)f.Fat,
                    IsCustom = f.IsCustom,
                    UserId = f.UserId
                })
                .ToListAsync();

            return items;
        }

        public async Task<FoodItem?> GetByNameAndUserIdAsync(string name, int? userId)
        {
            return await _context.FoodItems
                .FirstOrDefaultAsync(f => f.Name == name && f.UserId == userId);
        }

        public async Task<IEnumerable<FoodItem>> GetByUserIdAsync(int userId)
        {
            return await _context.FoodItems
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }
    }
}