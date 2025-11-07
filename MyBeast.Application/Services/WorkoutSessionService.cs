using Microsoft.Extensions.Logging;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Interfaces;
using MyBeast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq; // Para Any() e Count()
using System.Threading.Tasks;

namespace MyBeast.Application.Services
{
    public class WorkoutSessionService : IWorkoutSessionService
    {
        private readonly IWorkoutSessionRepository _sessionRepository;
        private readonly ISetLogRepository _setLogRepository;
        private readonly IUserRepository _userRepository;

        // Dependências para Lógica de Negócio (Gatilhos)
        private readonly IAchievementService _achievementService;
        private readonly IAchievementRepository _achievementRepository; // Para verificar duplicatas
        private readonly IPetService _petService;
        private readonly ILogger<WorkoutSessionService> _logger;

        public WorkoutSessionService(
            IWorkoutSessionRepository sessionRepository,
            ISetLogRepository setLogRepository,
            IUserRepository userRepository,
            IAchievementService achievementService,
            IAchievementRepository achievementRepository,
            IPetService petService,
            ILogger<WorkoutSessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _setLogRepository = setLogRepository;
            _userRepository = userRepository;
            _achievementService = achievementService;
            _achievementRepository = achievementRepository;
            _petService = petService;
            _logger = logger;
        }

        // --- Métodos Públicos (CRUD) ---

        public async Task<WorkoutSession?> GetWorkoutSessionByIdAsync(int sessionId, int requestingUserId)
        {
            // O repositório já inclui SetLogs e Exercícios
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null) return null; // Não encontrado

            // Verificação de Permissão
            if (session.UserId != requestingUserId)
            {
                // TODO: Adicionar lógica para Admin/Moderador se necessário
                throw new Exception("Usuário não tem permissão para ver esta sessão.");
            }

            return session;
        }

        public async Task<IEnumerable<WorkoutSession>> GetWorkoutSessionsByUserIdAsync(int userId)
        {
            // A verificação de permissão (se usuário A pode ver sessões do usuário B)
            // é feita no Controller (ex: /me vs /user/{id})
            return await _sessionRepository.GetByUserIdAsync(userId);
        }

        public async Task<WorkoutSession> StartWorkoutSessionAsync(int requestingUserId, DateTime startTime)
        {
            // Verificar se o usuário existe
            var user = await _userRepository.GetByIdAsync(requestingUserId);
            if (user == null) throw new Exception($"Usuário com ID {requestingUserId} não encontrado.");

            var newSession = new WorkoutSession
            {
                UserId = requestingUserId, // Usa o ID do token
                Date = startTime,
                DurationMinutes = null,
                TotalVolume = 0
            };

            return await _sessionRepository.AddAsync(newSession);
        }

        public async Task<WorkoutSession> EndWorkoutSessionAsync(int sessionId, int requestingUserId, DateTime endTime, decimal totalVolume, List<SetLog> setLogs)
        {
            // 1. Buscar Sessão e Verificar Permissão
            var session = await _sessionRepository.GetByIdAsync(sessionId); // GetByIdAsync simples, sem includes, pois vamos atualizar
            if (session == null) throw new Exception($"Sessão de treino com ID {sessionId} não encontrada.");

            if (session.UserId != requestingUserId)
            {
                throw new Exception("Usuário não tem permissão para finalizar esta sessão.");
            }

            // 2. Atualizar Dados da Sessão
            session.DurationMinutes = (int)(endTime - session.Date).TotalMinutes;
            session.TotalVolume = totalVolume;
            var updatedSession = await _sessionRepository.UpdateAsync(session);

            // 3. Salvar os SetLogs
            if (setLogs != null && setLogs.Any())
            {
                foreach (var log in setLogs)
                {
                    log.SessionId = sessionId; // Garante a FK correta
                }
                await _setLogRepository.AddRangeAsync(setLogs);
            }

            // --- 4. EXECUTAR GATILHOS DE LÓGICA DE NEGÓCIO ---
            // (Envolvemos em try/catch para que falhas aqui não impeçam o salvamento do treino)

            // Gatilho de Conquistas
            try
            {
                await CheckForWorkoutAchievementsAsync(session.UserId);
            }
            catch (Exception ex)
            {
                // Logar erro (substitua Console.WriteLine por ILogger em produção)
                _logger.LogError(ex, "Erro ao conceder conquista para UserId {UserId}", session.UserId);
            }

            // Gatilho de Evolução do Pet
            try
            {
                await UpdatePetStatusAfterWorkoutAsync(session.UserId);
            }
            catch (Exception ex)
            {
                // Logar erro
                _logger.LogError(ex, "Erro ao atualizar status do Pet pós-treino para UserId {UserId}", session.UserId);
            }

            return updatedSession;
        }

