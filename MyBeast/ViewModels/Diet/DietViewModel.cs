using System.Collections.ObjectModel;
using System.Windows.Input;
using MyBeast.Domain.Entities;
using MyBeast.Services;

namespace MyBeast.ViewModels.Diet
{
    public class DietViewModel : BindableObject
    {
        private readonly IDietService _dietService;

        // --- Estado da Tela ---
        private DateTime _currentDate;
        private bool _isAddFormVisible;
        private bool _isLoading;

        // --- Dados de Entrada (Novo Alimento) ---
        // Usando strings para os inputs numéricos para evitar "0" fixo na UI antes de digitar, 
        // mas convertendo para decimal na lógica.
        private string _newItemName;
        private decimal _newItemCalories;
        private decimal _newItemProtein;
        private decimal _newItemCarbs;
        private decimal _newItemFat;
        private decimal _newItemQuantity = 1;
        private string _selectedMealType;

        // --- Totais do Dia (Usando decimal como nas Entities) ---
        private decimal _currentCalories;
        private decimal _targetCalories = 3200; // Exemplo fixo, poderia vir de User settings
        private decimal _currentProtein;
        private decimal _currentCarbs;
        private decimal _currentFat;

        // Coleção principal que a View vai observar
        public ObservableCollection<MealLogDisplay> DailyMeals { get; set; }

        // Lista estática de tipos de refeição para o Picker
        public ObservableCollection<string> AvailableMealTypes { get; } = new ObservableCollection<string>
        {
            "Café da Manhã", "Almoço", "Lanche da Tarde", "Jantar", "Ceia"
        };

        // --- Comandos ---
        public ICommand PreviousDateCommand { get; }
        public ICommand NextDateCommand { get; }
        public ICommand ToggleAddFormCommand { get; }
        public ICommand SaveMealCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public DietViewModel(IDietService dietService)
        {
            _dietService = dietService;

            DailyMeals = new ObservableCollection<MealLogDisplay>();
            CurrentDate = DateTime.Today;
            SelectedMealType = AvailableMealTypes[0];

            // Navegação de datas
            PreviousDateCommand = new Command(() => CurrentDate = CurrentDate.AddDays(-1));
            NextDateCommand = new Command(() => CurrentDate = CurrentDate.AddDays(1));

            // UI Control
            ToggleAddFormCommand = new Command(() => IsAddFormVisible = !IsAddFormVisible);

            // Operações CRUD (Async)
            SaveMealCommand = new Command(async () => await SaveMealAsync());
            RemoveItemCommand = new Command<MealLogItemDisplay>(async (item) => await RemoveItemAsync(item));

            // Carga Inicial
            LoadDataForDate(CurrentDate);
        }

