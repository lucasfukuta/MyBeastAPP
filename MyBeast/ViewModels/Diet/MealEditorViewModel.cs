using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyBeast.Models;
using MyBeast.Messages;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyBeast.ViewModels.Diet
{
    public class MealSavedMessage : CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<Meal>
    {
        public MealSavedMessage(Meal value) : base(value) { }
    }

    public partial class MealEditorViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private string pageTitle = "Nova Refeição";

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private TimeSpan time;

        // Lista de itens (Arroz, Feijão, etc.)
        public ObservableCollection<FoodItem> FoodItems { get; } = new();

        // Propriedade calculada: Soma das calorias dos itens
        public int TotalCalories => FoodItems.Sum(item => item.Kcal);

        private Meal _existingMeal;

        public MealEditorViewModel()
        {
            Time = DateTime.Now.TimeOfDay;

            // Observa se itens foram adicionados ou removidos para recalcular o total
            FoodItems.CollectionChanged += FoodItems_CollectionChanged;
        }

        // Toda vez que a lista muda (add/remove), recalculamos
        private void FoodItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalCalories));

            // Se adicionou item novo, precisamos "escutar" mudanças nele também
            if (e.NewItems != null)
            {
                foreach (FoodItem item in e.NewItems)
                    item.PropertyChanged += FoodItem_PropertyChanged;
            }

            // Se removeu, paramos de escutar
            if (e.OldItems != null)
            {
                foreach (FoodItem item in e.OldItems)
                    item.PropertyChanged -= FoodItem_PropertyChanged;
            }
        }

        // Se o usuário mudar o valor "Kcal" de um item, recalculamos o total
        private void FoodItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FoodItem.Kcal))
            {
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        [RelayCommand]
        private void AddNewItem()
        {
            // Adiciona uma linha vazia
            FoodItems.Add(new FoodItem { Name = "", Kcal = 0 });
        }

        [RelayCommand]
        private void RemoveItem(FoodItem item)
        {
            if (FoodItems.Contains(item))
                FoodItems.Remove(item);
        }

        [RelayCommand]
        private async Task Save()
        {
            var mealToSave = _existingMeal ?? new Meal();

            mealToSave.Name = Name;
            mealToSave.Time = Time;
            mealToSave.Icon = "apple_icon.png";

            // IMPORTANTE: Salvamos a lista atualizada e o total calculado
            mealToSave.FoodItems = new List<FoodItem>(FoodItems);
            mealToSave.Kcal = TotalCalories;
            mealToSave.ItemsCount = FoodItems.Count;

            WeakReferenceMessenger.Default.Send(new MealSavedMessage(mealToSave));

            await Shell.Current.GoToAsync("..");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("MealToEdit"))
            {
                _existingMeal = query["MealToEdit"] as Meal;
                if (_existingMeal != null)
                {
                    PageTitle = "Editar Refeição";
                    Name = _existingMeal.Name;
                    Time = _existingMeal.Time;

                    // Carrega os itens existentes para edição
                    FoodItems.Clear();
                    if (_existingMeal.FoodItems != null)
                    {
                        foreach (var item in _existingMeal.FoodItems)
                        {
                            // Criamos cópias para não editar o original antes de salvar
                            FoodItems.Add(new FoodItem
                            {
                                Name = item.Name,
                                Kcal = item.Kcal,
                                Protein = item.Protein,
                                Carbs = item.Carbs,
                                Fat = item.Fat
                            });
                        }
                    }
                }
            }
            else
            {
                // Se é nova, já adiciona um item vazio para facilitar
                FoodItems.Clear();
                FoodItems.Add(new FoodItem { Name = "", Kcal = 0 });
            }
        }
    }
}