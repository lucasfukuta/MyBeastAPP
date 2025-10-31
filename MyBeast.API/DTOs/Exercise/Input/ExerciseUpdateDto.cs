using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.Exercise.Input
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