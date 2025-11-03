using MyBeast.Domain.Models; // Importa o Modelo de Domínio
using System; // Para DateTime
using System.Threading.Tasks;

namespace MyBeast.Application.Interfaces
{
    public interface IAuthService
    {
        // Retorna uma tupla: (Token, Data de Expiração, Modelo do Usuário)
        Task<(string Token, DateTime Expiration, User User)> LoginAsync(string email, string password);
    }
}