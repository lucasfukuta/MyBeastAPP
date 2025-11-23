using MyBeast.Domain.Entities;

namespace MyBeast.Services.Mocks
{
    public class MockWorkoutTemplateService : IWorkoutTemplateService
    {
        private readonly List<WorkoutTemplate> _templates;

        public MockWorkoutTemplateService()
        {
            _templates = new List<WorkoutTemplate>();
            SeedData();
        }

        private void SeedData()
        {
            // Ficha 1: Treino A
            var templateA = new WorkoutTemplate
            {
                WorkoutTemplateId = 1,
                Name = "Treino A (Peito e Tríceps)",
                Difficulty = "Intermediário",
                IsPremium = false,
                UserId = 1,
                TemplateExercises = new List<TemplateExercise>()
            };

            // Adicionando exercícios ao Template A
            // NOTA: Não definimos Sets ou Reps, pois sua entidade não possui esses campos.
            // Definimos apenas a Ordem (OrderIndex) e o Exercício vinculado.
            templateA.TemplateExercises.Add(new TemplateExercise
            {
                TemplateId = 1,
                ExerciseId = 1,
                OrderIndex = 1,
                Exercise = new Exercise { ExerciseId = 1, Name = "Supino Reto", MuscleGroup = "Peito" }
            });

            templateA.TemplateExercises.Add(new TemplateExercise
            {
                TemplateId = 1,
                ExerciseId = 99,
                OrderIndex = 2,
                Exercise = new Exercise { ExerciseId = 99, Name = "Flexão Diamante", MuscleGroup = "Tríceps" }
            });

            // Ficha 2: Full Body
            var templateB = new WorkoutTemplate
            {
                WorkoutTemplateId = 2,
                Name = "Full Body Iniciante",
                Difficulty = "Fácil",
                IsPremium = false,
                UserId = null, // Ficha do sistema
                TemplateExercises = new List<TemplateExercise>()
            };

            templateB.TemplateExercises.Add(new TemplateExercise
            {
                TemplateId = 2,
                ExerciseId = 2,
                OrderIndex = 1,
                Exercise = new Exercise { ExerciseId = 2, Name = "Agachamento", MuscleGroup = "Pernas" }
            });

            templateB.TemplateExercises.Add(new TemplateExercise
            {
                TemplateId = 2,
                ExerciseId = 3,
                OrderIndex = 2,
                Exercise = new Exercise { ExerciseId = 3, Name = "Barra Fixa", MuscleGroup = "Costas" }
            });

            _templates.Add(templateA);
            _templates.Add(templateB);
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetAllTemplatesAsync()
        {
            await Task.Delay(150);
            return _templates;
        }

        public async Task<WorkoutTemplate?> GetTemplateByIdAsync(int id)
        {
            await Task.Delay(100);
            return _templates.FirstOrDefault(t => t.WorkoutTemplateId == id);
        }

        public async Task<WorkoutTemplate> CreateTemplateAsync(WorkoutTemplate template)
        {
            await Task.Delay(300);
            template.WorkoutTemplateId = _templates.Any() ? _templates.Max(t => t.WorkoutTemplateId) + 1 : 1;
            template.UserId = 1; // Mock user ID

            if (template.TemplateExercises != null)
            {
                foreach (var item in template.TemplateExercises)
                {
                    // Garante que a chave estrangeira do template esteja correta
                    item.TemplateId = template.WorkoutTemplateId;

                    // Num app real, buscaríamos o objeto Exercise completo do banco
                    // No mock, se o Exercise vier nulo, pode dar erro na UI, então
                    // idealmente garantiríamos que item.Exercise tenha pelo menos o Nome.
                }
            }

            _templates.Add(template);
            return template;
        }

        public async Task DeleteTemplateAsync(int id)
        {
            await Task.Delay(200);
            var item = _templates.FirstOrDefault(t => t.WorkoutTemplateId == id);
            if (item != null)
            {
                _templates.Remove(item);
            }
        }
    }
}