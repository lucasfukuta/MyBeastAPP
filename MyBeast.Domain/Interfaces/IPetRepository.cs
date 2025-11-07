using MyBeast.Domain.Entities;
using System.Threading.Tasks;

namespace MyBeast.Domain.Interfaces
{
    public interface IPetRepository
    {
        Task<Pet?> GetByUserIdAsync(int userId);
        Task<Pet?> GetByIdAsync(int petId);
        Task<Pet> AddAsync(Pet pet);
        Task<Pet> UpdateAsync(Pet pet);
        Task DeleteAsync(int petId); // Novo
    }
}