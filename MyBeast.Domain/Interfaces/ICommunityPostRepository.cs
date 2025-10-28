using MyBeast.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    // Contrato para acessar dados de CommunityPost
    public interface ICommunityPostRepository
    {
        Task<CommunityPost?> GetByIdAsync(int postId);
        Task<IEnumerable<CommunityPost>> GetAllAsync(int pageNumber, int pageSize); // Para paginação do feed
        Task<IEnumerable<CommunityPost>> GetByUserIdAsync(int userId);
        Task<CommunityPost> AddAsync(CommunityPost post);
        Task<CommunityPost> UpdateAsync(CommunityPost post); // Para editar post (se permitido)
        Task DeleteAsync(int postId);
    }
}