using System;

namespace MyBeast.API.DTOs.WorkoutSessions.Input
{
    public class StartSessionDto
    {
        // UserId FOI REMOVIDO. O servidor pegará do token.
        // public int UserId { get; set; } 

        public DateTime StartTime { get; set; } = DateTime.UtcNow; // Padrão para agora
    }
}