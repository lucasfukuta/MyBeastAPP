namespace MyBeast.Models.DTOs 
{
    public class ExerciseDto
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty;
        public string? Instructions { get; set; }
        public bool IsCustom { get; set; }
        public int? UserId { get; set; }
    }
}