using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.Exercise.Input
{
    public class ExerciseCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string MuscleGroup { get; set; } = string.Empty;

        public string? Instructions { get; set; }

        [Required] // O UserId será pego do usuário autenticado no Controller
        public int UserId { get; set; } // Temporário: Remover quando tiver Auth
    }
}