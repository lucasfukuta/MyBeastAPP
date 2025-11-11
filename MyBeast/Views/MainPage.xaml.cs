namespace MyBeast.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Método chamado quando o usuário clica no botão "Criar conta"
        private async void onCriarConta(object sender, EventArgs e)
        {
            // Aqui você pode navegar para a página de registro
            await Shell.Current.GoToAsync("//register");
        }

    }
}
