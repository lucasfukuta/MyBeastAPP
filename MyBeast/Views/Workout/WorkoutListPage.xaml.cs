using MyBeast.ViewModels.Workout;

namespace MyBeast.Views.Workout;

public partial class WorkoutListPage : ContentPage
{
    private readonly WorkoutListViewModel _viewModel;

    public WorkoutListPage(WorkoutListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Carrega o histórico sempre que a tela aparece
        await _viewModel.LoadHistoryCommand.ExecuteAsync(null);
    }
}