namespace MyBeast.API.Dtos.WorkoutSession // Verifique/Ajuste o namespace
{
    public class SetLogDto // DTO para receber os dados de um set
    {
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public decimal Weight { get; set; }
        public int Reps { get; set; }
        public int? RestTimeSeconds { get; set; }
    }
}