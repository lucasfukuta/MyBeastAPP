using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Models
{
    [Table("TemplateExercise")]
    public class TemplateExercise
    {
        // Chave Primária Composta 
        public int TemplateId { get; set; } 
        public int ExerciseId { get; set; } 

        public int OrderIndex { get; set; } 

        // --- Propriedades de Navegação ---
        [ForeignKey("TemplateId")]
        public virtual WorkoutTemplate WorkoutTemplate { get; set; } 
        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; } 
    }
}