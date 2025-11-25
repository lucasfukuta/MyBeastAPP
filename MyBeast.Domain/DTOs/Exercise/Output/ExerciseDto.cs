namespace MyBeast.Domain.DTOs.Exercise.Output // Verifique o namespace
{
    // DTO para retornar dados de um Exercise
    public class ExerciseDto
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty;
        public string? Instructions { get; set; }
        public bool IsCustom { get; set; }
        public int? UserId { get; set; } // Identifica o criador (se custom)

        // Não incluir propriedades de navegação (User, TemplateExercises, SetLogs)
    }
}