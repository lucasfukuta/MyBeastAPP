using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Domain.Entities;
using MyBeast.Services;
using MyBeast.Views.Workout;
using System.Collections.ObjectModel;

namespace MyBeast.ViewModels.Workout
{
    public partial class WorkoutListViewModel : ObservableObject
    {
        private readonly IWorkoutSessionService _workoutService;

        // Coleção que a View vai observar para montar a lista
        public ObservableCollection<WorkoutSession> History { get; } = new();

        [ObservableProperty]
        private bool isBusy;

        public WorkoutListViewModel(IWorkoutSessionService workoutService)
        {
            _workoutService = workoutService;
        }

        [RelayCommand]
        public async Task LoadHistoryAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var sessions = await _workoutService.GetSessionHistoryAsync();

                History.Clear();
                foreach (var session in sessions)
                {
                    History.Add(session);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Falha ao carregar histórico: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task StartNewWorkoutAsync()
        {
            // Navega para a tela de treino ativo
            await Shell.Current.GoToAsync(nameof(ActiveWorkoutPage));
        }

        [RelayCommand]
        public async Task GoToDetailsAsync(WorkoutSession session)
        {
            if (session == null) return;

            // Navega para a tela de detalhes passando o objeto da sessão
            var navigationParameter = new Dictionary<string, object>
            {
                { "Session", session }
            };
            await Shell.Current.GoToAsync(nameof(WorkoutDetailPage), navigationParameter);
        }
    }
}