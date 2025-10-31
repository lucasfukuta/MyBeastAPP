using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.WorkoutTemplate.Input
{
    //criar um template de treino completo
    public class WorkoutTemplateCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Difficulty { get; set; }

        // Lista de exercícios que compõem o template
        public List<TemplateExerciseDto> Exercises { get; set; } = new List<TemplateExerciseDto>();
    }
}