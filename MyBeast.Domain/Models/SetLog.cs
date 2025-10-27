using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBeast.Domain.Models
{
    [Table("SetLog")]
    public class SetLog
    {
        [Key]
        public int SetLogId { get; set; } 

        public int SessionId { get; set; } 
        [ForeignKey("SessionId")]
        public virtual WorkoutSession WorkoutSession { get; set; } 

        public int ExerciseId { get; set; } 
        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; } 

        public int SetNumber { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Weight { get; set; } 
        public int Reps { get; set; } 
        public int? RestTimeSeconds { get; set; } 
    }
}