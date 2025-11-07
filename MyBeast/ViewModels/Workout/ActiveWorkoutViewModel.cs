using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Workout
{
    internal class ActiveWorkoutViewModel
    {
        // Properties
        public string WorkoutName { get; set; }
        public TimeSpan Duration { get; set; }
        public int CaloriesBurned { get; set; }
        public List<string> Exercises { get; set; }

        // Constructor
        public ActiveWorkoutViewModel()
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

        public void ClearExercises()
        {
            Exercises.Clear();
        }
    }
}
