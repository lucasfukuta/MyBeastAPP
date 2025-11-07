using MyBeast.Domain.Entities;
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para acessar dados de WorkoutSession
    public interface IWorkoutSessionRepository
    {
        Task<WorkoutSession?> GetByIdAsync(int sessionId);
        Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(int userId);
        Task<WorkoutSession> AddAsync(WorkoutSession session);
        Task<WorkoutSession> UpdateAsync(WorkoutSession session);
        Task DeleteAsync(int sessionId);
    }
}