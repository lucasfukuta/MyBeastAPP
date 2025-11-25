namespace MyBeast.Views.Community;

public partial class CommunityFeedPage : ContentPage
{
    public CommunityFeedPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.Community.CommunityFeedViewModel();
    }
}