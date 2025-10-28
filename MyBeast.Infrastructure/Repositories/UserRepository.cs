using Microsoft.EntityFrameworkCore;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq; // Para Select
using System.Threading.Tasks;

namespace MyBeast.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiDbContext _context;

        public UserRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // NUNCA RETORNAR O HASH DA SENHA EM LISTAS
            return await _context.Users.AsNoTracking()
                       .Select(u => new User
                       {
                           UserId = u.UserId,
                           Username = u.Username,
                           Email = u.Email,
                           PlanType = u.PlanType,
                           IsModerator = u.IsModerator,
                           CreatedAt = u.CreatedAt
                           // Omitir PasswordHash
                           // Omitir propriedades de navegação se não forem necessárias aqui
                       })
                       .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            // Retorna o objeto completo para o Serviço (incluindo o hash se necessário para validação interna)
            return await _context.Users.AsNoTracking()
                       .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            // Retorna o objeto completo para o Serviço (incluindo o hash para login/registro)
            return await _context.Users.AsNoTracking()
                      .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // --- NOVOS MÉTODOS ---
        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            // Garante que o Hash da Senha não seja modificado acidentalmente aqui
            // (A mudança de senha deve ter um fluxo/método separado)
            _context.Entry(user).Property(u => u.PasswordHash).IsModified = false;
            _context.Entry(user).Property(u => u.CreatedAt).IsModified = false; // Não modificar data de criação
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(); // A cascata deve cuidar do Pet, Achievements, etc.
            }
        }
        // --- FIM DOS NOVOS MÉTODOS ---
    }
}