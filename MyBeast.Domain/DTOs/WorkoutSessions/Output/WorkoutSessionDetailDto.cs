using System;
using System.Collections.Generic;

namespace MyBeast.Domain.DTOs.WorkoutSessions.Output
{
    // DTO para retornar dados detalhados de uma WorkoutSession (incluindo sets)
    public class WorkoutSessionDetailDto
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int? DurationMinutes { get; set; }
        public decimal TotalVolume { get; set; }
        public List<SetLogResponseDto> SetLogs { get; set; } = new List<SetLogResponseDto>(); // Lista de sets detalhados
    }
}