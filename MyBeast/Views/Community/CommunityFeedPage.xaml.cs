using System.Collections.ObjectModel;
using System.ComponentModel;
using MyBeast.Views.Community;

public partial class CommunityFeedViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Post> Posts { get; set; } = new();
    public bool IsLoading { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadPostsAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        OnPropertyChanged(nameof(IsLoading));

        try
        {
            // Simule o carregamento dos posts
            await Task.Delay(1000);
            // Exemplo: Posts.Add(new Post { ... });
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
