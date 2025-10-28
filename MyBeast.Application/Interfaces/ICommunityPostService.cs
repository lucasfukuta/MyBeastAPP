using MyBeast.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    // Contrato para a lógica de negócio de CommunityPost
    public interface ICommunityPostService
    {
        Task<CommunityPost?> GetPostByIdAsync(int postId);
        Task<IEnumerable<CommunityPost>> GetCommunityFeedAsync(int pageNumber, int pageSize); // Feed principal
        Task<IEnumerable<CommunityPost>> GetPostsByUserIdAsync(int userId);
        Task<CommunityPost> CreatePostAsync(CommunityPost post);
        Task<CommunityPost> UpdatePostAsync(int postId, string title, string content); // Editar
        Task DeletePostAsync(int postId, int userId); // Verificar permissão
        Task ReactToPostAsync(int postId, int userId, string reactionType); // Reagir (Upvote/Downvote/Report)
    }
}