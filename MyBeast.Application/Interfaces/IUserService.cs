using MyBeast.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync(); // Manter por enquanto, mas remover hash
        Task<User?> GetUserByIdAsync(int id); // Remover hash
        Task<User?> GetUserByEmailAsync(string email); // Usado internamente para login/registro
        Task<User> RegisterUserAsync(User user); // Remover hash da resposta
        Task<User> UpdateUserProfileAsync(int id, string? username, string? email); // Novo
        Task DeleteUserAsync(int id); // Novo
        // (Adicionaremos LoginAsync depois)
    }
}