using Microsoft.Extensions.DependencyInjection;
using MyBeast.Views.Diet; // 1. Adicione este using se não tiver

namespace MyBeast
{
    public partial class App : Application
    {
        // 2. Modifique o construtor para receber a DietPage pronta
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = serviceProvider.GetRequiredService<MyBeast.Views.Diet.DietPage>(); ;
        }
    }
}