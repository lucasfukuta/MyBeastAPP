using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyBeast.Messages;
using MyBeast.Models;
using MyBeast.Services;
using MyBeast.Domain.DTOs.MealLog.Input; // DTOs para API
using System.Collections.ObjectModel;

namespace MyBeast.ViewModels.Diet
{
    public partial class DietViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        [ObservableProperty] private ObservableCollection<Meal> meals;
        [ObservableProperty] private int currentCalories;
        [ObservableProperty] private int targetCalories = 2200;

        [ObservableProperty] private int proteinConsumed;
        [ObservableProperty] private int proteinTarget = 150;
        [ObservableProperty] private int carbsConsumed;
        [ObservableProperty] private int carbsTarget = 250;
        [ObservableProperty] private int fatConsumed;
        [ObservableProperty] private int fatTarget = 70;
        [ObservableProperty] private int caloriesBurned;

        public int NetCalories => TargetCalories - CurrentCalories + CaloriesBurned;
        public string FormattedProteinConsumed => $"{ProteinConsumed}/{ProteinTarget}g";
        public string FormattedCarbsConsumed => $"{CarbsConsumed}/{CarbsTarget}g";
        public string FormattedFatConsumed => $"{FatConsumed}/{FatTarget}g";

        public DietViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Meals = new ObservableCollection<Meal>();

            // Carrega dados REAIS do banco ao iniciar
            LoadMealsFromApi();

            // ESCUTA: Treino Finalizado
            WeakReferenceMessenger.Default.Register<WorkoutFinishedMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CaloriesBurned += m.Value.Calories;
                    OnPropertyChanged(nameof(NetCalories));
                });
            });

            // ESCUTA: Refeição Salva na Tela de Edição
            WeakReferenceMessenger.Default.Register<MealSavedMessage>(this, async (r, m) =>
            {
                var savedMeal = m.Value;

                // 1. Adiciona visualmente na hora
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Meals.Add(savedMeal);
                    UpdateMacros();
                });

                // 2. Salva no Banco (API)
                await SaveMealToApi(savedMeal);
            });
        }

        // --- CARREGAR DO BANCO ---
        private async void LoadMealsFromApi()
        {
            try
            {
                var apiMeals = await _apiService.GetMealsByDateAsync(DateTime.Now);

                Meals.Clear();
                foreach (var apiMeal in apiMeals)
                {
                    Meals.Add(new Meal
                    {
                        // Id = apiMeal.MealLogId, // Se tiver ID no model Meal
                        Name = apiMeal.MealType,
                        Time = apiMeal.Date.TimeOfDay,
                        Icon = "apple_icon.png",
                        Kcal = 0, // Precisa vir da API ou calcular
                        ItemsCount = apiMeal.Items?.Count ?? 0,
                        // FoodItems = ... mapear se a API retornar detalhes
                        IsConsumed = true // Se veio do histórico, assume que comeu? Ou a API deve retornar o status
                    });
                }
                UpdateMacros();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar: {ex.Message}");
            }
        }

        // --- SALVAR NO BANCO ---
        private async Task SaveMealToApi(Meal meal)
        {
            // Converte Model -> DTO
            var dto = new LogMealDto
            {
                Date = DateTime.Today.Add(meal.Time),
                MealType = meal.Name,
                Items = meal.FoodItems.Select(f => new MealItemDto
                {
                    FoodId = f.FoodId, // ATENÇÃO: FoodItem precisa ter ID válido!
                    Quantity = 1
                }).ToList()
            };

            bool sucesso = await _apiService.LogMealAsync(dto);

            if (!sucesso)
            {
                // Opcional: Mostrar aviso discreto ou tentar novamente depois
                Console.WriteLine("Falha ao salvar refeição na nuvem.");
            }
        }

        // --- AÇÕES DA TELA ---

        [RelayCommand]
        private async Task AddMeal()
        {
            await Shell.Current.GoToAsync(nameof(Views.Diet.MealEditorPage));
        }

        [RelayCommand]
        private async Task EditMeal(Meal meal)
        {
            if (meal == null) return;
            var navParam = new Dictionary<string, object> { { "MealToEdit", meal } };
            await Shell.Current.GoToAsync(nameof(Views.Diet.MealEditorPage), navParam);
        }

        [RelayCommand]
        private async Task DeleteMeal(Meal meal)
        {
            if (meal == null) return;
            bool confirm = await Shell.Current.DisplayAlert("Excluir", $"Remover {meal.Name}?", "Sim", "Não");

            if (confirm)
            {
                Meals.Remove(meal);
                UpdateMacros();

                // TODO: Chamar API para deletar (se tiver ID)
                // await _apiService.DeleteMealLogAsync(meal.Id);
            }
        }

        // --- LÓGICA DE CONSUMO (Gamification + Stats) ---
        [RelayCommand]
        private async Task ConsumeMeal(Meal meal)
        {
            if (meal == null) return;

            meal.IsConsumed = !meal.IsConsumed;
            UpdateMacros();

            // Calcula valor para o gráfico (+ ou -)
            int caloriesToSend = meal.IsConsumed ? meal.Kcal : -meal.Kcal;

            // Envia para StatsPage (Gráfico)
            WeakReferenceMessenger.Default.Send(new DietConsumedMessage(caloriesToSend));

            // Envia para Pet (Gamification) - Só se marcou
            if (meal.IsConsumed)
            {
                WeakReferenceMessenger.Default.Send(new PetUpdateMessage(PetActionType.MealFinished));
            }
        }

        private void UpdateMacros()
        {
            if (Meals == null) return;

            // Só soma o que foi marcado como consumido
            var consumedMeals = Meals.Where(m => m.IsConsumed).ToList();

            CurrentCalories = consumedMeals.Sum(m => m.Kcal);
            ProteinConsumed = consumedMeals.Sum(m => m.FoodItems?.Sum(fi => fi.Protein) ?? 0);
            CarbsConsumed = consumedMeals.Sum(m => m.FoodItems?.Sum(fi => fi.Carbs) ?? 0);
            FatConsumed = consumedMeals.Sum(m => m.FoodItems?.Sum(fi => fi.Fat) ?? 0);

            OnPropertyChanged(nameof(FormattedProteinConsumed));
            OnPropertyChanged(nameof(FormattedCarbsConsumed));
            OnPropertyChanged(nameof(FormattedFatConsumed));
        }
    }
}