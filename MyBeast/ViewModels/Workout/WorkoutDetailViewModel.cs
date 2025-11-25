using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using MyBeast.Messages;
using MyBeast.Models;
using MyBeast.Models.DTOs;

namespace MyBeast.ViewModels.Workout
{
    // A classe deve implementar IQueryAttributable para receber dados da navegação
    public class WorkoutDetailViewModel : BindableObject, IQueryAttributable
    {
        private string _name;
        private string _description;
        private bool _isExistingWorkout;

        public string TitlePage => IsExistingWorkout ? "Editar Treino" : "Novo Treino";

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public int DurationMinutes { get; set; }
        public bool IsExistingWorkout
        {
            get => _isExistingWorkout;
            set
            {
                _isExistingWorkout = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TitlePage));
            }
        }

        public ObservableCollection<WorkoutExercise> Exercises { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }
        public ICommand StartWorkoutCommand { get; }

        public WorkoutDetailViewModel()
        {
            Exercises = new ObservableCollection<WorkoutExercise>();

            SaveCommand = new Command(async () => await SaveWorkout());

            StartWorkoutCommand = new Command(async () =>
            {
                // Verifica se a página ActiveWorkoutPage existe e foi registrada no AppShell
                await Shell.Current.GoToAsync(nameof(Views.Workout.ActiveWorkoutPage));
            });

            AddExerciseCommand = new Command<string>((exerciseName) =>
            {
                if (!string.IsNullOrWhiteSpace(exerciseName))
                {
                    Exercises.Add(new WorkoutExercise
                    {
                        Name = exerciseName,
                        Description = "Adicionado manualmente",
                        MuscleGroup = "Geral",
                        Sets = 3,
                        Reps = 12,
                        ExerciseId = 0
                    });
                }
            });

            RemoveExerciseCommand = new Command<WorkoutExercise>((exercise) =>
            {
                if (Exercises.Contains(exercise)) Exercises.Remove(exercise);
            });
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Exercises.Clear();

            // Verifica se veio um objeto para edição
            if (query.ContainsKey("WorkoutToEdit"))
            {
                IsExistingWorkout = true;
                Name = "Treino Carregado";
                Exercises.Add(new WorkoutExercise { Name = "Exemplo Carregado", Sets = 4, Reps = 10 });
            }
            else
            {
                // Modo CRIAÇÃO
                IsExistingWorkout = false;
                Name = "";
                Description = "";
            }
        }

        private async Task SaveWorkout()
        {
            // Cria o objeto que vai aparecer na lista
            var novoTreinoParaLista = new WorkoutItemViewModel
            {
                Name = this.Name,
                Duration = this.DurationMinutes, // Propriedade nova que criamos
                ExerciseCount = this.Exercises.Count,
                Level = "CUSTOM", // Ou uma propriedade selecionada
                Calories = 300 // Valor estimado
            };

            // --- Envia mensagem avisando que tem treino novo ---
            WeakReferenceMessenger.Default.Send(new WorkoutSavedMessage(novoTreinoParaLista));

            Console.WriteLine($"Salvando: {Name} - Mensagem enviada!");

            await Shell.Current.GoToAsync("..");
        }
    }
}