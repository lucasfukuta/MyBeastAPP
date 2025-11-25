using System;

namespace MyBeast.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string Type { get; set; } // "Conquista", "Comentário", "Treino", "Dieta"
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; }
    }
}
