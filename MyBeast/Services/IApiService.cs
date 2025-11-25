using MyBeast.Domain.Entities; // <--- CORREÇÃO 1: Adicionado para reconhecer Achievement
using MyBeast.Domain.DTOs.WorkoutTemplate.Output; // <--- DTOs de Treino
using MyBeast.Domain.DTOs.WorkoutTemplate.Input;

namespace MyBeast.Services
{
    public interface IApiService
    {
        // --- MÉTODOS DE CONQUISTAS (Legado/Existente) ---
        Task<Achievement> GetAchievementAsync(string url);
        Task<IEnumerable<Achievement>> GetAchievementsAsync(string url);
        Task<bool> PostAchievementAsync(string url, Achievement achievement);
        Task<bool> PutAchievementAsync(string url, Achievement achievement);
        Task<bool> DeleteAchievementAsync(string url);

        // --- MÉTODOS DE TREINOS (Novos com DTOs) ---
        Task<List<WorkoutTemplateDto>> GetMyWorkoutsAsync();
        Task<List<WorkoutTemplateDto>> GetDefaultWorkoutsAsync();
        Task<bool> CreateWorkoutAsync(WorkoutTemplateCreateDto dto);
        Task<bool> DeleteWorkoutAsync(int id);
    }
}
