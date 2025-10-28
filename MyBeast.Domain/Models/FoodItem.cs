using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Para ICollection

namespace MyBeast.Domain.Models
{
    [Table("FoodItem")]
    public class FoodItem
    {
        [Key]
        public int FoodId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Corrigido

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Calories { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Protein { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Carbs { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Fat { get; set; }

        public bool IsCustom { get; set; }

        // --- NOVO: Associação com Usuário ---
        public int? UserId { get; set; } // Anulável: Nulo para templates, preenchido para customizados
        [ForeignKey("UserId")]
        public virtual User? User { get; set; } // Propriedade de navegação
        // --- FIM DA ADIÇÃO ---


        // --- Propriedades de Navegação (Já existentes) ---
        public virtual ICollection<MealLogItem> MealLogItems { get; set; } = new List<MealLogItem>(); // Inicializado
    }
}