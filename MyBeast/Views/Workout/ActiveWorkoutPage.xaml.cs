using MyBeast.ViewModels.Workout;

namespace MyBeast.Views.Workout;

public partial class ActiveWorkoutPage : ContentPage
{
    public ActiveWorkoutPage(ActiveWorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // Opcional: Se precisar iniciar o timer ao abrir a página
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ActiveWorkoutViewModel vm)
        {
            await vm.InitializeSessionCommand.ExecuteAsync(null);
        }
    }
}