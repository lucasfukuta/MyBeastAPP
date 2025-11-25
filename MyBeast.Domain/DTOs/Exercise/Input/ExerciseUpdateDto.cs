using System.ComponentModel.DataAnnotations;

namespace MyBeast.Domain.DTOs.Exercise.Input
{
    public class ExerciseUpdateDto
    {
        // Campos que podem ser atualizados
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? MuscleGroup { get; set; }

        public string? Instructions { get; set; }
    }
}