using MyBeast.ViewModels.Workout;

namespace MyBeast.Views.Workout;

public partial class WorkoutDetailPage : ContentPage
{
    public WorkoutDetailPage(WorkoutDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}