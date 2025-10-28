using System; // Adicionar se ainda não existir

namespace MyBeast.API.Dtos.WorkoutSession // Verifique/Ajuste o namespace
{
    public class StartSessionDto
    {
        public int UserId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow; // Padrão para agora
    }
}