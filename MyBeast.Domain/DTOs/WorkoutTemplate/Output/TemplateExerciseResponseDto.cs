namespace MyBeast.Domain.DTOs.WorkoutTemplate.Output
{
    // DTO para retornar dados de um exercício dentro de um template
    public class TemplateExerciseResponseDto
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty; // Nome do Exercício
        public int OrderIndex { get; set; }
        // Adicionar MuscleGroup se útil?
        // public string MuscleGroup { get; set; } = string.Empty;
    }
}