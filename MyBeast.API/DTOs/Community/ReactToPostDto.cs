using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.Dtos.Community
{
    public class ReactToPostDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [RegularExpression("^(Upvote|Downvote|Report)$", ErrorMessage = "Tipo inválido. Use 'Upvote', 'Downvote' ou 'Report'.")]
        public string ReactionType { get; set; } = string.Empty;
    }
}