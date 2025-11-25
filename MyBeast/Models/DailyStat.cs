namespace MyBeast.Models
{
    public class DailyStat
    {
        public string Day { get; set; } // "Seg", "Ter", etc.
        public double Calories { get; set; }

        public DailyStat(string day, double calories)
        {
            Day = day;
            Calories = calories;
        }
    }
}