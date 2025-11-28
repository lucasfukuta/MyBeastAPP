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
}