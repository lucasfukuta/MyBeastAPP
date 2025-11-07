using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Entities;
using MyBeast.Domain.Interfaces;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class CommunityPostRepository : ICommunityPostRepository
    {
        private readonly ApiDbContext _context;

        public CommunityPostRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityPost?> GetByIdAsync(int postId)
        {
            // Inclui dados do autor (User) e contagem de reações (exemplo)
            return await _context.CommunityPosts
                       .Include(p => p.User) // Inclui dados do usuário que postou
                       .Include(p => p.PostReactions) // Inclui as reações para contagem
                       .AsNoTracking()
                       .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<IEnumerable<CommunityPost>> GetAllAsync(int pageNumber, int pageSize)
        {
            // Implementa paginação básica
            return await _context.CommunityPosts
                           .Include(p => p.User) // Inclui dados do autor
                           .Include(p => p.PostReactions) // <-- ADICIONE ESTA LINHA
                           .OrderByDescending(p => p.CreatedAt)
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .AsNoTracking()
                           .ToListAsync();
        }

        public async Task<IEnumerable<CommunityPost>> GetByUserIdAsync(int userId)
        {
            return await _context.CommunityPosts
                           .Where(p => p.UserId == userId)
                           .Include(p => p.User)          // Incluir se necessário
                           .Include(p => p.PostReactions) // <-- ADICIONE ESTA LINHA
                           .OrderByDescending(p => p.CreatedAt)
                           .AsNoTracking()
                           .ToListAsync();
        }

        public async Task<CommunityPost> AddAsync(CommunityPost post)
        {
            // Limpa navegações que não devem ser salvas aqui
            post.User = null;
            post.PostReactions = new List<PostReaction>();

            await _context.CommunityPosts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<CommunityPost> UpdateAsync(CommunityPost post)
        {
            _context.Entry(post).State = EntityState.Modified;
            // Evita atualizar campos sensíveis
            _context.Entry(post).Property(p => p.UserId).IsModified = false;
            _context.Entry(post).Property(p => p.CreatedAt).IsModified = false;
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task DeleteAsync(int postId)
        {
            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post != null)
            {
                _context.CommunityPosts.Remove(post);
                await _context.SaveChangesAsync(); // Cascata deve remover reações
            }
        }
    }
}