using System;

namespace MyBeast.API.DTOs.WorkoutSessions.Output
{
    // DTO para retornar dados resumidos de uma WorkoutSession (para listas)
    public class WorkoutSessionDto
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int? DurationMinutes { get; set; }
        public decimal TotalVolume { get; set; }
        public int SetCount { get; set; } // Adicionado: Contagem de sets
    }
}