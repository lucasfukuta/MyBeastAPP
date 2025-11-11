namespace MyBeast.Views.Auth
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        // Método chamado quando o usuário clica no botão "Login" (link para Login)
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Navegar de volta para a página de login
            await Shell.Current.GoToAsync("//mainpage");  // Usando a rota definida no AppShell.xaml
        }
    }
}
