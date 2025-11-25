using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Services;
using System.Diagnostics;

namespace MyBeast.ViewModels.Auth
{
    // 'partial' é obrigatório para o Toolkit gerar o código
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty] // Gera a propriedade pública Username com notificação
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private string successMessage;

        [ObservableProperty]
        private bool hasSuccess;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        // Gera o comando 'BackCommand' automaticamente
        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        // Gera o comando 'RegisterUserCommand' automaticamente
        [RelayCommand]
        private async Task RegisterUser()
        {
            try
            {
                HasError = false;
                HasSuccess = false; // Reseta sucesso também
                ErrorMessage = string.Empty;

                // Validação Básica
                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ErrorMessage = "Todos os campos são obrigatórios.";
                    HasError = true;
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "As senhas não coincidem.";
                    HasError = true;
                    return;
                }

                // CHAMADA AO SERVIÇO (CORRIGIDA A ORDEM DOS PARÂMETROS)
                // Verifica a assinatura do seu IAuthService. Se for (user, pass, email):
                var result = await _authService.RegisterAsync(Username, Password, Email);

                if (result)
                {
                    SuccessMessage = "Conta criada! Redirecionando...";
                    HasSuccess = true;

                    // Pequeno delay para o usuário ler a mensagem
                    await Task.Delay(1500);
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    ErrorMessage = "Falha ao registrar. O email ou usuário já existe?";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro: {ex.Message}");
                ErrorMessage = "Erro de conexão com o servidor.";
                HasError = true;
            }
        }

    }
}