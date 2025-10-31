using System;
using MyBeast.API.DTOs.User.Output;

namespace MyBeast.API.DTOs.Community.Output
{
    // DTO para retornar dados de um CommunityPost
    public class PostDto
    {
        public int PostId { get; set; }
        // public int UserId { get; set; } // Opcional, já temos AuthorInfo
        public UserSummaryDto? AuthorInfo { get; set; } // Informações resumidas do autor
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // 'Photo', 'Video', 'Discussion', 'Poll'
        public DateTime CreatedAt { get; set; }

        // Dados agregados
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        // Poderíamos adicionar o status da reação do usuário atual (logado)? Ex: "Upvoted", "Downvoted", null

        // Não incluir a coleção PostReactions completa
    }
}