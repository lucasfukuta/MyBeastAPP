namespace MyBeast
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new MyBeast.Views.Diet.DietPage();
        }
    }
}
