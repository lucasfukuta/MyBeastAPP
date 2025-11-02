using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq; // Para Any()
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class WorkoutSessionService : IWorkoutSessionService
    {
        private readonly IWorkoutSessionRepository _sessionRepository;
        private readonly ISetLogRepository _setLogRepository;
        private readonly IUserRepository _userRepository; // Para verificar se o usuário existe
        private readonly IAchievementService _achievementService;
        private readonly IAchievementRepository _achievementRepository;
        private readonly IPetService _petService;

        public WorkoutSessionService(
            IWorkoutSessionRepository sessionRepository,
            ISetLogRepository setLogRepository,
            IUserRepository userRepository,
            IAchievementService achievementService,
            IAchievementRepository achievementRepository,
            IPetService petService)
        {
            _sessionRepository = sessionRepository;
            _setLogRepository = setLogRepository;
            _userRepository = userRepository;
            _achievementService = achievementService;
            _achievementRepository = achievementRepository;
            _petService = petService;
        }

        public async Task<WorkoutSession?> GetWorkoutSessionByIdAsync(int sessionId)
        {
            // Poderíamos incluir os SetLogs aqui se necessário
            return await _sessionRepository.GetByIdAsync(sessionId);
        }

        public async Task<IEnumerable<WorkoutSession>> GetWorkoutSessionsByUserIdAsync(int userId)
        {
            return await _sessionRepository.GetByUserIdAsync(userId);
        }

        public async Task<WorkoutSession> StartWorkoutSessionAsync(int userId, DateTime startTime)
        {
            // Verificar usuário
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception($"Usuário com ID {userId} não encontrado.");

            var newSession = new WorkoutSession
            {
                UserId = userId,
                Date = startTime, // Usamos a data/hora de início
                DurationMinutes = null, // Será definido no fim
                TotalVolume = 0 // Será calculado no fim
            };

            return await _sessionRepository.AddAsync(newSession);
        }

        public async Task<WorkoutSession> EndWorkoutSessionAsync(int sessionId, DateTime endTime, decimal totalVolume, List<SetLog> setLogs)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null) throw new Exception($"Sessão de treino com ID {sessionId} não encontrada.");

            // Atualizar dados da sessão
            session.DurationMinutes = (int)(endTime - session.Date).TotalMinutes;
            session.TotalVolume = totalVolume;

            var updatedSession = await _sessionRepository.UpdateAsync(session);

            // Adicionar os SetLogs associados à sessão
            if (setLogs != null && setLogs.Any())
            {
                foreach (var log in setLogs)
                {
                    log.SessionId = sessionId; // Garante a FK correta
                }
                await _setLogRepository.AddRangeAsync(setLogs);
            }
            try
            {
                await CheckForWorkoutAchievementsAsync(session.UserId);
            }
            catch (Exception ex)
            {
                // Logar o erro (ex: Console.WriteLine($"Erro ao conceder conquista: {ex.Message}"))
                // Mas não lançar a exceção, pois o treino foi salvo com sucesso.
            }
            try
            {
                // 1. Define a lógica de mudança de status
                int healthChange = +10; // Recompensa por treinar
                int energyChange = -20; // Cansaço do treino
                int hungerChange = +15; // Fome pós-treino

                // 2. Busca o pet atual
                var pet = await _petService.GetPetByUserIdAsync(session.UserId);
                if (pet != null)
                {
                    // 3. Calcula os novos valores (usando os valores atuais)
                    int newHealth = pet.Health + healthChange;
                    int newEnergy = pet.Energy + energyChange;
                    int newHunger = pet.Hunger + hungerChange;

                    // 4. Chama o serviço do Pet (que já tem a lógica de Clamp)
                    await _petService.UpdatePetStatusAsync(session.UserId, newHealth, newEnergy, newHunger, "Healthy"); // Ou "Tired"?
                    await _petService.AddExperienceAsync(session.UserId, 50);
                }
            }
            catch (Exception ex)
            {
                // Logar erro do Pet
                Console.WriteLine($"Erro ao atualizar status do Pet pós-treino: {ex.Message}");
            }

            return updatedSession; // Retorna a sessão atualizada
        }

        public async Task DeleteWorkoutSessionAsync(int sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null) throw new Exception($"Sessão de treino com ID {sessionId} não encontrada.");

            // A exclusão em cascata deve cuidar dos SetLogs (se configurado no DbContext)
            await _sessionRepository.DeleteAsync(sessionId);
        }
        private async Task CheckForWorkoutAchievementsAsync(int userId)
        {
            // 1. Define os marcos
            const string ACHIEVEMENT_1_WORKOUT = "Primeiro Treino Concluído";
            const string ACHIEVEMENT_10_WORKOUTS = "Ritmo de Treino (10 Treinos)";
            // Adicionar mais marcos (50, 100, etc.)

            // 2. Obtém o total de treinos do usuário
            var allUserSessions = await _sessionRepository.GetByUserIdAsync(userId);
            int workoutCount = allUserSessions.Count();

            // 3. Verifica o Marco "Primeiro Treino"
            if (workoutCount >= 1)
            {
                // Verifica se ele já NÃO tem essa conquista
                var existing = await _achievementRepository.GetByNameAndUserIdAsync(ACHIEVEMENT_1_WORKOUT, userId);
                if (existing == null)
                {
                    // Concede a conquista!
                    await _achievementService.GrantAchievementAsync(userId, ACHIEVEMENT_1_WORKOUT, "Você completou seu primeiro treino!");
                }
            }

            // 4. Verifica o Marco "10 Treinos"
            if (workoutCount >= 10)
            {
                var existing = await _achievementRepository.GetByNameAndUserIdAsync(ACHIEVEMENT_10_WORKOUTS, userId);
                if (existing == null)
                {
                    await _achievementService.GrantAchievementAsync(userId, ACHIEVEMENT_10_WORKOUTS, "Você completou 10 treinos! Continue assim!");
                }
            }

            // 5. Adicionar outras verificações (ex: volume total, etc.)
        }
    }
}