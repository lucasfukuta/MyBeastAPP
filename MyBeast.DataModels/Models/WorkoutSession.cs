using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Models
{
    [Table("WorkoutSession")]
    public class WorkoutSession
    {
        [Key]
        public int SessionId { get; set; } 

        public int UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual User User { get; set; } 
        public DateTime Date { get; set; } 
        public int? DurationMinutes { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalVolume { get; set; } 

        // --- Propriedades de Navegação ---
        public virtual ICollection<SetLog> SetLogs { get; set; }
    }
}