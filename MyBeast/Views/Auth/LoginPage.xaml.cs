using MyBeast.ViewModels;
using MyBeast.ViewModels.Auth;

namespace MyBeast.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }
    }
}
