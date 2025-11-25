using MyBeast.ViewModels.Diet;

namespace MyBeast.Views.Diet;

public partial class DietPage : ContentPage
{
    public DietPage()
    {
        InitializeComponent();
        BindingContext = new DietViewModel();
    }
}