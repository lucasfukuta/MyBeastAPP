using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Domain.Entities;
using MyBeast.Services;
using MyBeast.Views.Workout;
using System.Collections.ObjectModel;
using System.Timers; // Necessário para o Timer

namespace MyBeast.ViewModels.Workout
{
    public partial class ActiveWorkoutViewModel : ObservableObject
    {
        private readonly IWorkoutSessionService _sessionService;
        private readonly IExerciseService _exerciseService;
        private System.Timers.Timer _timer;
        private DateTime _startTime;

        [ObservableProperty]
        private WorkoutSession currentSession;

        [ObservableProperty]
        private string timerText = "00:00";

        // Lista de exercícios disponíveis para selecionar (populada ao carregar)
        public ObservableCollection<Exercise> AvailableExercises { get; } = new();

        // Exercício selecionado no Picker (ou modal)
        [ObservableProperty]
        private Exercise selectedExercise;

        // Logs que estão sendo criados agora
        public ObservableCollection<SetLog> CurrentSets { get; } = new();

        public ActiveWorkoutViewModel(IWorkoutSessionService sessionService, IExerciseService exerciseService)
        {
            _sessionService = sessionService;
            _exerciseService = exerciseService;

            // Configura um timer simples para atualizar a UI a cada segundo
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var duration = DateTime.Now - _startTime;
            // Atualiza a propriedade na UI Thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerText = duration.ToString(@"mm\:ss");
            });
        }

        [RelayCommand]
        public async Task InitializeSessionAsync()
        {
            // Carrega exercícios para o usuário escolher
            var exercises = await _exerciseService.GetAllExercisesAsync();
            AvailableExercises.Clear();
            foreach (var ex in exercises) AvailableExercises.Add(ex);

            // Inicia sessão no mock/banco
            CurrentSession = await _sessionService.StartSessionAsync(DateTime.Now);
            _startTime = DateTime.Now;
            _timer.Start();
        }

        [RelayCommand]
        public void AddSet()
        {
            if (SelectedExercise == null)
            {
                Shell.Current.DisplayAlert("Atenção", "Selecione um exercício primeiro.", "OK");
                return;
            }

            var newSet = new SetLog
            {
                ExerciseId = SelectedExercise.ExerciseId,
                Exercise = SelectedExercise, // Para exibir o nome na tela
                SetNumber = CurrentSets.Count(s => s.ExerciseId == SelectedExercise.ExerciseId) + 1,
                Weight = 0, // Usuário preencherá na tela
                Reps = 0    // Usuário preencherá na tela
            };

            CurrentSets.Add(newSet);
        }

        [RelayCommand]
        public async Task FinishWorkoutAsync()
        {
            _timer.Stop();

            if (CurrentSession != null)
            {
                CurrentSession.SetLogs = CurrentSets.ToList();
                CurrentSession.DurationMinutes = (int)(DateTime.Now - _startTime).TotalMinutes;

                var finishedSession = await _sessionService.FinishSessionAsync(CurrentSession);

                // Navega para o resumo passando a sessão finalizada
                var navParam = new Dictionary<string, object>
                {
                    { "Session", finishedSession }
                };

                // Remove a página atual da pilha para o usuário não voltar para o treino "ativo"
                await Shell.Current.GoToAsync($"../{nameof(WorkoutSummaryPage)}", navParam);
            }
        }
    }
}