using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq; // Para Where
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class WorkoutSessionRepository : IWorkoutSessionRepository
    {
        private readonly ApiDbContext _context;

        public WorkoutSessionRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<WorkoutSession?> GetByIdAsync(int sessionId)
        {
            // Exemplo incluindo SetLogs e Exercício associado
            return await _context.WorkoutSessions
                       .Include(ws => ws.SetLogs)
                           .ThenInclude(sl => sl.Exercise) // Inclui dados do Exercício
                       .AsNoTracking()
                       .FirstOrDefaultAsync(ws => ws.SessionId == sessionId);
        }

        public async Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(int userId)
        {
            return await _context.WorkoutSessions
                       .Where(ws => ws.UserId == userId)
                       .OrderByDescending(ws => ws.Date) // Mais recentes primeiro
                       .AsNoTracking()
                       .ToListAsync();
        }

        public async Task<WorkoutSession> AddAsync(WorkoutSession session)
        {
            await _context.WorkoutSessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<WorkoutSession> UpdateAsync(WorkoutSession session)
        {
            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task DeleteAsync(int sessionId)
        {
            var session = await _context.WorkoutSessions.FindAsync(sessionId);
            if (session != null)
            {
                _context.WorkoutSessions.Remove(session);
                await _context.SaveChangesAsync();
            }
        }
    }
}