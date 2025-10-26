using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Models
{
    [Table("Achievement")]
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; } 

        public int UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual User User { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 
        [MaxLength(255)]
        public string Description { get; set; } 
        public DateTime DateAchieved { get; set; } 
    }
}