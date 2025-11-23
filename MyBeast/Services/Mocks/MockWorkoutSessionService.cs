using MyBeast.Domain.Entities;
using MyBeast.Services;

namespace MyBeast.Services.Mocks
{
    public class MockWorkoutSessionService : IWorkoutSessionService
    {
        private readonly List<WorkoutSession> _sessions;

        public MockWorkoutSessionService()
        {
            // Inicializa com alguns dados fictícios para o histórico aparecer preenchido
            _sessions = new List<WorkoutSession>
            {
                new WorkoutSession
                {
                    SessionId = 1,
                    UserId = 1,
                    Date = DateTime.Now.AddDays(-2), // Treino de 2 dias atrás
                    DurationMinutes = 45,
                    TotalVolume = 1500,
                    SetLogs = new List<SetLog>
                    {
                        new SetLog { SetLogId = 1, ExerciseId = 1, SetNumber = 1, Weight = 60, Reps = 10 },
                        new SetLog { SetLogId = 2, ExerciseId = 1, SetNumber = 2, Weight = 60, Reps = 8 },
                        new SetLog { SetLogId = 3, ExerciseId = 1, SetNumber = 3, Weight = 60, Reps = 7 }
                    }
                },
                new WorkoutSession
                {
                    SessionId = 2,
                    UserId = 1,
                    Date = DateTime.Now.AddDays(-5), // Treino de 5 dias atrás
                    DurationMinutes = 60,
                    TotalVolume = 3200,
                    SetLogs = new List<SetLog>
                    {
                        new SetLog { SetLogId = 4, ExerciseId = 2, SetNumber = 1, Weight = 80, Reps = 10 },
                        new SetLog { SetLogId = 5, ExerciseId = 2, SetNumber = 2, Weight = 80, Reps = 10 },
                        new SetLog { SetLogId = 6, ExerciseId = 2, SetNumber = 3, Weight = 80, Reps = 10 },
                        new SetLog { SetLogId = 7, ExerciseId = 2, SetNumber = 4, Weight = 80, Reps = 10 }
                    }
                }
            };
        }

        public async Task<WorkoutSession> StartSessionAsync(DateTime startTime, int? templateId = null)
        {
            await Task.Delay(100); // Simula latência de rede

            var newSession = new WorkoutSession
            {
                SessionId = _sessions.Any() ? _sessions.Max(s => s.SessionId) + 1 : 1,
                UserId = 1, // Mock User ID fixo
                Date = startTime,
                SetLogs = new List<SetLog>(),
                TotalVolume = 0
            };

            // Em um cenário real, se templateId != null, carregaríamos os exercícios do template aqui.
            // Para o mock, retornamos a sessão vazia pronta para ser preenchida.

            return newSession;
        }

        public async Task<WorkoutSession> FinishSessionAsync(WorkoutSession session)
        {
            await Task.Delay(200); // Simula processamento

            // Garante que a sessão tenha uma duração se não foi setada
            if (session.DurationMinutes == null || session.DurationMinutes == 0)
            {
                var duration = DateTime.Now - session.Date;
                session.DurationMinutes = (int)duration.TotalMinutes;
            }

            // Recalcula o volume total baseando-se nos logs adicionados
            if (session.SetLogs != null && session.SetLogs.Any())
            {
                session.TotalVolume = session.SetLogs.Sum(s => s.Weight * s.Reps);
            }

            // Adiciona ao "banco de dados" em memória se ainda não estiver lá
            var existingSession = _sessions.FirstOrDefault(s => s.SessionId == session.SessionId);
            if (existingSession == null)
            {
                _sessions.Add(session);
            }
            else
            {
                // Atualiza a sessão existente (embora em memória a referência já cuide disso na maioria dos casos)
                existingSession.DurationMinutes = session.DurationMinutes;
                existingSession.TotalVolume = session.TotalVolume;
                existingSession.SetLogs = session.SetLogs;
            }

            return session;
        }

        public async Task<IEnumerable<WorkoutSession>> GetSessionHistoryAsync()
        {
            await Task.Delay(100);
            // Retorna ordenado por data, do mais recente para o mais antigo
            return _sessions.OrderByDescending(s => s.Date);
        }
    }
}