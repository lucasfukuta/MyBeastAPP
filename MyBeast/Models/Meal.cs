using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace MyBeast.Models
{
    public partial class Meal : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private TimeSpan time;

        [ObservableProperty]
        private int kcal;

        [ObservableProperty]
        private int itemsCount;

        [ObservableProperty]
        private string icon;

        [ObservableProperty]
        private bool isConsumed;

        public List<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }

    // --- AQUI ESTÁ A MUDANÇA CRUCIAL PARA O CÁLCULO FUNCIONAR ---
    public partial class FoodItem : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int kcal; // Agora gera notificações de mudança!

        [ObservableProperty]
        private int protein;

        [ObservableProperty]
        private int carbs;

        [ObservableProperty]
        private int fat;
    }
}