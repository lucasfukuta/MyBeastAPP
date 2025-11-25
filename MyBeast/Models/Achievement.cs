namespace MyBeast.Models
{
    public class Achievement
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Icon { get; set; } // "trophy_icon.png"
        public string BadgeIcon { get; set; } // "achieved_badge.png"
        public bool IsUnlocked { get; set; }
    }
}