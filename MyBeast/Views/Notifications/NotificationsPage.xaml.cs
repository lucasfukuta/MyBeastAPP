namespace MyBeast.Views.Notifications;

public partial class NotificationsPage : ContentPage
{
	public NotificationsPage()
	{
		InitializeComponent();
		BindingContext = new ViewModels.Notifications.NotificationsViewModel();
	}

	private async void OnBackButtonClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//ProfilePage");
	}
}