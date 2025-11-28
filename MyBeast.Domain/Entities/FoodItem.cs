using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Opcional

namespace MyBeast.Domain.Entities
{
    [Table("FoodItem")]
    public class FoodItem
    {
        [Key]
        public int FoodId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        // --- MUDANÇA: De int para decimal (para bater com o SQL Server) ---
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }

        public bool IsCustom { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [JsonIgnore] // Evita ciclos ao serializar se necessário
        public virtual ICollection<MealLogItem> MealLogItems { get; set; } = new List<MealLogItem>();
    }
}