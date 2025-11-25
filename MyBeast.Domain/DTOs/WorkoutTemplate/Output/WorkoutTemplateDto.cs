namespace MyBeast.Domain.DTOs.WorkoutTemplate.Output
{
    // DTO para retornar dados resumidos de um WorkoutTemplate (para listas)
    public class WorkoutTemplateDto
    {
        public int TemplateId { get; set; }
        public int? UserId { get; set; } // Identifica se é template padrão (null) ou de usuário
        public string Name { get; set; } = string.Empty;
        public string? Difficulty { get; set; }
        public bool IsPremium { get; set; }
        public int ExerciseCount { get; set; } // Adicionado: Contagem de exercícios
    }
}