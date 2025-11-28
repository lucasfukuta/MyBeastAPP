using MyBeast.Views;
using MyBeast.Views.Auth;
using MyBeast.Views.Stats;

namespace MyBeast
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Views.Workout.WorkoutDetailPage), typeof(Views.Workout.WorkoutDetailPage));
            Routing.RegisterRoute(nameof(Views.Workout.ActiveWorkoutPage), typeof(Views.Workout.ActiveWorkoutPage));
            Routing.RegisterRoute(nameof(Views.Diet.MealEditorPage), typeof(Views.Diet.MealEditorPage));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
        }
    }
}
