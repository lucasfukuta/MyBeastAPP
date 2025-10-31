using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.WorkoutTemplate.Input
{
    // DTO para representar um exercício dentro de um template
    public class TemplateExerciseDto
    {
        [Required]
        public int ExerciseId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Ordem deve ser positiva.")]
        public int OrderIndex { get; set; }
    }
}