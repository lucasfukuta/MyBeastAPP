using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    public interface ICommunityPostService
    {
        Task<CommunityPost?> GetPostByIdAsync(int postId);
        Task<IEnumerable<CommunityPost>> GetCommunityFeedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<CommunityPost>> GetPostsByUserIdAsync(int userId);

        // Assinaturas Atualizadas (recebem 'requestingUserId')
        Task<CommunityPost> CreatePostAsync(CommunityPost post, int requestingUserId);
        Task<CommunityPost> UpdatePostAsync(int postId, string title, string content, int requestingUserId);
        Task DeletePostAsync(int postId, int requestingUserId);
        Task ReactToPostAsync(int postId, int requestingUserId, string reactionType);
    }
}