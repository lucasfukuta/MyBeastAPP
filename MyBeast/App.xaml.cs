using Microsoft.Extensions.DependencyInjection;
using MyBeast.Views.Diet;
using MyBeast.Views.Workout; 

namespace MyBeast
{
    public partial class App : Application
    {
        // 2. Modifique o construtor para receber a DietPage pronta
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            MainPage = serviceProvider.GetRequiredService<WorkoutSummaryPage>();
        }
    }
}