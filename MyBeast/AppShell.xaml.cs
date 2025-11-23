using MyBeast.Views;

namespace MyBeast
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            //Routing.RegisterRoute("HomePage", typeof(MainPage));
            Routing.RegisterRoute(nameof(Views.Workout.ActiveWorkoutPage), typeof(Views.Workout.ActiveWorkoutPage));
            Routing.RegisterRoute(nameof(Views.Workout.WorkoutDetailPage), typeof(Views.Workout.WorkoutDetailPage));
            Routing.RegisterRoute(nameof(Views.Workout.WorkoutSummaryPage), typeof(Views.Workout.WorkoutSummaryPage));
        }
    }
}
 