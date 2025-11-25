using MyBeast.Models.DTOs; // Referência ao DTO criado acima

namespace MyBeast.Models
{
    public class WorkoutExercise
    {
        // Propriedades Visuais
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MuscleGroup { get; set; }

        // Propriedades de Execução (Sets/Reps)
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }

        public WorkoutExercise() { }

        // Recebe ExerciseDto em vez da Entidade
        public WorkoutExercise(ExerciseDto dto, int sets, int reps)
        {
            ExerciseId = dto.ExerciseId;
            Name = dto.Name;
            Description = dto.Instructions ?? "Sem instruções";
            MuscleGroup = dto.MuscleGroup;
            Sets = sets;
            Reps = reps;
            Weight = 0;
        }
    }
}