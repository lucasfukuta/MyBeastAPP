namespace MyBeast.Views.Profile;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}

	private async void OnNotificationsTapped(object sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("///NotificationsPage");
	}

	private async void OnSettingsTapped(object sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("///SettingsPage");
	}

	private async void OnHelpSupportTapped(object sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("///HelpSupportPage");
	}

	private async void OnLogOutTapped(object sender, TappedEventArgs e)
	{
		await Shell.Current.GoToAsync("///LoginPage");
	}

	private async void OnBackButtonClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//HomePage");
	}
}
