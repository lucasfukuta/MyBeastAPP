using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Workout
{
    internal class WorkoutDetailViewModel
    {
        // Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> Exercises { get; set; }

        // Constructor
        public WorkoutDetailViewModel()
        {
            Exercises = new List<string>();
        }

        // Methods
        public void AddExercise(string exercise)
        {
            if (!string.IsNullOrWhiteSpace(exercise))
            {
                Exercises.Add(exercise);
            }
        }

        public void RemoveExercise(string exercise)
        {
            if (Exercises.Contains(exercise))
            {
                Exercises.Remove(exercise);
            }
        }
    }
}
