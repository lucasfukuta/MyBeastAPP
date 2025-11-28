using CommunityToolkit.Mvvm.ComponentModel;

namespace MyBeast.Models
{
    public partial class FoodItem : ObservableObject
    {
        [ObservableProperty]
        private int foodId;

        [ObservableProperty]
        private string name;

        // O Toolkit gera: public int Kcal { get; set; }
        [ObservableProperty]
        private int kcal;

        [ObservableProperty]
        private int protein;

        [ObservableProperty]
        private int carbs;

        [ObservableProperty]
        private int fat;
    }
}