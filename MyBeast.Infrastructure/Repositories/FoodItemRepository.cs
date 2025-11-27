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
            return await _context.FoodItems
                .Where(f => f.UserId == userId || f.UserId == null || f.IsCustom == false)
                .ToListAsync();
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