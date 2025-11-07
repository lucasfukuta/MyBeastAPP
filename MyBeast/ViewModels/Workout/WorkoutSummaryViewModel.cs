using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Workout
{
    internal class WorkoutSummaryViewModel
    {
        public string WorkoutName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public int CaloriesBurned { get; set; }
        public List<string> Exercises { get; set; }

        public WorkoutSummaryViewModel()
        {
            Exercises = new List<string>();
        }
    }
}
