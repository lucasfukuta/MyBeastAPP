using MyBeast.Application.Interfaces;
using MyBeast.Domain.Entities;
using MyBeast.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IUserRepository _userRepository; // Para verificar usuário

        public AchievementService(IAchievementRepository achievementRepository, IUserRepository userRepository)
        {
            _achievementRepository = achievementRepository;
            _userRepository = userRepository;
        }

        public async Task<Achievement?> GetAchievementByIdAsync(int achievementId)
        {
            return await _achievementRepository.GetByIdAsync(achievementId);
        }

        public async Task<IEnumerable<Achievement>> GetAchievementsByUserIdAsync(int userId)
        {
            return await _achievementRepository.GetByUserIdAsync(userId);
        }

        public async Task<Achievement> GrantAchievementAsync(int userId, string name, string description)
        {
            // Verificar usuário
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            // Validar dados
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome da conquista é obrigatório.");

            // Opcional: Verificar se o usuário já tem essa conquista específica?

            var newAchievement = new Achievement
            {
                UserId = userId,
                Name = name,
                Description = description ?? string.Empty, // Garante não nulo
                DateAchieved = DateTime.UtcNow
            };

            return await _achievementRepository.AddAsync(newAchievement);
        }
    }
}