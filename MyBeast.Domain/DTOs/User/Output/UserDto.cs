using System;

namespace MyBeast.Domain.DTOs.User.Output
{
    // DTO para retornar dados do usuário (sem senha)
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PlanType { get; set; } = string.Empty;
        public bool IsModerator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}