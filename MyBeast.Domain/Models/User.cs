using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Domain.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } 

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } 

        [Required]
        [MaxLength(10)]
        public string PlanType { get; set; } 

        public bool IsModerator { get; set; } 
        public DateTime CreatedAt { get; set; } 

        // --- Propriedades de Navegação (Relações) ---
        public virtual Pet Pet { get; set; } // Relação 1:1 com Pet 
        public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
        public virtual ICollection<WorkoutTemplate> WorkoutTemplates { get; set; } = new List<WorkoutTemplate>();
        public virtual ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
        public virtual ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();
        public virtual ICollection<CommunityPost> CommunityPosts { get; set; } = new List<CommunityPost>();
        public virtual ICollection<AISuggestion> AISuggestions { get; set; } = new List<AISuggestion>();
        public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
    }
}
