using System;
using System.Collections.Generic;

namespace MyBeast.ViewModels.Workout
{
    internal class WorkoutListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkoutItemViewModel> Workouts { get; set; }

        public WorkoutListViewModel()
        {
            Workouts = new List<WorkoutItemViewModel>();
        }
    }

    internal class WorkoutItemViewModel
    {
        public int Id { get; set; }
        public string ExerciseName { get; set; }
        public int Repetitions { get; set; }
        public int Sets { get; set; }
    }
}
