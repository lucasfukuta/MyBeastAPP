using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyBeast.ViewModels.Auth
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        public ICommand BackCommand { get; private set; }

        public ForgotPasswordViewModel()
        {
            BackCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
