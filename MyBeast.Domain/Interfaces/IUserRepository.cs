using MyBeast.Domain.Entities;
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user); // Novo
        Task DeleteAsync(int id); // Novo
    }
}