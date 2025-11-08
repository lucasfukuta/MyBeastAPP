using Microsoft.Maui.Controls;
using MyBeast.Services; // Ajuste se Post estiver em outro namespace

namespace MyBeast.Views.Controls
{
    public partial class PostCardView : ContentView
    {
        public static readonly BindableProperty PostProperty =
            BindableProperty.Create(nameof(Post), typeof(Post), typeof(PostCardView));

        public Post? Post
        {
            get => (Post?)GetValue(PostProperty);
            set => SetValue(PostProperty, value);
        }

        public PostCardView()
        {
            InitializeComponent();
        }
    }
}
