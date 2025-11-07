using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MyBeast.Domain.Entities;

namespace MyBeast.Domain.Entities
{
    [Table("Exercise")]
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Corrigido com null forgiving

        [Required]
        [MaxLength(50)]
        public string MuscleGroup { get; set; } = null!; // Corrigido com null forgiving

        public string? Instructions { get; set; } // Pode ser nulo/vazio

        public bool IsCustom { get; set; }

        // --- NOVO: Associação com Usuário ---
        public int? UserId { get; set; } // Anulável: Nulo para templates, preenchido para customizados
        [ForeignKey("UserId")]
        public virtual User? User { get; set; } // Propriedade de navegação
        // --- FIM DA ADIÇÃO ---


        // --- Propriedades de Navegação (Já existentes) ---
        public virtual ICollection<TemplateExercise> TemplateExercises { get; set; } = new List<TemplateExercise>(); // Inicializado
        public virtual ICollection<SetLog> SetLogs { get; set; } = new List<SetLog>(); // Inicializado
    }
}