        // --- Propriedades Bindable ---

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged();
                LoadDataForDate(_currentDate); // Recarrega ao mudar data
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
            set
            {
                _isAddFormVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAddButtonVisible)); // Atualiza o botão "+"
            }
        }
        public bool IsAddButtonVisible => !IsAddFormVisible;

        // --- Propriedades de Entrada ---
        public string NewItemName { get => _newItemName; set { _newItemName = value; OnPropertyChanged(); } }
        public decimal NewItemCalories { get => _newItemCalories; set { _newItemCalories = value; OnPropertyChanged(); } }
        public decimal NewItemProtein { get => _newItemProtein; set { _newItemProtein = value; OnPropertyChanged(); } }
        public decimal NewItemCarbs { get => _newItemCarbs; set { _newItemCarbs = value; OnPropertyChanged(); } }
        public decimal NewItemFat { get => _newItemFat; set { _newItemFat = value; OnPropertyChanged(); } }
        public decimal NewItemQuantity { get => _newItemQuantity; set { _newItemQuantity = value; OnPropertyChanged(); } }
        public string SelectedMealType { get => _selectedMealType; set { _selectedMealType = value; OnPropertyChanged(); } }

        // --- Propriedades de Totais (Macros) ---
        public decimal CurrentCalories { get => _currentCalories; set { _currentCalories = value; OnPropertyChanged(); OnPropertyChanged(nameof(CaloriesProgress)); } }
        public decimal TargetCalories { get => _targetCalories; set { _targetCalories = value; OnPropertyChanged(); OnPropertyChanged(nameof(CaloriesProgress)); } }

        // Progress Bar espera double, fazemos o cast aqui apenas para a View
        public double CaloriesProgress => (double)(TargetCalories > 0 ? CurrentCalories / TargetCalories : 0);

        public decimal CurrentProtein { get => _currentProtein; set { _currentProtein = value; OnPropertyChanged(); } }
        public decimal CurrentCarbs { get => _currentCarbs; set { _currentCarbs = value; OnPropertyChanged(); } }
        public decimal CurrentFat { get => _currentFat; set { _currentFat = value; OnPropertyChanged(); } }


        // --- Lógica de Negócio ---

        private async void LoadDataForDate(DateTime date)
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                // 1. Busca dados do Serviço (retorna IEnumerable<MealLog>)
                var mealLogsEntity = await _dietService.GetMealsByDateAsync(date);

                DailyMeals.Clear();

                // 2. Mapeia Entidades -> ViewModel Displays
                // Mesmo que não haja logs no banco, iteramos sobre os tipos conhecidos se quisermos mostrar seções vazias,
                // ou apenas mostramos o que veio do banco. Aqui vou mostrar o que veio do banco.

                foreach (var log in mealLogsEntity)
                {
                    var mealDisplay = new MealLogDisplay
                    {
                        MealType = log.MealType,
                        MealLogId = log.MealLogId
                    };

                    foreach (var item in log.MealLogItems)
                    {
                        mealDisplay.Items.Add(new MealLogItemDisplay
                        {
                            MealLogItemId = item.MealLogItemId,
                            FoodId = item.FoodId,
                            Name = item.FoodItem?.Name ?? "Desconhecido",
                            Quantity = item.Quantity,
                            CaloriesPerUnit = item.FoodItem?.Calories ?? 0,
                            ProteinPerUnit = item.FoodItem?.Protein ?? 0,
                            CarbsPerUnit = item.FoodItem?.Carbs ?? 0,
                            FatPerUnit = item.FoodItem?.Fat ?? 0,
                            IsCustom = item.FoodItem?.IsCustom ?? false
                        });
                    }

                    // Calcula totais daquela refeição específica
                    mealDisplay.RecalculateTotals();
                    DailyMeals.Add(mealDisplay);
                }

                // 3. Calcula Totais do Dia
                CalculateDailyTotals();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveMealAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItemName) || string.IsNullOrWhiteSpace(SelectedMealType))
                return;

            // 1. Constrói a entidade FoodItem (Domínio)
            var newFood = new FoodItem
            {
                Name = NewItemName,
                Calories = NewItemCalories,
                Protein = NewItemProtein,
                Carbs = NewItemCarbs,
                Fat = NewItemFat,
                IsCustom = true,
                UserId = 1 // TODO: Pegar do contexto do usuário logado
            };

            // 2. Chama o serviço (note o cast para double na quantidade, pois sua interface IDietService usa double, embora a Entity use decimal)
            // Idealmente, refatore IDietService para usar decimal também.
            await _dietService.AddFoodItemAsync(CurrentDate, SelectedMealType, newFood, (double)NewItemQuantity);

            // 3. Limpa campos e atualiza
            NewItemName = string.Empty;
            NewItemCalories = 0;
            NewItemProtein = 0;
            NewItemCarbs = 0;
            NewItemFat = 0;
            NewItemQuantity = 1;
            IsAddFormVisible = false;

            LoadDataForDate(CurrentDate);
        }

        private async Task RemoveItemAsync(MealLogItemDisplay item)
        {
            if (item == null) return;

            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", $"Remover {item.Name}?", "Sim", "Não");
            if (!confirm) return;

            await _dietService.RemoveFoodItemAsync(item.MealLogItemId);
            LoadDataForDate(CurrentDate);
        }

        private void CalculateDailyTotals()
        {
            CurrentCalories = DailyMeals.Sum(m => m.TotalCalories);
            CurrentProtein = DailyMeals.Sum(m => m.Items.Sum(i => i.TotalProtein));
            CurrentCarbs = DailyMeals.Sum(m => m.Items.Sum(i => i.TotalCarbs));
            CurrentFat = DailyMeals.Sum(m => m.Items.Sum(i => i.TotalFat));
        }
    }

    // --- Classes de Visualização (Helpers) ---

    // Representa uma "Refeição" (ex: Almoço) e sua lista de itens
    public class MealLogDisplay : BindableObject
    {
        public int MealLogId { get; set; }
        public string MealType { get; set; }
        public ObservableCollection<MealLogItemDisplay> Items { get; set; } = new();

        private decimal _totalCalories;
        public decimal TotalCalories
        {
            get => _totalCalories;
            set { _totalCalories = value; OnPropertyChanged(); }
        }

        public void RecalculateTotals()
        {
            TotalCalories = Items.Sum(i => i.TotalCalories);
        }
    }

    // Representa um Item dentro da refeição (FoodItem + Quantidade)
    public class MealLogItemDisplay
    {
        public int MealLogItemId { get; set; } // ID fundamental para deletar
        public int FoodId { get; set; }
        public string Name { get; set; }
        public bool IsCustom { get; set; }
        public decimal Quantity { get; set; }

        // Valores unitários (do FoodItem)
        public decimal CaloriesPerUnit { get; set; }
        public decimal ProteinPerUnit { get; set; }
        public decimal CarbsPerUnit { get; set; }
        public decimal FatPerUnit { get; set; }

        // Valores totais calculados (Visualização)
        public decimal TotalCalories => CaloriesPerUnit * Quantity;
        public decimal TotalProtein => ProteinPerUnit * Quantity;
        public decimal TotalCarbs => CarbsPerUnit * Quantity;
        public decimal TotalFat => FatPerUnit * Quantity;
    }
}