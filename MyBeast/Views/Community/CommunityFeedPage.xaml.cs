// MyBeast/Views/Community/CommunityFeedPage.xaml.cs
using Microsoft.Maui.Controls;
using MyBeast.ViewModels.Community;

namespace MyBeast.Views.Community
{
    public partial class CommunityFeedPage : ContentPage
    {
        // 1. O ViewModel agora é injetado via construtor (o MauiProgram precisa estar configurado para isso)
        private readonly CommunityFeedViewModel _viewModel;

        public CommunityFeedPage(CommunityFeedViewModel viewModel)
        {
            InitializeComponent();

            // Atribui a instância injetada ao campo e ao BindingContext
            _viewModel = viewModel;
            BindingContext = _viewModel;

            // Para que o MauiProgram.cs funcione corretamente, ele precisará ter algo como:
            // builder.Services.AddSingleton<CommunityFeedPage>();
            // builder.Services.AddSingleton<CommunityFeedViewModel>();
        }

        // 2. Aciona o carregamento dos posts quando a página é exibida
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Carrega os posts se a lista estiver vazia (primeiro carregamento)
            if (_viewModel.Posts.Count == 0 && !_viewModel.IsLoading)
            {
                await _viewModel.LoadPostsAsync();
            }
        }
    }
}