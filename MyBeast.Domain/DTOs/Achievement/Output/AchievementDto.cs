using System;

namespace MyBeast.Domain.DTOs.Achievement.Output // Verifique o namespace
{
    // DTO para retornar dados de uma Achievement
    public class AchievementDto
    {
        public int AchievementId { get; set; }
        public int UserId { get; set; } // Incluir UserId para referência
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // Usar string vazia se Description for nula no modelo
        public DateTime DateAchieved { get; set; }

        // Não incluir a propriedade de navegação 'User'
    }
}