using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.WorkoutTemplate.Input
{
    //atualizar um template
    public class WorkoutTemplateUpdateDto
    {
        // Campos que podem ser atualizados
        [MaxLength(100)]
        public string? Name { get; set; } // Opcional na atualização

        [MaxLength(50)]
        public string? Difficulty { get; set; } // Opcional

        // A lista completa de exercícios (substitui a existente)
        public List<TemplateExerciseDto>? Exercises { get; set; }
    }
}