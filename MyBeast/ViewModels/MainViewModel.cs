using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging; // Necessário para receber a mensagem
using MyBeast.Messages;
using MyBeast.ViewModels.Workout;

namespace MyBeast.ViewModels
{
    public class MainViewModel : BindableObject
    {
        // --- PROPRIEDADES DO PET ---
        private double _health;
        private double _energy;
        private double _hunger; // Vamos tratar como "Saciedade" (1.0 = Cheio, 0.0 = Faminto)

        public double Health
        {
            get => _health;
            set { _health = value; OnPropertyChanged(); OnPropertyChanged(nameof(HealthText)); }
        }

        public double Energy
        {
            get => _energy;
            set { _energy = value; OnPropertyChanged(); OnPropertyChanged(nameof(EnergyText)); }
        }

        public double Hunger
        {
            get => _hunger;
            set { _hunger = value; OnPropertyChanged(); OnPropertyChanged(nameof(HungerText)); }
        }

        // Textos para a tela (ex: "85%")
        public string HealthText => $"{Math.Round(Health * 100)}%";
        public string EnergyText => $"{Math.Round(Energy * 100)}%";
        public string HungerText => $"{Math.Round(Hunger * 100)}%";

        // --- RESTANTE DO CÓDIGO (Lista, Seleção, etc) ---
        private WorkoutItemViewModel _selectedWorkout;
        public ObservableCollection<WorkoutItemViewModel> RecommendedWorkouts { get; set; }

        public WorkoutItemViewModel SelectedWorkout
        {
            get => _selectedWorkout;
            set
            {
                if (_selectedWorkout != value)
                {
                    _selectedWorkout = value;
                    OnPropertyChanged();
                    (StartWorkoutCommand as Command).ChangeCanExecute();
                }
            }
        }

        public ICommand StartWorkoutCommand { get; }
        public ICommand ProfileTappedCommand { get; }

        // Simulação de comando de comer (para testar a dieta)
        public ICommand EatCommand { get; }

        public MainViewModel()
        {
            LoadMockData();
            InitializePetStats();

            // 1. ESCUTAR MENSAGENS DE ATIVIDADE
            WeakReferenceMessenger.Default.Register<PetUpdateMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdatePetStats(m.Value);
                });
            });

            // Configuração dos comandos
            StartWorkoutCommand = new Command(async () =>
            {
                if (SelectedWorkout == null)
                {
                    await Shell.Current.DisplayAlert("Atenção", "Selecione um treino primeiro!", "OK");
                    return;
                }
                var navParams = new Dictionary<string, object> { { "CurrentWorkout", SelectedWorkout } };
                await Shell.Current.GoToAsync(nameof(Views.Workout.ActiveWorkoutPage), navParams);
            }, () => SelectedWorkout != null);

            ProfileTappedCommand = new Command(async () => await Shell.Current.DisplayAlert("Perfil", "Acessar perfil...", "OK"));

            // Comando temporário para você testar a dieta na tela inicial se quiser
            EatCommand = new Command(() =>
            {
                // Simula terminar uma dieta
                WeakReferenceMessenger.Default.Send(new PetUpdateMessage(PetActionType.MealFinished));
            });
        }

        private void InitializePetStats()
        {
            // Valores Iniciais
            Health = 0.5; // 50%
            Energy = 0.8; // 80%
            Hunger = 0.6; // 60%
        }

        private void UpdatePetStats(PetActionType action)
        {
            switch (action)
            {
                case PetActionType.WorkoutFinished:
                    // Treino: Ganha Saúde, Perde Energia, Perde Saciedade (Fome aumenta)
                    Health = Math.Clamp(Health + 0.15, 0, 1); // +15%
                    Energy = Math.Clamp(Energy - 0.20, 0, 1); // -20%
                    Hunger = Math.Clamp(Hunger - 0.10, 0, 1); // -10% (ficou com fome)
                    break;

                case PetActionType.MealFinished:
                    // Dieta: Recupera Energia, Enche a Barriga (Saciedade), Pequeno ganho de saúde
                    Energy = Math.Clamp(Energy + 0.10, 0, 1); // +10%
                    Hunger = Math.Clamp(Hunger + 0.30, 0, 1); // +30% (encheu o bucho)
                    Health = Math.Clamp(Health + 0.05, 0, 1); // +5%
                    break;
            }
        }

        private void LoadMockData()
        {
            RecommendedWorkouts = new ObservableCollection<WorkoutItemViewModel>
            {
                new WorkoutItemViewModel { Name = "Full Body Strength", Level = "INTERMEDIATE", Duration = 45, Calories = 380, ExerciseCount = 8 },
                new WorkoutItemViewModel { Name = "HIIT Cardio Blast", Level = "ADVANCED", Duration = 30, Calories = 420, ExerciseCount = 6 },
                new WorkoutItemViewModel { Name = "Upper Body Focus", Level = "BEGINNER", Duration = 40, Calories = 320, ExerciseCount = 10 }
            };
        }
    }
}