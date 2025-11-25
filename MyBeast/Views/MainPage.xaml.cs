namespace MyBeast
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnProfileAvatarTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("///ProfilePage");
        }
    }
}
