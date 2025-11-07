using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    // Contrato para a lógica de negócio de Achievement
    public interface IAchievementService
    {
        Task<Achievement?> GetAchievementByIdAsync(int achievementId);
        Task<IEnumerable<Achievement>> GetAchievementsByUserIdAsync(int userId);
        Task<Achievement> GrantAchievementAsync(int userId, string name, string description); // Método para conceder uma conquista
    }
}