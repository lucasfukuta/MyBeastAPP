using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.DTOs.Community.Input
{
    public class CreatePostDto
    {
        // UserId FOI REMOVIDO. O servidor pegará do token.

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Photo|Video|Discussion|Poll)$", ErrorMessage = "Tipo inválido. Use 'Photo', 'Video', 'Discussion' ou 'Poll'.")]
        public string Type { get; set; } = string.Empty;
    }
}