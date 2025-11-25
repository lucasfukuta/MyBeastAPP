namespace MyBeast.Domain.DTOs.User.Output
{
    // DTO resumido para informações do autor de um post
    public class UserSummaryDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        // Talvez adicionar URL da foto de perfil no futuro?
        // public string? ProfilePictureUrl { get; set; }
    }
}