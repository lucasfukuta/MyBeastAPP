using MyBeast.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    // Contrato para a lógica de negócio de WorkoutSession
    public interface IWorkoutSessionService
    {
        Task<WorkoutSession?> GetWorkoutSessionByIdAsync(int sessionId);
        Task<IEnumerable<WorkoutSession>> GetWorkoutSessionsByUserIdAsync(int userId);
        Task<WorkoutSession> StartWorkoutSessionAsync(int userId, DateTime startTime); // Iniciar uma sessão
        Task<WorkoutSession> EndWorkoutSessionAsync(int sessionId, DateTime endTime, decimal totalVolume, List<SetLog> setLogs); // Finalizar e salvar sets
        Task DeleteWorkoutSessionAsync(int sessionId);
    }
}