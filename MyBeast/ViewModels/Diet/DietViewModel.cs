using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Diet
{
    internal class DietViewModel
    {
        public string Name { get; set; }
        public int Calories { get; set; }
        public List<string> Meals { get; set; }

        public DietViewModel()
        {
            Meals = new List<string>();
        }

        public void AddMeal(string meal)
        {
            if (!string.IsNullOrWhiteSpace(meal))
            {
                Meals.Add(meal);
            }
        }

        public void RemoveMeal(string meal)
        {
            Meals.Remove(meal);
        }

        public int TotalMeals()
        {
            return Meals.Count;
        }
    }
}
