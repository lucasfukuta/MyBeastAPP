using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Models
{
    [Table("MealLogItem")]
    public class MealLogItem
    {
        // Chave Primária Composta 
        public int MealLogId { get; set; } 
        public int FoodId { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; } 

        // --- Propriedades de Navegação ---
        [ForeignKey("MealLogId")]
        public virtual MealLog MealLog { get; set; } 
        [ForeignKey("FoodId")]
        public virtual FoodItem FoodItem { get; set; } 
    }
}