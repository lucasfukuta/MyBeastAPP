using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Threading.Tasks; // Verifique este using

namespace MyBeast.Application.Services
{
    public class PetService : IPetService // Garante que implementa a interface
    {
        private readonly IPetRepository _petRepository;
        private readonly IUserRepository _userRepository;

        public PetService(IPetRepository petRepository, IUserRepository userRepository)
        {
            _petRepository = petRepository;
            _userRepository = userRepository;
        }

        // --- IMPLEMENTAÇÕES ---
        public async Task<Pet?> GetPetByUserIdAsync(int userId) // async Task<Pet?>
        {
            return await _petRepository.GetByUserIdAsync(userId);
        }

        public async Task<Pet?> GetPetByIdAsync(int petId) // async Task<Pet?>
        {
            return await _petRepository.GetByIdAsync(petId);
        }

        public async Task<Pet> CreatePetAsync(Pet pet) // async Task<Pet>
        {
            if (pet == null) throw new ArgumentNullException(nameof(pet));
            if (pet.UserId <= 0) throw new ArgumentException("UserId inválido.", nameof(pet.UserId));
            if (string.IsNullOrWhiteSpace(pet.Name)) throw new ArgumentException("Nome do Pet é obrigatório.", nameof(pet.Name));

            var userExists = await _userRepository.GetByIdAsync(pet.UserId);
            if (userExists == null) throw new Exception($"Usuário com ID {pet.UserId} não encontrado.");

            var existingPet = await _petRepository.GetByUserIdAsync(pet.UserId);
            if (existingPet != null) throw new Exception($"Usuário {pet.UserId} já possui um Pet.");

            pet.EvolutionLevel = 1;
            pet.Health = 100;
            pet.Energy = 100;
            pet.Hunger = 100;
            pet.Status = "Healthy";

            return await _petRepository.AddAsync(pet);
        }

        public async Task<Pet> UpdatePetStatusAsync(int userId, int health, int energy, int hunger, string status) // async Task<Pet>
        {
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet == null) throw new Exception($"Pet não encontrado para o usuário {userId}.");

            pet.Health = Math.Clamp(health, 0, 100);
            pet.Energy = Math.Clamp(energy, 0, 100);
            pet.Hunger = Math.Clamp(hunger, 0, 100);
            pet.Status = string.IsNullOrWhiteSpace(status) ? pet.Status : status;

            return await _petRepository.UpdateAsync(pet);
        }

        public async Task<Pet> UpdatePetDetailsAsync(int userId, string? name) // async Task<Pet>
        {
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet == null) throw new Exception($"Pet não encontrado para o usuário {userId}.");

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (name.Length > 50) throw new ArgumentException("Nome do Pet não pode exceder 50 caracteres.");
                pet.Name = name;
            }

            return await _petRepository.UpdateAsync(pet);
        }

        public async Task DeletePetByUserIdAsync(int userId) // async Task
        {
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet != null)
            {
                await _petRepository.DeleteAsync(pet.PetId);
            }
            // else { Console.WriteLine($"Tentativa de deletar pet para usuário {userId}, mas nenhum pet foi encontrado."); } // Opcional
        }
    }
}