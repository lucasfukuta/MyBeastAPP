using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace MyBeast.Models
{
    public partial class Post : ObservableObject
    {
        public string AuthorName { get; set; }
        public string AuthorInitials { get; set; } // Para o avatar, e.g., "CS"
        public string TimeAgo { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; } // e.g., "🏆 30 Dias de Sequência"
        public string TagBackgroundColor { get; set; } // Cor de fundo da tag

        [ObservableProperty]
        private int likes;

        [ObservableProperty]
        private int comments;

        [ObservableProperty]
        private bool isLiked; // Para futura funcionalidade de like/deslike

        [ObservableProperty]
        private ObservableCollection<string> postComments = new ObservableCollection<string>();

        [ObservableProperty]
        private bool isCommentSectionVisible;

        public Post()
        {
            // Inicializar a coleção de comentários para evitar null reference
            PostComments = new ObservableCollection<string>();
        }
    }
}
