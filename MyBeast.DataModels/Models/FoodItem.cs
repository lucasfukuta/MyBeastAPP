using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Models
{
    [Table("FoodItem")]
    public class FoodItem
    {
        [Key]
        public int FoodId { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Calories { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Protein { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Carbs { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Fat { get; set; } 
        public bool IsCustom { get; set; } 

        // --- Propriedades de Navegação ---
        public virtual ICollection<MealLogItem> MealLogItems { get; set; }
    }
}