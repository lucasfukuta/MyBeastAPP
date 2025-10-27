using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Domain.Models
{
    [Table("CommunityPost")]
    public class CommunityPost
    {
        [Key]
        public int PostId { get; set; } 

        public int UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual User User { get; set; } 

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } 
        public string Content { get; set; } 

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } 
        public DateTime CreatedAt { get; set; } 

        // --- Propriedades de Navegação ---
        public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
    }
}