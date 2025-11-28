using MyBeast.ViewModels.Stats; 

namespace MyBeast
{
    public partial class App : Application
    {
        // Adicione o parâmetro statsViewModel no construtor
        public App(StatsPageViewModel statsViewModel)
        {
            InitializeComponent();

            //UserAppTheme = AppTheme.Dark;

            MainPage = new AppShell();
        }
    }
}