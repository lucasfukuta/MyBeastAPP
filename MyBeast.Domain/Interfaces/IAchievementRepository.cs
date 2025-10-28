using MyBeast.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para acessar dados de Achievement
    public interface IAchievementRepository
    {
        Task<Achievement?> GetByIdAsync(int achievementId);
        Task<IEnumerable<Achievement>> GetByUserIdAsync(int userId);
        Task<Achievement> AddAsync(Achievement achievement);
        // Não teremos Update/Delete por enquanto, conquistas são geralmente apenas adicionadas
    }
}