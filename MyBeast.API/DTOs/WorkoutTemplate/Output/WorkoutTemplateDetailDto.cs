using System.Collections.Generic;

namespace MyBeast.API.DTOs.WorkoutTemplate.Output
{
    // DTO para retornar dados detalhados de um WorkoutTemplate (incluindo exercícios)
    public class WorkoutTemplateDetailDto
    {
        public int TemplateId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Difficulty { get; set; }
        public bool IsPremium { get; set; }
        public List<TemplateExerciseResponseDto> Exercises { get; set; } = new List<TemplateExerciseResponseDto>(); // Lista de exercícios detalhados
    }
}