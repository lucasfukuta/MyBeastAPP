using System.ComponentModel.DataAnnotations;

namespace MyBeast.Domain.DTOs.User.Input
{
    // DTO para receber dados no registro
    //Cria um novo usuário
    public class UserRegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)] // Adicionar validação de força de senha
        public string Password { get; set; } = string.Empty; // Recebe a senha em texto plano

        public string? PlanType { get; set; } // Opcional, padrão 'Free'
    }
}