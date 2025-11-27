using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MyBeast.Views.Workout;
using MyBeast.ViewModels.Workout;
using CommunityToolkit.Mvvm.Messaging;
using MyBeast.Messages;
using MyBeast.Views.Stats;

namespace MyBeast.ViewModels.Workout
{
    public class WorkoutListViewModel : BindableObject
    {
        private ObservableCollection<WorkoutItemViewModel> _workouts;
        public ObservableCollection<WorkoutItemViewModel> Workouts
        {
            get => _workouts;
            set
            {
                _workouts = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateWorkoutCommand { get; set; }
        public ICommand SelectWorkoutCommand { get; set; }
        public ICommand EditWorkoutCommand { get; set; }
        public ICommand DeleteWorkoutCommand { get; set; }
        public ICommand GoToStatsCommand { get; set; }
        public WorkoutListViewModel()
        {
            CreateCommands();
            LoadDataFromImage();

            // --- Escuta a mensagem de "Adicionar Treino" ---
            // Quando a tela de detalhes salvar, ela manda essa mensagem e nós adicionamos na lista aqui.
            WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, (recipient, message) =>
            {
                // O MainThread garante que a atualização visual não quebre o app
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // 'message.Value' contém o WorkoutItemViewModel que enviamos
                    Workouts.Add(message.Value);
                });
            });
        }

        private void CreateCommands()
        {
            // 1. Criar NOVO (Vai para a tela de detalhes vazia)
            CreateWorkoutCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(WorkoutDetailPage));
            });

            // 2. CLIQUE NO CARD -> IR PARA O CRONÔMETRO (ActiveWorkoutPage)
            SelectWorkoutCommand = new Command<WorkoutItemViewModel>(async (workout) =>
            {
                if (workout == null) return;

                var navigationParameter = new Dictionary<string, object>
        {
            { "CurrentWorkout", workout } // "CurrentWorkout" é a chave que o ActiveViewModel espera
        };

                // Vai para a tela de treino ativo
                await Shell.Current.GoToAsync(nameof(ActiveWorkoutPage), navigationParameter);
            });

            // 3. BOTÃO EDITAR -> IR PARA DETALHES/EDIÇÃO (WorkoutDetailPage)
            EditWorkoutCommand = new Command<WorkoutItemViewModel>(async (workout) =>
            {
                if (workout == null) return;

                var navigationParameter = new Dictionary<string, object>
        {
            { "WorkoutToEdit", workout } // "WorkoutToEdit" é a chave que o DetailViewModel espera
        };

                // Vai para a tela de edição
                await Shell.Current.GoToAsync(nameof(WorkoutDetailPage), navigationParameter);
            });

            // 4. BOTÃO EXCLUIR
            DeleteWorkoutCommand = new Command<WorkoutItemViewModel>(async (workout) =>
            {
                if (workout == null) return;

                bool confirm = await Shell.Current.DisplayAlert(
                    "Excluir Treino",
                    $"Tem certeza que deseja excluir '{workout.Name}'?",
                    "Sim, Excluir",
                    "Cancelar");

                if (confirm && Workouts.Contains(workout))
                {
                    Workouts.Remove(workout);
                }
            });
        }

        private void LoadDataFromImage()
        {
            Workouts = new ObservableCollection<WorkoutItemViewModel>
            {
                new WorkoutItemViewModel
                {
                    Name = "Full Body Strength",
                    Level = "INTERMEDIATE",
                    Duration = 45,
                    Calories = 380,
                    ExerciseCount = 8
                },
                new WorkoutItemViewModel
                {
                    Name = "HIIT Cardio Blast",
                    Level = "ADVANCED",
                    Duration = 30,
                    Calories = 420,
                    ExerciseCount = 6
                },
                new WorkoutItemViewModel
                {
                    Name = "Upper Body Focus",
                    Level = "BEGINNER",
                    Duration = 40,
                    Calories = 320,
                    ExerciseCount = 10
                }
            };
        }
    }

    // Classe auxiliar para os cards da lista
    public class WorkoutItemViewModel
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public int Duration { get; set; }
        public int Calories { get; set; }
        public int ExerciseCount { get; set; }
        public ObservableCollection<MyBeast.Models.WorkoutExercise> ExercisesList { get; set; } = new();
        public Color LevelColor
        {
            get
            {
                return Level?.ToUpper() switch
                {
                    "BEGINNER" => Color.FromArgb("#4CAF50"),     // Verde
                    "INTERMEDIATE" => Color.FromArgb("#FF9800"), // Laranja
                    "ADVANCED" => Color.FromArgb("#F44336"),     // Vermelho
                    _ => Colors.Gray
                };
            }
        }
    }
}