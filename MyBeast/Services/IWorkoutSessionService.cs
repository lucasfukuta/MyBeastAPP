using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    public interface IWorkoutSessionService
    {
        // Inicia um treino (pode ser vazio ou baseado em um template)
        Task<WorkoutSession> StartSessionAsync(DateTime startTime, int? templateId = null);

        // Finaliza e salva o treino com seus logs
        Task<WorkoutSession> FinishSessionAsync(WorkoutSession session);

        // Obtém o histórico de treinos do usuário
        Task<IEnumerable<WorkoutSession>> GetSessionHistoryAsync();
    }
}