using MyBeast.ViewModels.Diet; 

namespace MyBeast.Views.Diet;

public partial class MealEditorPage : ContentPage
{
    public MealEditorPage(MealEditorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}