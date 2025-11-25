using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using MyBeast.Messages;
using MyBeast.Models;

namespace MyBeast.ViewModels.Workout
{
    public class ActiveWorkoutViewModel : BindableObject, IQueryAttributable
    {
        private IDispatcherTimer _timer;
        private TimeSpan _elapsedTime;
        private bool _isRunning;
        private string _workoutTime;

        // Propriedades Visuais
        public string WorkoutName { get; set; } = "Treino em Andamento";

        public string WorkoutTime
        {
            get => _workoutTime;
            set { _workoutTime = value; OnPropertyChanged(); }
        }

        // --- Lógica de Habilitação dos Botões ---
        // O botão INICIAR só funciona se NÃO estiver rodando
        public bool CanStart => !_isRunning;

        // O botão PAUSAR só funciona se ESTIVER rodando
        public bool CanPause => _isRunning;

        // O botão FINALIZAR funciona se tiver começado (tempo > 0)
        public bool CanFinish => _elapsedTime.TotalSeconds > 0;

        // Comandos
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand FinishCommand { get; }
        public int CaloriesBurned { get; set; } = 300;
        public ActiveWorkoutViewModel()
        {
            _elapsedTime = TimeSpan.Zero;
            WorkoutTime = "00:00:00";
            _isRunning = false;

            // Timer do MAUI
            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
                WorkoutTime = _elapsedTime.ToString(@"hh\:mm\:ss");

                // Força a atualização do botão finalizar caso seja o primeiro segundo
                if (_elapsedTime.TotalSeconds == 1) OnPropertyChanged(nameof(CanFinish));
            };

            // 1. INICIAR
            StartCommand = new Command(() =>
            {
                if (_isRunning) return; // Evita duplo clique

                _isRunning = true;
                _timer.Start();
                UpdateButtons(); // Atualiza a tela
            });

            // 2. PAUSAR
            PauseCommand = new Command(() =>
            {
                if (!_isRunning) return;

                _isRunning = false;
                _timer.Stop();
                UpdateButtons(); // Atualiza a tela
            });

            // 3. FINALIZAR
            FinishCommand = new Command(async () =>
            {
                // 1. Para o timer (congela o tempo atual em _elapsedTime)
                _timer.Stop();

                // 2. Pergunta APENAS as calorias (O tempo nós já temos)
                string result = await Shell.Current.DisplayPromptAsync(
                    title: "Treino Finalizado! 💪",
                    message: $"Tempo total: {WorkoutTime}\nQuantas calorias você queimou?",
                    placeholder: "Ex: 350",
                    accept: "Concluir",
                    cancel: "Cancelar",
                    keyboard: Keyboard.Numeric);

                // Se cancelar, retoma o timer
                if (string.IsNullOrWhiteSpace(result))
                {
                    _timer.Start();
                    return;
                }

                if (int.TryParse(result, out int userCalories))
                {
                    _isRunning = false;

                    // --- AQUI ESTÁ A AUTOMATIZAÇÃO ---
                    var workoutData = new WorkoutResult
                    {
                        Calories = userCalories,
                        Duration = _elapsedTime // <--- PEGA O TEMPO AUTOMATICAMENTE DAQUI
                    };

                    // Envia o pacote completo (Calorias + Tempo Automático)
                    WeakReferenceMessenger.Default.Send(new WorkoutFinishedMessage(workoutData));

                    // Avisa o Pet também
                    WeakReferenceMessenger.Default.Send(new PetUpdateMessage(PetActionType.WorkoutFinished));

                    await Shell.Current.DisplayAlert("Salvo!", "Seus dados foram atualizados.", "OK");
                    await Shell.Current.GoToAsync("///WorkoutListPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Digite um número válido para as calorias.", "OK");
                    _timer.Start();
                }
            });
        }

        // Método auxiliar para avisar a tela que os botões mudaram de estado
        private void UpdateButtons()
        {
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanPause));
            OnPropertyChanged(nameof(CanFinish));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // Verifica se foi enviado um treino com a chave "CurrentWorkout"
            if (query.ContainsKey("CurrentWorkout"))
            {
                var workout = query["CurrentWorkout"] as WorkoutItemViewModel;

                if (workout != null)
                {
                    // Atualiza o nome do treino na tela
                    WorkoutName = workout.Name;

                    // Reseta o timer para começar do zero
                    _elapsedTime = TimeSpan.Zero;
                    WorkoutTime = "00:00:00";
                    _isRunning = false;
                    UpdateButtons();
                }
            }
        }
    }
}