using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly ApiDbContext _context;

        public AchievementRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Achievement?> GetByIdAsync(int achievementId)
        {
            return await _context.Achievements.AsNoTracking()
                       .FirstOrDefaultAsync(a => a.AchievementId == achievementId);
        }

        public async Task<IEnumerable<Achievement>> GetByUserIdAsync(int userId)
        {
            return await _context.Achievements
                       .Where(a => a.UserId == userId)
                       .OrderByDescending(a => a.DateAchieved) // Mais recentes primeiro
                       .AsNoTracking()
                       .ToListAsync();
        }

        public async Task<Achievement> AddAsync(Achievement achievement)
        {
            await _context.Achievements.AddAsync(achievement);
            await _context.SaveChangesAsync();
            return achievement;
        }
        public async Task<Achievement?> GetByNameAndUserIdAsync(string name, int userId)
        {
            // Busca por nome exato (ignorando case) e UserId
            return await _context.Achievements.AsNoTracking()
                       .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower() && a.UserId == userId);
        }
    }
}