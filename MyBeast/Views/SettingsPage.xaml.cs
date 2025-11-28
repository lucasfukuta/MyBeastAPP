namespace MyBeast.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

        if (Application.Current.UserAppTheme == AppTheme.Light)
        {
            ThemeSwitch.IsToggled = true;
        }
        else
        {
            ThemeSwitch.IsToggled = false;
        }
    }

    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value) 
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Preferences.Set("AppTheme", "Light"); 
        }
        else 
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            Preferences.Set("AppTheme", "Dark"); 
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//ProfilePage");
	}
}