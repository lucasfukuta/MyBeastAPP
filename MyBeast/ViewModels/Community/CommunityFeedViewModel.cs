using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Community
{
    public partial class CommunityFeedViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Post> posts;

        [ObservableProperty]
        private string newPostText;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string newCommentText; // Para a área de entrada de comentários

        public CommunityFeedViewModel()
        {
            Posts = new ObservableCollection<Post>();
            LoadSamplePosts();
        }

        [RelayCommand]
        private async Task PostMessage()
        {
            if (!string.IsNullOrWhiteSpace(NewPostText))
            {
                var newPost = new Post
                {
                    AuthorName = "Você",
                    AuthorInitials = "VC",
                    TimeAgo = "Agora",
                    Content = NewPostText,
                    Tag = "", // Pode ser uma opção para o usuário ou vazio
                    TagBackgroundColor = "Transparent",
                    Likes = 0,
                    Comments = 0,
                    IsLiked = false
                };
                Posts.Insert(0, newPost); // Adiciona a nova postagem no topo
                NewPostText = string.Empty;
                // Aqui você adicionaria a lógica para enviar a postagem para um serviço/backend
            }
        }

        [RelayCommand]
        private void LikePost(Post post)
        {
            if (post == null) return;

            if (post.IsLiked)
            {
                post.Likes--;
            }
            else
            {
                post.Likes++;
            }
            post.IsLiked = !post.IsLiked;
            // Notificar que a propriedade Posts foi alterada para que a UI seja atualizada
            OnPropertyChanged(nameof(Posts));
        }

        [RelayCommand]
        private void CommentPost(Post post)
        {
            if (post == null) return;
            post.IsCommentSectionVisible = !post.IsCommentSectionVisible; // Alterna a visibilidade da seção de comentários
            // Notificar que a propriedade IsCommentSectionVisible do post foi alterada
            OnPropertyChanged(nameof(Posts)); // Notifica a mudança na coleção para atualizar a UI
        }

        [RelayCommand]
        private void SubmitComment(Post post)
        {
            if (post == null || string.IsNullOrWhiteSpace(NewCommentText)) return;

            post.PostComments.Add($"Você: {NewCommentText}");
            post.Comments++;
            NewCommentText = string.Empty; // Limpa a caixa de texto após enviar
            // Notificar que a propriedade Posts foi alterada para que a UI seja atualizada
            OnPropertyChanged(nameof(Posts));
        }

        private void LoadSamplePosts()
        {
            // Limpar posts existentes antes de adicionar novos dados de exemplo
            Posts.Clear();

            Posts.Add(new Post
            {
                AuthorName = "Carlos Silva",
                AuthorInitials = "CS",
                TimeAgo = "2h atrás",
                Content = "Acabei de completar meu 30º treino consecutivo! 💪 A consistência é a chave!",
                Tag = "🏆 30 Dias de Sequência",
                TagBackgroundColor = "#FFA500",
                Likes = 42,
                Comments = 8,
                IsLiked = false,
                PostComments = new ObservableCollection<string> { "Ótimo, Carlos!", "Parabéns pela dedicação!" }
            });

            Posts.Add(new Post
            {
                AuthorName = "Ana Santos",
                AuthorInitials = "AS",
                TimeAgo = "5h atrás",
                Content = "Novo PR no leg press! 180kg x 10 reps. Quem disse que era impossível? 🔥",
                Tag = "",
                TagBackgroundColor = "Transparent",
                Likes = 67,
                Comments = 15,
                IsLiked = false,
                PostComments = new ObservableCollection<string> { "Incrível, Ana!", "Muito forte!" }
            });

            Posts.Add(new Post
            {
                AuthorName = "Pedro Costa",
                AuthorInitials = "PC",
                TimeAgo = "1d atrás",
                Content = "Minha jornada de transformação: -15kg em 3 meses. Muito orgulhoso do progresso!",
                Tag = "",
                TagBackgroundColor = "Transparent",
                Likes = 124,
                Comments = 32,
                IsLiked = false,
                PostComments = new ObservableCollection<string> { "Parabéns, Pedro!", "Que inspiração!" }
            });
        }
    }
}
