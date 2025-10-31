using System;

namespace MyBeast.API.DTOs.WorkoutSessions.Input // Verifique/Ajuste o namespace
{
    public class StartSessionDto
    {
        public int UserId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow; // Padrão para agora
    }
}