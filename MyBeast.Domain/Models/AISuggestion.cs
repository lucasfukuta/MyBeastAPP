using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Domain.Models
{
    [Table("AISuggestion")]
    public class AISuggestion
    {
        [Key]
        public int SuggestionId { get; set; } 

        public int UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual User User { get; set; } 

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } 

        [Required]
        public string Content { get; set; } 
        public DateTime Date { get; set; } 
    }
}