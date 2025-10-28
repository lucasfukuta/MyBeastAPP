using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class SetLogRepository : ISetLogRepository
    {
        private readonly ApiDbContext _context;

        public SetLogRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SetLog>> GetBySessionIdAsync(int sessionId)
        {
            return await _context.SetLogs
                       .Where(sl => sl.SessionId == sessionId)
                       .Include(sl => sl.Exercise) // Inclui dados do exercício
                       .OrderBy(sl => sl.SetNumber) // Ordena por número da série
                       .AsNoTracking()
                       .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<SetLog> setLogs)
        {
            await _context.SetLogs.AddRangeAsync(setLogs);
            await _context.SaveChangesAsync();
        }
    }
}