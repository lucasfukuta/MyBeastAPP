using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
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

        public async Task<CommunityPost> CreatePostAsync(CommunityPost post)
        {
            // Verificar usuário
            var user = await _userRepository.GetByIdAsync(post.UserId);
            if (user == null) throw new Exception($"Usuário com ID {post.UserId} não encontrado.");

            // Validar dados do post (título, conteúdo, tipo)
            if (string.IsNullOrWhiteSpace(post.Title)) throw new ArgumentException("Título é obrigatório.");
            if (string.IsNullOrWhiteSpace(post.Type)) throw new ArgumentException("Tipo de post é obrigatório."); // Ex: 'Photo', 'Discussion'

            post.CreatedAt = DateTime.UtcNow;
            return await _postRepository.AddAsync(post);
        }

        public async Task<CommunityPost> UpdatePostAsync(int postId, string title, string content)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) throw new Exception($"Post com ID {postId} não encontrado.");

            // Adicionar verificação de permissão (se o usuário logado é o dono do post)

            if (!string.IsNullOrWhiteSpace(title)) post.Title = title;
            if (content != null) post.Content = content; // Permite conteúdo vazio

            return await _postRepository.UpdateAsync(post);
        }


        public async Task DeletePostAsync(int postId, int userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) throw new Exception($"Post com ID {postId} não encontrado.");

            // Verificar permissão (usuário é dono do post ou é moderador)
            var user = await _userRepository.GetByIdAsync(userId); // Assumindo que temos o ID do usuário logado
            if (user == null) throw new Exception("Usuário não encontrado."); // Segurança
            if (post.UserId != userId && !user.IsModerator)
            {
                throw new Exception("Usuário não tem permissão para deletar este post.");
            }

            await _postRepository.DeleteAsync(postId);
        }

        public async Task ReactToPostAsync(int postId, int userId, string reactionType)
        {
            // Validar reactionType (Upvote, Downvote, Report)
            var validTypes = new[] { "Upvote", "Downvote", "Report" };
            if (!validTypes.Contains(reactionType)) throw new ArgumentException("Tipo de reação inválido.");

            // Verificar se usuário e post existem
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) throw new Exception($"Post com ID {postId} não encontrado.");


            var existingReaction = await _reactionRepository.GetReactionAsync(postId, userId);

            if (existingReaction != null)
            {
                // Usuário já reagiu
                if (existingReaction.ReactionType == reactionType)
                {
                    // Clicou de novo no mesmo botão: remove a reação
                    await _reactionRepository.DeleteAsync(postId, userId);
                }
                else
                {
                    // Mudou a reação (ex: de Upvote para Downvote)
                    existingReaction.ReactionType = reactionType;
                    await _reactionRepository.AddOrUpdateAsync(existingReaction);
                }
            }
            else
            {
                // Nova reação
                var newReaction = new PostReaction
                {
                    PostId = postId,
                    UserId = userId,
                    ReactionType = reactionType
                };
                await _reactionRepository.AddOrUpdateAsync(newReaction);
            }
        }
    }
}