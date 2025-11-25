using MyBeast.ViewModels.Auth;

namespace MyBeast.Views.Auth
{
    public partial class RegisterPage : ContentPage
    {
        // O MAUI injeta o ViewModel aqui automaticamente
        public RegisterPage(RegisterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel; // Conecta o XAML ao C#
        }
    }
}