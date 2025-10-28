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

        public WorkoutSessionService(
            IWorkoutSessionRepository sessionRepository,
            ISetLogRepository setLogRepository,
            IUserRepository userRepository)
        {
            _sessionRepository = sessionRepository;
            _setLogRepository = setLogRepository;
            _userRepository = userRepository;
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

            return updatedSession; // Retorna a sessão atualizada
        }

        public async Task DeleteWorkoutSessionAsync(int sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null) throw new Exception($"Sessão de treino com ID {sessionId} não encontrada.");

            // A exclusão em cascata deve cuidar dos SetLogs (se configurado no DbContext)
            await _sessionRepository.DeleteAsync(sessionId);
        }
    }
}