using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Services;

namespace MyBeast.ViewModels.Auth
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool hasError;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Login()
        {
            try
            {
                // 1. Validação
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Usuário e senha são obrigatórios.";
                    HasError = true;
                    return;
                }

                // 2. Chama a API
                bool isAuthenticated = await _authService.LoginAsync(Username, Password);

                if (isAuthenticated)
                {
                    HasError = false;
                    // 3. Navega para a Home (Rota absoluta para limpar o histórico)
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    ErrorMessage = "Usuário ou senha inválidos.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro de conexão: {ex.Message}";
                HasError = true;
            }
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            // Navega para a tela de registro
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}