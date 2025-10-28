using System; // Adicionar
using System.Collections.Generic; // Adicionar

namespace MyBeast.API.Dtos.WorkoutSession // Verifique/Ajuste o namespace
{
    public class EndSessionDto
    {
        public DateTime EndTime { get; set; } = DateTime.UtcNow;
        public decimal TotalVolume { get; set; }
        public List<SetLogDto>? SetLogs { get; set; } // Lista de sets realizados
    }
}