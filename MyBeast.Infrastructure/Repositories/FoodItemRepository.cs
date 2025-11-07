using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Infrastructure.Data;
using MyBeast.Domain.Entities;
using System.Collections.Generic; // Adicionado
using System.Linq; // Adicionado
using System.Threading.Tasks; // Adicionado

namespace MyBeast.Infrastructure.Repositories
{
    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly ApiDbContext _context;

        public FoodItemRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FoodItem>> GetAllAsync()
        {
            // Retorna todos (templates e customizados)
            return await _context.FoodItems.AsNoTracking().ToListAsync();
        }

        public async Task<FoodItem?> GetByIdAsync(int id)
        {
            return await _context.FoodItems.AsNoTracking()
                       .FirstOrDefaultAsync(f => f.FoodId == id);
        }

        // --- NOVOS MÉTODOS ---
        public async Task<IEnumerable<FoodItem>> GetByUserIdAsync(int userId)
        {
            // Retorna apenas alimentos customizados do usuário
            return await _context.FoodItems
                       .Where(f => f.IsCustom && f.UserId == userId)
                       .OrderBy(f => f.Name)
                       .AsNoTracking()
                       .ToListAsync();
        }

        public async Task<FoodItem?> GetByNameAndUserIdAsync(string name, int? userId)
        {
            // Busca por nome exato, ignorando case, considerando template (null) ou custom (userId)
            return await _context.FoodItems.AsNoTracking()
                       .FirstOrDefaultAsync(f => f.Name.ToLower() == name.ToLower() && f.UserId == userId);
        }

        public async Task<FoodItem> AddAsync(FoodItem foodItem)
        {
            foodItem.User = null; // Evita inserir User
            await _context.FoodItems.AddAsync(foodItem);
            await _context.SaveChangesAsync();
            return foodItem;
        }

        public async Task<FoodItem> UpdateAsync(FoodItem foodItem)
        {
            _context.Entry(foodItem).State = EntityState.Modified;
            // Garante que IsCustom e UserId não sejam modificados
            _context.Entry(foodItem).Property(f => f.IsCustom).IsModified = false;
            _context.Entry(foodItem).Property(f => f.UserId).IsModified = false;
            await _context.SaveChangesAsync();
            return foodItem;
        }

        public async Task DeleteAsync(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem != null && foodItem.IsCustom) // Só permite deletar customizados
            {
                // Verificar se está em uso em MealLogItem?
                // FK constraint deve impedir se estiver em uso.
                _context.FoodItems.Remove(foodItem);
                await _context.SaveChangesAsync();
            }
        }
        // --- FIM DOS NOVOS MÉTODOS ---
    }
}