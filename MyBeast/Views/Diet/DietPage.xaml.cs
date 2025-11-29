namespace MyBeast.Views.Diet;

public partial class DietPage : ContentPage
{
    public DietPage(DietViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}