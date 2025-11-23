using MyBeast.Domain.Entities;

namespace MyBeast.Services.Mocks
{
    public class MockExerciseService : IExerciseService
    {
        private readonly List<Exercise> _exercises;

        public MockExerciseService()
        {
            _exercises = new List<Exercise>
            {
                new Exercise { ExerciseId = 1, Name = "Supino Reto", MuscleGroup = "Peito", Instructions = "Deite-se no banco...", IsCustom = false },
                new Exercise { ExerciseId = 2, Name = "Agachamento", MuscleGroup = "Pernas", Instructions = "Pés na largura dos ombros...", IsCustom = false },
                new Exercise { ExerciseId = 3, Name = "Barra Fixa", MuscleGroup = "Costas", Instructions = "Puxe o corpo...", IsCustom = false },
                new Exercise { ExerciseId = 4, Name = "Desenvolvimento", MuscleGroup = "Ombros", Instructions = "Eleve a barra...", IsCustom = false },
                new Exercise { ExerciseId = 5, Name = "Rosca Direta", MuscleGroup = "Bíceps", Instructions = "Flexione os cotovelos...", IsCustom = false },
                // Exercício do usuário mockado
                new Exercise { ExerciseId = 99, Name = "Flexão Diamante", MuscleGroup = "Tríceps", Instructions = "Mãos em formato de diamante", IsCustom = true, UserId = 1 }
            };
        }

        public async Task<IEnumerable<Exercise>> GetAllExercisesAsync()
        {
            await Task.Delay(100);
            return _exercises;
        }

        public async Task<IEnumerable<Exercise>> GetCustomExercisesAsync()
        {
            await Task.Delay(100);
            return _exercises.Where(e => e.IsCustom).ToList();
        }

        public async Task<Exercise?> GetExerciseByIdAsync(int id)
        {
            await Task.Delay(50);
            return _exercises.FirstOrDefault(e => e.ExerciseId == id);
        }

        public async Task<Exercise> AddExerciseAsync(Exercise exercise)
        {
            await Task.Delay(200);
            exercise.ExerciseId = _exercises.Any() ? _exercises.Max(e => e.ExerciseId) + 1 : 1;
            exercise.IsCustom = true;
            exercise.UserId = 1; // Mock User ID
            _exercises.Add(exercise);
            return exercise;
        }

        public async Task DeleteExerciseAsync(int id)
        {
            await Task.Delay(200);
            var item = _exercises.FirstOrDefault(e => e.ExerciseId == id && e.IsCustom);
            if (item != null)
            {
                _exercises.Remove(item);
            }
        }
    }
}