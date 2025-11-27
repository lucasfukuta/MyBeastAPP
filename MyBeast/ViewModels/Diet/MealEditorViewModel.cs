using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyBeast.Models;
using MyBeast.Messages;
using MyBeast.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyBeast.ViewModels.Diet
{
    // Mensagem usada para avisar a tela anterior que salvou
    public class MealSavedMessage : ValueChangedMessage<Meal>
    {
        public MealSavedMessage(Meal value) : base(value) { }
    }

    public partial class MealEditorViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IApiService _apiService;

        // Cache dos alimentos vindos do Banco de Dados (API)
        private List<FoodItem> _allDatabaseFoods = new();

        // --- Propriedades da Tela ---
        [ObservableProperty] private string pageTitle = "Nova Refeição";
        [ObservableProperty] private string name;
        [ObservableProperty] private TimeSpan time;

        // --- MUDANÇA 1: Barra de Pesquisa ---
        // Propriedade manual para evitar conflito com o método OnSearchTextChanged gerado
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PerformSearch(value); // Chama a busca a cada letra digitada
                }
            }
        }

        // Listas Visuais
        public ObservableCollection<FoodItem> FoodItems { get; } = new(); // Itens do prato
        public ObservableCollection<FoodItem> SearchResults { get; } = new(); // Sugestões da busca

        // Propriedade Calculada (Total)
        public int TotalCalories => FoodItems.Sum(item => item.Kcal);

        private Meal _existingMeal; // Guarda a referência se for edição

        // --- CONSTRUTOR ---
        public MealEditorViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Time = DateTime.Now.TimeOfDay;

            // Monitora a lista para recalcular o total quando adiciona/remove itens
            FoodItems.CollectionChanged += FoodItems_CollectionChanged;

            // Carrega os dados da API silenciosamente assim que abre
            LoadFoodDatabase();
        }

        // --- LÓGICA DE CARREGAMENTO E BUSCA ---
        private async void LoadFoodDatabase()
        {
            try
            {
                var foodsFromApi = await _apiService.GetFoodTemplatesAsync();

                if (foodsFromApi != null && foodsFromApi.Any())
                {
                    _allDatabaseFoods.Clear();
                    foreach (var apiItem in foodsFromApi)
                    {
                        _allDatabaseFoods.Add(new FoodItem
                        {
                            Name = apiItem.Name,
                            Kcal = (int)apiItem.Calories,
                            Protein = (int)apiItem.Protein,
                            Carbs = (int)apiItem.Carbs,
                            Fat = (int)apiItem.Fat
                        });
                    }

                    // --- DIAGNÓSTICO 1: SUCESSO NO CARREGAMENTO ---
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] SUCESSO! Carregados {_allDatabaseFoods.Count} alimentos da API.");
                }
                else
                {
                    // --- DIAGNÓSTICO 2: API RETORNOU VAZIO ---
                    System.Diagnostics.Debug.WriteLine("[DEBUG] ALERTA: A API conectou, mas a lista veio vazia (0 itens).");
                }
            }
            catch (Exception ex)
            {
                // --- DIAGNÓSTICO 3: ERRO TÉCNICO ---
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ERRO CRÍTICO ao carregar: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Falha ao carregar alimentos.", "OK");
            }
        }

        // Lógica de busca robusta
        private void PerformSearch(string value)
        {
            SearchResults.Clear();

            if (string.IsNullOrWhiteSpace(value)) return;

            // Proteção contra lista vazia
            if (_allDatabaseFoods == null || !_allDatabaseFoods.Any())
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Tentou buscar, mas a lista do banco ainda está vazia.");
                return;
            }

            // --- DIAGNÓSTICO 4: O QUE ESTÁ SENDO DIGITADO ---
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Filtrando por: '{value}'...");

            var filtered = _allDatabaseFoods
                .Where(f => f.Name != null && f.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();

            foreach (var item in filtered)
            {
                SearchResults.Add(item);
                // --- DIAGNÓSTICO 5: O QUE FOI ENCONTRADO ---
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Item encontrado e adicionado na tela: {item.Name}");
            }

            if (filtered.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Nenhum item corresponde à busca.");
            }
        }

        // --- MUDANÇA 3: Selecionar Alimento da Busca ---
        [RelayCommand]
        private void SelectFood(FoodItem item)
        {
            if (item == null) return;

            // Adiciona uma CÓPIA do item selecionado à refeição
            FoodItems.Add(new FoodItem
            {
                Name = item.Name,
                Kcal = (int)item.Kcal,
                Protein = (int)item.Protein,
                Carbs = (int)item.Carbs,
                Fat = (int)item.Fat
            });

            // Limpa a busca
            SearchText = string.Empty;
            SearchResults.Clear();
        }

        // --- LÓGICA DE EDIÇÃO DA LISTA (MANUAL) ---

        [RelayCommand]
        private void AddNewItem()
        {
            // Adiciona uma linha vazia manual
            FoodItems.Add(new FoodItem { Name = "", Kcal = 0 });
        }

        [RelayCommand]
        private void RemoveItem(FoodItem item)
        {
            if (FoodItems.Contains(item))
                FoodItems.Remove(item);
        }

        // Monitora mudanças na coleção para atualizar o TotalCalories
        private void FoodItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalCalories));

            // Se adicionou item novo, escuta mudanças na propriedade Kcal dele
            if (e.NewItems != null)
            {
                foreach (FoodItem item in e.NewItems)
                    item.PropertyChanged += FoodItem_PropertyChanged;
            }

            // Se removeu item, para de escutar
            if (e.OldItems != null)
            {
                foreach (FoodItem item in e.OldItems)
                    item.PropertyChanged -= FoodItem_PropertyChanged;
            }
        }

        // Monitora se o usuário digitou um novo valor de Kcal num item existente
        private void FoodItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FoodItem.Kcal))
            {
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        // --- SALVAR ---

        [RelayCommand]
        private async Task Save()
        {
            // Se for edição usa o existente, se não cria novo
            var mealToSave = _existingMeal ?? new Meal();

            mealToSave.Name = string.IsNullOrWhiteSpace(Name) ? "Refeição" : Name;
            mealToSave.Time = Time;
            mealToSave.Icon = "apple_icon.png";

            // Salva a lista e o total calculado
            mealToSave.FoodItems = new List<FoodItem>(FoodItems);
            mealToSave.Kcal = TotalCalories;
            mealToSave.ItemsCount = FoodItems.Count;

            // Avisa a tela anterior (DietPage)
            WeakReferenceMessenger.Default.Send(new MealSavedMessage(mealToSave));

            await Shell.Current.GoToAsync("..");
        }

        // --- NAVEGAÇÃO (Recebe dados ao abrir) ---
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // Modo EDIÇÃO
            if (query.ContainsKey("MealToEdit") && query["MealToEdit"] is Meal meal)
            {
                _existingMeal = meal;
                PageTitle = "Editar Refeição";
                Name = meal.Name;
                Time = meal.Time;

                FoodItems.Clear();
                if (meal.FoodItems != null)
                {
                    foreach (var item in meal.FoodItems)
                    {
                        FoodItems.Add(new FoodItem
                        {
                            Name = item.Name,
                            Kcal = (int)item.Kcal,
                            Protein = (int)item.Protein,
                            Carbs = (int)item.Carbs,
                            Fat = (int)item.Fat
                        });
                    }
                }
            }
            // Modo CRIAÇÃO
            else
            {
                _existingMeal = null;
                PageTitle = "Nova Refeição";
                Name = string.Empty;
                Time = DateTime.Now.TimeOfDay;
                FoodItems.Clear();
            }
        }
    }
}