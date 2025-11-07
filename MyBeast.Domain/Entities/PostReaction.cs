using MyBeast.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Domain.Entities
{
    [Table("PostReaction")]
    public class PostReaction
    {
        // Chave Primária Composta 
        public int PostId { get; set; } 
        public int UserId { get; set; } 

        [Required]
        [MaxLength(10)]
        public string ReactionType { get; set; } 

        // --- Propriedades de Navegação ---
        [ForeignKey("PostId")]
        public virtual CommunityPost CommunityPost { get; set; } 
        [ForeignKey("UserId")]
        public virtual User User { get; set; } 
    }
}
