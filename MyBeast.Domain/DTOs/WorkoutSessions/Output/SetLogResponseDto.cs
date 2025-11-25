namespace MyBeast.Domain.DTOs.WorkoutSessions.Output
{
    // DTO para retornar dados de um SetLog (incluindo nome do exercício) sem precisar chamar a API
    public class SetLogResponseDto
    {
        public int SetLogId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty; // Nome do exercício
        public int SetNumber { get; set; }
        public decimal Weight { get; set; }
        public int Reps { get; set; }
        public int? RestTimeSeconds { get; set; }
    }
}