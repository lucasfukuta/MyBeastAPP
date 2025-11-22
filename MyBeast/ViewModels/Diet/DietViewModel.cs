using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using MyBeast.Services; // Importar a interface
using MyBeast.Domain.Entities; // Importar Entidades reais

namespace MyBeast.ViewModels.Diet
{
    public class DietViewModel : BindableObject
    {
        private readonly IDietService _dietService; // Injeção de Dependência

        // --- Propriedades de Controle ---
        private DateTime _currentDate;
        private bool _isAddFormVisible;
        private bool _isLoading;

        // --- Campos Novo Alimento ---
        private string _newItemName;
        private double _newItemCalories;
        private double _newItemProtein;
        private double _newItemCarbs;
        private double _newItemFat;
        private double _newItemQuantity = 1;
        private string _selectedMealType;

        // --- Totais ---
        private double _currentCalories;
        private double _targetCalories = 3200;
        private double _currentProtein;
        private double _currentCarbs;
        private double _currentFat;

        public ObservableCollection<MealEntryDisplay> DailyMeals { get; set; }
        public ObservableCollection<string> AvailableMealTypes { get; } = new ObservableCollection<string>
        {
            "Café da Manhã", "Almoço", "Lanche", "Jantar", "Ceia"
        };

        public ICommand PreviousDateCommand { get; }
        public ICommand NextDateCommand { get; }
        public ICommand ToggleAddFormCommand { get; }
        public ICommand SaveMealCommand { get; }
        public ICommand RemoveItemCommand { get; }

        // CONSTRUTOR: Recebe o serviço
        public DietViewModel(IDietService dietService)
        {
            _dietService = dietService; // O Mock será injetado aqui

            DailyMeals = new ObservableCollection<MealEntryDisplay>();
            CurrentDate = DateTime.Today;
            SelectedMealType = AvailableMealTypes[0];

            PreviousDateCommand = new Command(() => CurrentDate = CurrentDate.AddDays(-1));
            NextDateCommand = new Command(() => CurrentDate = CurrentDate.AddDays(1));
            ToggleAddFormCommand = new Command(() => IsAddFormVisible = !IsAddFormVisible);

            // Agora as ações chamam métodos assíncronos
            SaveMealCommand = new Command(async () => await SaveMealAsync());
            RemoveItemCommand = new Command<MealItemDisplay>(async (item) => await RemoveItemAsync(item));

            // Carrega dados iniciais
            LoadDataForDate(CurrentDate);
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged();
                LoadDataForDate(_currentDate);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public bool IsAddFormVisible
        {
            get => _isAddFormVisible;
            set { _isAddFormVisible = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAddButtonVisible)); }
        }
        public bool IsAddButtonVisible => !IsAddFormVisible;

        // Inputs
        public string NewItemName { get => _newItemName; set { _newItemName = value; OnPropertyChanged(); } }
        public double NewItemCalories { get => _newItemCalories; set { _newItemCalories = value; OnPropertyChanged(); } }
        public double NewItemProtein { get => _newItemProtein; set { _newItemProtein = value; OnPropertyChanged(); } }
        public double NewItemCarbs { get => _newItemCarbs; set { _newItemCarbs = value; OnPropertyChanged(); } }
        public double NewItemFat { get => _newItemFat; set { _newItemFat = value; OnPropertyChanged(); } }
        public double NewItemQuantity { get => _newItemQuantity; set { _newItemQuantity = value; OnPropertyChanged(); } }
        public string SelectedMealType { get => _selectedMealType; set { _selectedMealType = value; OnPropertyChanged(); } }

        // Totais
        public double CurrentCalories { get => _currentCalories; set { _currentCalories = value; OnPropertyChanged(); OnPropertyChanged(nameof(CaloriesProgress)); } }
        public double TargetCalories { get => _targetCalories; set { _targetCalories = value; OnPropertyChanged(); OnPropertyChanged(nameof(CaloriesProgress)); } }
        public double CaloriesProgress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
        public double CurrentProtein { get => _currentProtein; set { _currentProtein = value; OnPropertyChanged(); } }
        public double CurrentCarbs { get => _currentCarbs; set { _currentCarbs = value; OnPropertyChanged(); } }
        public double CurrentFat { get => _currentFat; set { _currentFat = value; OnPropertyChanged(); } }

