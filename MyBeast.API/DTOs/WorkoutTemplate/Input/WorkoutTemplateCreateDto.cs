using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.WorkoutTemplate.Input
{
    public class WorkoutTemplateCreateDto
    {
        // UserId FOI REMOVIDO. O servidor pegará do token.

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Difficulty { get; set; }

        public List<TemplateExerciseDto> Exercises { get; set; } = new List<TemplateExerciseDto>();
    }
}