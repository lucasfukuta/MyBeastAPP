using MyBeast.Application.Interfaces;
using MyBeast.Domain.Entities;
using MyBeast.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class CommunityPostService : ICommunityPostService
    {
        private readonly ICommunityPostRepository _postRepository;
        private readonly IPostReactionRepository _reactionRepository;
        private readonly IUserRepository _userRepository; // Para verificar usuário

        public CommunityPostService(
            ICommunityPostRepository postRepository,
            IPostReactionRepository reactionRepository,
            IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _reactionRepository = reactionRepository;
            _userRepository = userRepository;
        }

        public async Task<CommunityPost?> GetPostByIdAsync(int postId)
        {
            // Poderíamos incluir informações do autor (User) e reações aqui
            return await _postRepository.GetByIdAsync(postId);
        }

        public async Task<IEnumerable<CommunityPost>> GetCommunityFeedAsync(int pageNumber, int pageSize)
        {
            // Adicionar lógica de ordenação (ex: mais recentes, mais votados)
            return await _postRepository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<CommunityPost>> GetPostsByUserIdAsync(int userId)
        {
            return await _postRepository.GetByUserIdAsync(userId);
        }

        public async Task<CommunityPost> CreatePostAsync(CommunityPost post, int requestingUserId)
        {
            // Verificar usuário
            var user = await _userRepository.GetByIdAsync(requestingUserId);
            if (user == null) throw new Exception($"Usuário com ID {requestingUserId} não encontrado.");

            // Atribui o UserId do token
            post.UserId = requestingUserId;

            // Validar dados do post
            if (string.IsNullOrWhiteSpace(post.Title)) throw new ArgumentException("Título é obrigatório.");
            if (string.IsNullOrWhiteSpace(post.Type)) throw new ArgumentException("Tipo de post é obrigatório.");

            post.CreatedAt = DateTime.UtcNow;
            return await _postRepository.AddAsync(post);
        }

        // MÉTODO ATUALIZADO (recebe requestingUserId)
        public async Task<CommunityPost> UpdatePostAsync(int postId, string title, string content, int requestingUserId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) throw new Exception($"Post com ID {postId} não encontrado.");

            // Verificação de Permissão
            if (post.UserId != requestingUserId)
            {
                // (Poderia adicionar lógica de Admin aqui: "&& !user.IsModerator")
                throw new Exception("Usuário não tem permissão para editar este post.");
            }

            if (!string.IsNullOrWhiteSpace(title)) post.Title = title;
            if (content != null) post.Content = content;

            return await _postRepository.UpdateAsync(post);
        }

        // MÉTODO ATUALIZADO (recebe requestingUserId)
        public async Task DeletePostAsync(int postId, int requestingUserId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) throw new Exception($"Post com ID {postId} não encontrado.");

            // Verificar permissão
            var user = await _userRepository.GetByIdAsync(requestingUserId);
            if (user == null) throw new Exception("Usuário não encontrado.");

            if (post.UserId != requestingUserId && !user.IsModerator)
            {
                throw new Exception("Usuário não tem permissão para deletar este post.");
            }

            await _postRepository.DeleteAsync(postId);
        }

        // MÉTODO ATUALIZADO (recebe requestingUserId)
        public async Task ReactToPostAsync(int postId, int requestingUserId, string reactionType)
        {
            var validTypes = new[] { "Upvote", "Downvote", "Report" };
            if (!validTypes.Contains(reactionType)) throw new ArgumentException("Tipo de reação inválido.");

            // Verificar se usuário e post existem
            var user = await _userRepository.GetByIdAsync(requestingUserId);
            if (user == null) throw new Exception($"Usuário com ID {requestingUserId} não encontrado.");
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) throw new Exception($"Post com ID {postId} não encontrado.");

            var existingReaction = await _reactionRepository.GetReactionAsync(postId, requestingUserId);

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == reactionType)
                {
                    await _reactionRepository.DeleteAsync(postId, requestingUserId);
                }
                else
                {
                    existingReaction.ReactionType = reactionType;
                    await _reactionRepository.AddOrUpdateAsync(existingReaction);
                }
            }
            else
            {
                var newReaction = new PostReaction
                {
                    PostId = postId,
                    UserId = requestingUserId, // Usa o ID do token
                    ReactionType = reactionType
                };
                await _reactionRepository.AddOrUpdateAsync(newReaction);
            }
        }
    }
}