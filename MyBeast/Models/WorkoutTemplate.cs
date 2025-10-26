using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Models
{
    [Table("WorkoutTemplate")]
    public class WorkoutTemplate
    {
        [Key]
        public int TemplateId { get; set; } 
        public int? UserId { get; set; } // Permite NULO
        [ForeignKey("UserId")]
        public virtual User User { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 
        [MaxLength(50)]
        public string Difficulty { get; set; } 
        public bool IsPremium { get; set; } 

        // --- Propriedades de Navegação ---
        public virtual ICollection<TemplateExercise> TemplateExercises { get; set; } = new List<TemplateExercise>();
    }
}