using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace MyBeast.ViewModels.Diet
{
    public class DietViewModel : BindableObject
    {
        // --- Propriedades de Controle da Tela ---
        private DateTime _currentDate;
        private bool _isAddFormVisible;

        // --- Campos para Novo Alimento (Input do Usuário) ---
        private string _newItemName;
        private double _newItemCalories;
        private double _newItemProtein;
        private double _newItemCarbs;
        private double _newItemFat;
        private double _newItemQuantity = 1;
        private string _selectedMealType;

        // --- Totais e Metas ---
        private double _currentCalories;
        private double _targetCalories;
        private double _currentProtein;
        private double _currentCarbs;
        private double _currentFat;

        // --- Coleções ---
        public ObservableCollection<MealEntryDisplay> DailyMeals { get; set; }
        public ObservableCollection<string> AvailableMealTypes { get; } = new ObservableCollection<string>
        {
            "Café da Manhã", "Almoço", "Lanche", "Jantar", "Ceia"
        };

        // --- Comandos ---
        public ICommand PreviousDateCommand { get; }
        public ICommand NextDateCommand { get; }
        public ICommand ToggleAddFormCommand { get; } // Abre/Fecha formulário
        public ICommand SaveMealCommand { get; }      // Salva o alimento
        public ICommand RemoveItemCommand { get; }    // Remove o item

        public DietViewModel()
        {
            DailyMeals = new ObservableCollection<MealEntryDisplay>();
            TargetCalories = 3200;
            CurrentDate = DateTime.Today;
            SelectedMealType = AvailableMealTypes[0]; // Padrão

            PreviousDateCommand = new Command(() => CurrentDate = CurrentDate.AddDays(-1));
            NextDateCommand = new Command(() => CurrentDate = CurrentDate.AddDays(1));

            ToggleAddFormCommand = new Command(() => IsAddFormVisible = !IsAddFormVisible);

            SaveMealCommand = new Command(SaveMeal);

            RemoveItemCommand = new Command<MealItemDisplay>(RemoveItem);
        }

        // --- Getters e Setters com Notificação ---
        public DateTime CurrentDate
        {
            get => _currentDate;
            set { _currentDate = value; OnPropertyChanged(); LoadDataForDate(_currentDate); }
        }

        public bool IsAddFormVisible
        {
            get => _isAddFormVisible;
            set { _isAddFormVisible = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAddButtonVisible));

            }
        }

        public bool IsAddButtonVisible => !IsAddFormVisible;

        // Inputs do Formulário
        public string NewItemName { get => _newItemName; set { _newItemName = value; OnPropertyChanged(); } }
        public double NewItemCalories { get => _newItemCalories; set { _newItemCalories = value; OnPropertyChanged(); } }
        public double NewItemProtein { get => _newItemProtein; set { _newItemProtein = value; OnPropertyChanged(); } }
        public double NewItemCarbs { get => _newItemCarbs; set { _newItemCarbs = value; OnPropertyChanged(); } }
        public double NewItemFat { get => _newItemFat; set { _newItemFat = value; OnPropertyChanged(); } }
        public double NewItemQuantity { get => _newItemQuantity; set { _newItemQuantity = value; OnPropertyChanged(); } }
        public string SelectedMealType { get => _selectedMealType; set { _selectedMealType = value; OnPropertyChanged(); } }

        // Totais
        public double CurrentCalories
        {
            get => _currentCalories;
            set { _currentCalories = value; OnPropertyChanged(); OnPropertyChanged(nameof(CaloriesProgress)); }
        }
        public double TargetCalories { get => _targetCalories; set { _targetCalories = value; OnPropertyChanged(); OnPropertyChanged(nameof(CaloriesProgress)); } }
        public double CaloriesProgress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;

        public double CurrentProtein { get => _currentProtein; set { _currentProtein = value; OnPropertyChanged(); } }
        public double CurrentCarbs { get => _currentCarbs; set { _currentCarbs = value; OnPropertyChanged(); } }
        public double CurrentFat { get => _currentFat; set { _currentFat = value; OnPropertyChanged(); } }


        // --- Lógica ---

        private void LoadDataForDate(DateTime date)
        {
            // Aqui você integrará com o GET /api/MealLogs/me/date/{date} depois
            DailyMeals.Clear();

            // Dados de exemplo para não iniciar vazio
            if (date.Date == DateTime.Today)
            {
                var almoco = new MealEntryDisplay { MealType = "Almoço" };
                almoco.Items.Add(new MealItemDisplay
                {
                    Food = new FoodItemDisplay { Name = "Frango Grelhado", Calories = 165, Protein = 31, Carbs = 0, Fat = 3.6 },
                    Quantity = 1
                });
                DailyMeals.Add(almoco);
            }
            CalculateTotals();
        }

        private void SaveMeal()
        {
            if (string.IsNullOrWhiteSpace(NewItemName) || string.IsNullOrWhiteSpace(SelectedMealType))
                return;

            // 1. Cria o objeto do alimento (como se fosse o FoodItem do backend)
            var food = new FoodItemDisplay
            {
                Name = NewItemName,
                Calories = NewItemCalories,
                Protein = NewItemProtein,
                Carbs = NewItemCarbs,
                Fat = NewItemFat,
                IsCustom = true
            };

            // 2. Encontra ou cria a categoria da refeição (Ex: Almoço)
            var mealEntry = DailyMeals.FirstOrDefault(m => m.MealType == SelectedMealType);
            if (mealEntry == null)
            {
                mealEntry = new MealEntryDisplay { MealType = SelectedMealType };
                // Tenta inserir na ordem correta (opcional) ou apenas adiciona
                DailyMeals.Add(mealEntry);
            }

            // 3. Adiciona o item à lista
            mealEntry.Items.Add(new MealItemDisplay
            {
                Food = food,
                Quantity = NewItemQuantity
            });

            // 4. Atualiza a UI e limpa formulário
            mealEntry.RefreshTotal();
            CalculateTotals();

            // Resetar campos
            NewItemName = string.Empty;
            NewItemCalories = 0;
            NewItemProtein = 0;
            NewItemCarbs = 0;
            NewItemFat = 0;
            NewItemQuantity = 1;
            IsAddFormVisible = false; // Fecha o form
        }

        private void RemoveItem(MealItemDisplay item)
        {
            foreach (var meal in DailyMeals)
            {
                if (meal.Items.Contains(item))
                {
                    meal.Items.Remove(item);
                    meal.RefreshTotal();
                    // Se a refeição ficar vazia, removemos o cabeçalho também
                    if (meal.Items.Count == 0) DailyMeals.Remove(meal);
                    break;
                }
            }
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            double cal = 0, prot = 0, carb = 0, fat = 0;
            foreach (var meal in DailyMeals)
            {
                foreach (var item in meal.Items)
                {
                    cal += item.Food.Calories * item.Quantity;
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

    // --- Classes de Modelo (Mantidas aqui para facilitar) ---
    public class FoodItemDisplay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public bool IsCustom { get; set; }
    }

    public class MealItemDisplay
    {
        public FoodItemDisplay Food { get; set; }
        public double Quantity { get; set; }
        public double TotalItemCalories => Food.Calories * Quantity;
    }

    public class MealEntryDisplay : BindableObject
    {
        public string MealType { get; set; }
        public ObservableCollection<MealItemDisplay> Items { get; set; } = new();

        private double _totalCalories;
        public double TotalCalories { get => _totalCalories; set { _totalCalories = value; OnPropertyChanged(); } }

        public MealEntryDisplay()
        {
            Items.CollectionChanged += (s, e) => RefreshTotal();
            RefreshTotal();
        }
        public void RefreshTotal() => TotalCalories = Items?.Sum(i => i.Food.Calories * i.Quantity) ?? 0;
    }
}