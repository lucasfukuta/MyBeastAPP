using System.ComponentModel.DataAnnotations;

namespace MyBeast.API.Dtos.User
{
    // DTO para receber dados na atualização do perfil
    public class UserUpdateDto
    {
        [MaxLength(50)]
        public string? Username { get; set; } // Opcional

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; } // Opcional

        // Não incluir Password ou PlanType aqui (fluxos separados)
    }
}