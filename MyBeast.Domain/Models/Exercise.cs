using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Domain.Models
{
    [Table("Exercise")]
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 

        [Required]
        [MaxLength(50)]
        public string MuscleGroup { get; set; } 
        public string Instructions { get; set; } 
        public bool IsCustom { get; set; } 

        // --- Propriedades de Navegação ---
        public virtual ICollection<TemplateExercise> TemplateExercises { get; set; } = new List<TemplateExercise>();
        public virtual ICollection<SetLog> SetLogs { get; set; } = new List<SetLog>();
    }
}