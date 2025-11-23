using MyBeast.ViewModels.Workout;

namespace MyBeast.Views.Workout;

public partial class WorkoutSummaryPage : ContentPage
{
    public WorkoutSummaryPage(WorkoutSummaryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}