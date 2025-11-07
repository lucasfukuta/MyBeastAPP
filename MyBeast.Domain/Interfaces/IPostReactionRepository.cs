using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para acessar dados de PostReaction
    public interface IPostReactionRepository
    {
        Task<PostReaction?> GetReactionAsync(int postId, int userId); // Verifica se usuário já reagiu
        Task<IEnumerable<PostReaction>> GetReactionsByPostIdAsync(int postId); // Para contar upvotes/downvotes
        Task<PostReaction> AddOrUpdateAsync(PostReaction reaction); // Adiciona ou atualiza reação
        Task DeleteAsync(int postId, int userId); // Remove reação
    }
}