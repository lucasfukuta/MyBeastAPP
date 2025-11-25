using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace MyBeast.Models
{
    public partial class MealTemplate : ObservableObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalKcal { get; set; }
        public List<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}
