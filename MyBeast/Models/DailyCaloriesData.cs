namespace MyBeast.Models
{
    public class DailyCaloriesData
    {
        public string Day { get; set; }      // Eixo X (Ex: "Seg")
        public double Calories { get; set; } // Eixo Y (Ex: 500)

        public DailyCaloriesData(string day, double calories)
        {
            Day = day;
            Calories = calories;
        }
    }
}