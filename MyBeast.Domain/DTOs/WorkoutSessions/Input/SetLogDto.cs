namespace MyBeast.Domain.DTOs.WorkoutSessions.Input // Verifique/Ajuste o namespace
{
    //Representa um único set realizado, com ExerciseId, Weight, Reps, etc
    public class SetLogDto // DTO para receber os dados de um set
    {
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public decimal Weight { get; set; }
        public int Reps { get; set; }
        public int? RestTimeSeconds { get; set; }
    }
}