using MyBeast.Domain.Entities;
using System.Threading.Tasks;

namespace MyBeast.Services
{
    public interface IPetService
    {
        Task<Pet?> GetPetByUserIdAsync(int userId);
        Task<Pet?> GetPetByIdAsync(int petId);
        Task<Pet> CreatePetAsync(Pet pet);
        Task AddExperienceAsync(int userId, int experienceGained);
        Task<Pet> UpdatePetStatusAsync(int userId, int health, int energy, int hunger, string status);
        Task<Pet> UpdatePetDetailsAsync(int userId, string? name); // Novo (Atualização geral)
        Task DeletePetByUserIdAsync(int userId); // Novo (Exclusão ligada ao User)
    }
}