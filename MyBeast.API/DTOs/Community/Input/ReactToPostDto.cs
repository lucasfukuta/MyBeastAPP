using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.Community.Input
{
    public class ReactToPostDto
    {
        // UserId FOI REMOVIDO. O servidor pegará do token.

        [Required]
        [RegularExpression("^(Upvote|Downvote|Report)$", ErrorMessage = "Tipo inválido. Use 'Upvote', 'Downvote' ou 'Report'.")]
        public string ReactionType { get; set; } = string.Empty;
    }
}