        public async Task DeleteWorkoutSessionAsync(int sessionId, int requestingUserId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null) throw new Exception($"Sessão de treino com ID {sessionId} não encontrada.");

            // Verificação de Permissão
            if (session.UserId != requestingUserId)
            {
                // TODO: Adicionar lógica de Admin/Moderador
                throw new Exception("Usuário não tem permissão para deletar esta sessão.");
            }

            // A cascata no banco deve cuidar dos SetLogs
            await _sessionRepository.DeleteAsync(sessionId);
        }


        // --- Métodos Privados (Lógica de Negócio) ---

        private async Task CheckForWorkoutAchievementsAsync(int userId)
        {
            // Define os marcos
            const string ACH_1_WORKOUT = "Primeiro Treino Concluído";
            const string ACH_10_WORKOUTS = "Ritmo de Treino (10 Treinos)";
            const string ACH_50_WORKOUTS = "Consistência (50 Treinos)";
            // (Adicionar mais marcos aqui)

            var allUserSessions = await _sessionRepository.GetByUserIdAsync(userId);
            int workoutCount = allUserSessions.Count();

            // Verifica Marco "Primeiro Treino"
            if (workoutCount >= 1)
            {
                var existing = await _achievementRepository.GetByNameAndUserIdAsync(ACH_1_WORKOUT, userId);
                if (existing == null)
                {
                    await _achievementService.GrantAchievementAsync(userId, ACH_1_WORKOUT, "Você completou seu primeiro treino!");
                }
            }

            // Verifica Marco "10 Treinos"
            if (workoutCount >= 10)
            {
                var existing = await _achievementRepository.GetByNameAndUserIdAsync(ACH_10_WORKOUTS, userId);
                if (existing == null)
                {
                    await _achievementService.GrantAchievementAsync(userId, ACH_10_WORKOUTS, "Você completou 10 treinos! Continue assim!");
                }
            }

            // Verifica Marco "50 Treinos"
            if (workoutCount >= 50)
            {
                var existing = await _achievementRepository.GetByNameAndUserIdAsync(ACH_50_WORKOUTS, userId);
                if (existing == null)
                {
                    await _achievementService.GrantAchievementAsync(userId, ACH_50_WORKOUTS, "50 treinos concluídos! Você é uma inspiração!");
                }
            }
        }

        private async Task UpdatePetStatusAfterWorkoutAsync(int userId)
        {
            // 1. Define a lógica de mudança de status
            int healthChange = +10; // Recompensa por treinar
            int energyChange = -20; // Cansaço do treino
            int hungerChange = +15; // Fome pós-treino
            int xpGained = 50;      // XP por treino

            // 2. Busca o pet atual
            var pet = await _petService.GetPetByUserIdAsync(userId); // O Serviço de Pet lida com o repositório
            if (pet != null)
            {
                // 3. Calcula os novos valores (usando os valores atuais)
                int newHealth = pet.Health + healthChange;
                int newEnergy = pet.Energy + energyChange;
                int newHunger = pet.Hunger + hungerChange;
                string newStatus = (newEnergy < 20) ? "Tired" : "Healthy"; // Lógica de status

                // 4. Chama o serviço do Pet para atualizar status (que já usa Clamp)
                await _petService.UpdatePetStatusAsync(userId, newHealth, newEnergy, newHunger, newStatus);

                // 5. Chama o serviço do Pet para adicionar XP (que lida com Level Up)
                await _petService.AddExperienceAsync(userId, xpGained);
            }
            // else: Usuário não tem pet, não faz nada.
        }
    }
}