        // --- Lógica Conectada ao Mock ---

        private async void LoadDataForDate(DateTime date)
        {
            IsLoading = true;
            DailyMeals.Clear();

            // 1. Busca os dados do serviço (Mock)
            var mealLogs = await _dietService.GetMealsByDateAsync(date);

            // 2. Converte as Entidades (MealLog) para os objetos de tela (MealEntryDisplay)
            foreach (var log in mealLogs)
            {
                var entryDisplay = new MealEntryDisplay { MealType = log.MealType };

                foreach (var item in log.MealLogItems)
                {
                    entryDisplay.Items.Add(new MealItemDisplay
                    {
                        Id = item.MealLogItemId, // Importante guardar o ID para deletar depois
                        Food = new FoodItemDisplay
                        {
                            Name = item.FoodItem.Name,
                            Calories = (double)item.FoodItem.Calories, // Cast de decimal para double
                            Protein = (double)item.FoodItem.Protein,
                            Carbs = (double)item.FoodItem.Carbs,
                            Fat = (double)item.FoodItem.Fat,
                            IsCustom = item.FoodItem.IsCustom
                        },
                        Quantity = (double)item.Quantity
                    });
                }
                entryDisplay.RefreshTotal(); // Recalcula total da refeição
                DailyMeals.Add(entryDisplay);
            }

            CalculateTotals();
            IsLoading = false;
        }

        private async Task SaveMealAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItemName) || string.IsNullOrWhiteSpace(SelectedMealType)) return;

            // 1. Cria entidade FoodItem
            var newFood = new FoodItem
            {
                Name = NewItemName,
                Calories = (decimal)NewItemCalories,
                Protein = (decimal)NewItemProtein,
                Carbs = (decimal)NewItemCarbs,
                Fat = (decimal)NewItemFat,
                IsCustom = true
            };

            // 2. Envia para o serviço
            await _dietService.AddFoodItemAsync(CurrentDate, SelectedMealType, newFood, NewItemQuantity);

            // 3. Recarrega a tela (para pegar IDs gerados e totais atualizados)
            LoadDataForDate(CurrentDate);

            // Limpa form
            NewItemName = ""; NewItemCalories = 0; NewItemProtein = 0; NewItemCarbs = 0; NewItemFat = 0; NewItemQuantity = 1;
            IsAddFormVisible = false;
        }

        private async Task RemoveItemAsync(MealItemDisplay item)
        {
            if (item == null) return;

            // Chama o serviço para deletar pelo ID
            await _dietService.RemoveFoodItemAsync(item.Id);

            // Recarrega dados
            LoadDataForDate(CurrentDate);
        }

        private void CalculateTotals()
        {
            double cal = 0, prot = 0, carb = 0, fat = 0;
            foreach (var meal in DailyMeals)
            {
                foreach (var item in meal.Items)
                {
                    cal += item.TotalItemCalories;
                    prot += item.Food.Protein * item.Quantity;
                    carb += item.Food.Carbs * item.Quantity;
                    fat += item.Food.Fat * item.Quantity;
                }
            }
            CurrentCalories = cal;
            CurrentProtein = prot;
            CurrentCarbs = carb;
            CurrentFat = fat;
        }
    }

    // --- Models de Visualização (Podem ficar no mesmo arquivo ou separar) ---
    public class MealEntryDisplay : BindableObject
    {
        public string MealType { get; set; }
        public ObservableCollection<MealItemDisplay> Items { get; set; } = new();
        private double _totalCalories;
        public double TotalCalories { get => _totalCalories; set { _totalCalories = value; OnPropertyChanged(); } }

        public void RefreshTotal() => TotalCalories = Items?.Sum(i => i.TotalItemCalories) ?? 0;
    }

    public class MealItemDisplay
    {
        public int Id { get; set; } // ID do LogItem para exclusão
        public FoodItemDisplay Food { get; set; }
        public double Quantity { get; set; }
        public double TotalItemCalories => Food.Calories * Quantity;
    }

    public class FoodItemDisplay
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public bool IsCustom { get; set; }
    }
}