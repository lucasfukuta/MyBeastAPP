using MyBeast.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System; // Para DateTime

namespace MyBeast.Application.Interfaces
{
    public interface IWorkoutSessionService
    {
        Task<WorkoutSession?> GetWorkoutSessionByIdAsync(int sessionId, int requestingUserId); // Adicionado requestingUserId
        Task<IEnumerable<WorkoutSession>> GetWorkoutSessionsByUserIdAsync(int userId);
        Task<WorkoutSession> StartWorkoutSessionAsync(int requestingUserId, DateTime startTime); // Parâmetro renomeado
        Task<WorkoutSession> EndWorkoutSessionAsync(int sessionId, int requestingUserId, DateTime endTime, decimal totalVolume, List<SetLog> setLogs); // Adicionado requestingUserId
        Task DeleteWorkoutSessionAsync(int sessionId, int requestingUserId); // Adicionado requestingUserId
    }
}