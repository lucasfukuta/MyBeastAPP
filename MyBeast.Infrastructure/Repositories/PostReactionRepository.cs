using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class PostReactionRepository : IPostReactionRepository
    {
        private readonly ApiDbContext _context;

        public PostReactionRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<PostReaction?> GetReactionAsync(int postId, int userId)
        {
            return await _context.PostReactions
                       .FirstOrDefaultAsync(pr => pr.PostId == postId && pr.UserId == userId);
        }

        public async Task<IEnumerable<PostReaction>> GetReactionsByPostIdAsync(int postId)
        {
            return await _context.PostReactions
                      .Where(pr => pr.PostId == postId)
                      .AsNoTracking()
                      .ToListAsync();
        }

        public async Task<PostReaction> AddOrUpdateAsync(PostReaction reaction)
        {
            var existing = await _context.PostReactions
                            .FirstOrDefaultAsync(pr => pr.PostId == reaction.PostId && pr.UserId == reaction.UserId);

            if (existing != null)
            {
                // Atualiza o tipo se já existe
                existing.ReactionType = reaction.ReactionType;
                _context.PostReactions.Update(existing);
            }
            else
            {
                // Adiciona se for nova
                await _context.PostReactions.AddAsync(reaction);
            }
            await _context.SaveChangesAsync();
            return reaction; // Ou 'existing' se atualizou
        }

        public async Task DeleteAsync(int postId, int userId)
        {
            var reaction = await _context.PostReactions
                            .FirstOrDefaultAsync(pr => pr.PostId == postId && pr.UserId == userId);
            if (reaction != null)
            {
                _context.PostReactions.Remove(reaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}