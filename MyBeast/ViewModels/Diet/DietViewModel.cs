using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyBeast.Messages;
using MyBeast.Models;
using MyBeast.Services;
using System.Collections.ObjectModel;

namespace MyBeast.ViewModels.Diet
{
    public partial class DietViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Meal> meals;

        [ObservableProperty]
        private int currentCalories;

        [ObservableProperty]
        private int targetCalories = 2200;

        [ObservableProperty]
        private int proteinConsumed;
        [ObservableProperty]
        private int proteinTarget = 150;

        [ObservableProperty]
        private int carbsConsumed;
        [ObservableProperty]
        private int carbsTarget = 250;

        [ObservableProperty]
        private int fatConsumed;
        [ObservableProperty]
        private int fatTarget = 70;

        [ObservableProperty]
        private int caloriesBurned;

        public int NetCalories => TargetCalories - CurrentCalories + CaloriesBurned;
        public string FormattedProteinConsumed => $"{ProteinConsumed}/{ProteinTarget}g";
        public string FormattedCarbsConsumed => $"{CarbsConsumed}/{CarbsTarget}g";
        public string FormattedFatConsumed => $"{FatConsumed}/{FatTarget}g";

        public DietViewModel()
        {
            Meals = new ObservableCollection<Meal>();
            LoadSampleMeals();
            UpdateMacros();

            // Escuta quando um treino é finalizado
            WeakReferenceMessenger.Default.Register<WorkoutFinishedMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Soma as calorias do treino que acabou de finalizar
                    CaloriesBurned += m.Value.Calories;

                    // Se quiser salvar isso num banco de dados, faria aqui.
                });
            });
            // Escuta quando uma refeição é salva na tela de edição
            WeakReferenceMessenger.Default.Register<MealSavedMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var savedMeal = m.Value;

                    // Verifica se estamos editando (busca por nome e hora iguais)
                    var existing = Meals.FirstOrDefault(x => x.Name == savedMeal.Name && x.Time == savedMeal.Time);

                    if (existing != null)
                    {
                        int index = Meals.IndexOf(existing);
                        Meals[index] = savedMeal; // Atualiza a existente
                    }
                    else
                    {
                        Meals.Add(savedMeal); // Adiciona nova
                    }
                    UpdateMacros();
                });
            });
        }

        // --- COMANDO EDITAR ---
        [RelayCommand]
        private async Task EditMeal(Meal meal)
        {
            if (meal == null) return;

            var navParam = new Dictionary<string, object>
            {
                { "MealToEdit", meal }
            };

            // Vai para a tela de edição passando a refeição
            await Shell.Current.GoToAsync(nameof(Views.Diet.MealEditorPage), navParam);
        }

        // --- COMANDO EXCLUIR ---
        [RelayCommand]
        private async Task DeleteMeal(Meal meal)
        {
            if (meal == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Excluir", $"Remover {meal.Name}?", "Sim", "Não");
            if (confirm)
            {
                Meals.Remove(meal);
                UpdateMacros();
            }
        }

        // --- COMANDO ADICIONAR (Navega para tela vazia) ---
        [RelayCommand]
        private async Task AddMeal()
        {
            await Shell.Current.GoToAsync(nameof(Views.Diet.MealEditorPage));
        }

        // --- COMANDO CONSUMIR (Gamification) ---
        [RelayCommand]
        private async Task ConsumeMeal(Meal meal)
        {
            if (meal == null) return;

            // 1. Alterna o estado (Marcado/Desmarcado)
            meal.IsConsumed = !meal.IsConsumed;

            // 2. Atualiza os totais na tela de dieta
            UpdateMacros();

            // Calcula o valor para mandar pro gráfico.
            // Se marcou (true) -> Manda positivo (soma no gráfico).
            // Se desmarcou (false) -> Manda negativo (subtrai do gráfico).
            int caloriesToSend = meal.IsConsumed ? meal.Kcal : -meal.Kcal;

            // Envia para a StatsPage atualizar a linha laranja
            WeakReferenceMessenger.Default.Send(new DietConsumedMessage(caloriesToSend));

            // 3. Gamification (Pet) - Só ganha ponto se marcou como feito
            if (meal.IsConsumed)
            {
                WeakReferenceMessenger.Default.Send(new PetUpdateMessage(PetActionType.MealFinished));
            }
        }

        private void UpdateMacros()
        {
            if (Meals == null) return;

            var consumedMeals = Meals.Where(m => m.IsConsumed).ToList();

            CurrentCalories = consumedMeals.Sum(m => m.Kcal);

            // Usa navegação segura (?. e ??) para evitar erros se FoodItems for nulo
            ProteinConsumed = Meals.Sum(m => m.FoodItems?.Sum(fi => fi.Protein) ?? 0);
            CarbsConsumed = Meals.Sum(m => m.FoodItems?.Sum(fi => fi.Carbs) ?? 0);
            FatConsumed = Meals.Sum(m => m.FoodItems?.Sum(fi => fi.Fat) ?? 0);

            OnPropertyChanged(nameof(FormattedProteinConsumed));
            OnPropertyChanged(nameof(FormattedCarbsConsumed));
            OnPropertyChanged(nameof(FormattedFatConsumed));
        }

        private void LoadSampleMeals()
        {
            Meals.Clear();

            // Café da Manhã
            Meals.Add(new Meal
            {
                Name = "Café da Manhã",
                Time = new TimeSpan(8, 0, 0),
                Icon = "apple_icon.png",
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Ovos Mexidos", Kcal = 155, Protein = 13, Carbs = 1, Fat = 11 },
                    new FoodItem { Name = "Pão Integral", Kcal = 80, Protein = 4, Carbs = 14, Fat = 1 },
                    new FoodItem { Name = "Café", Kcal = 0, Protein = 0, Carbs = 0, Fat = 0 }
                },
                Kcal = 235,
                ItemsCount = 3
            });

            // Almoço
            Meals.Add(new Meal
            {
                Name = "Almoço",
                Time = new TimeSpan(13, 0, 0),
                Icon = "apple_icon.png",
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Frango Grelhado", Kcal = 165, Protein = 31, Carbs = 0, Fat = 3 },
                    new FoodItem { Name = "Arroz Branco", Kcal = 130, Protein = 3, Carbs = 28, Fat = 0 },
                    new FoodItem { Name = "Brócolis Cozido", Kcal = 55, Protein = 4, Carbs = 11, Fat = 1 }
                },
                Kcal = 350,
                ItemsCount = 3
            });

            // Lanche
            Meals.Add(new Meal
            {
                Name = "Lanche",
                Time = new TimeSpan(16, 0, 0),
                Icon = "apple_icon.png",
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Iogurte Natural", Kcal = 100, Protein = 10, Carbs = 10, Fat = 3 },
                    new FoodItem { Name = "Banana", Kcal = 90, Protein = 1, Carbs = 23, Fat = 0 }
                },
                Kcal = 190,
                ItemsCount = 2
            });

            // Jantar
            Meals.Add(new Meal
            {
                Name = "Jantar",
                Time = new TimeSpan(19, 0, 0),
                Icon = "apple_icon.png",
                FoodItems = new List<FoodItem>(),
                Kcal = 0,
                ItemsCount = 0
            });

            UpdateMacros();
        }
    }
}