using MyBeast.ViewModels.Workout;

namespace MyBeast.Views.Workout;

public partial class ActiveWorkoutPage : ContentPage
{
    public ActiveWorkoutPage()
    {
        InitializeComponent();
        BindingContext = new ActiveWorkoutViewModel();
    }
}