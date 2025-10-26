using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Models
{
    [Table("MealLog")]
    public class MealLog
    {
        [Key]
        public int MealLogId { get; set; } 

        public int UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual User User { get; set; } 
        public DateTime Date { get; set; } 

        [Required]
        [MaxLength(50)]
        public string MealType { get; set; } 

        // --- Propriedades de Navegação ---
        public virtual ICollection<MealLogItem> MealLogItems { get; set; } = new List<MealLogItem>();
    }
}