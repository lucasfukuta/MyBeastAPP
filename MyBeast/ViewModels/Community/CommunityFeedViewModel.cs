using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyBeast.Views.Community;

public class CommunityFeedViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Post> Posts { get; set; }

    public CommunityFeedViewModel()
    {
        Posts = new ObservableCollection<Post>
        {
            new Post { Titulo = "Bem-vindo!", Conteudo = "Apresente-se para a comunidade.", Autor = "Admin" },
            new Post { Titulo = "Dica do dia", Conteudo = "Use o .NET MAUI para apps multiplataforma.", Autor = "João" }
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class Post
{
    public string Titulo { get; set; }
    public string Conteudo { get; set; }
    public string Autor { get; set; }
}

