using MyBeast.Domain.Entities;
using MyBeast.Services;
using System.Collections.ObjectModel;

internal class AchievementsViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<Achievement> Achievements { get; set; }

    public AchievementsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Achievements = new ObservableCollection<Achievement>();
    }

    public async Task LoadAchievementsAsync()
    {
        var achievements = await _apiService.GetAchievementsAsync("https://api.mybeast.com/achievements");
        Achievements.Clear();
        foreach (var achievement in achievements)
        {
            Achievements.Add(achievement);
        }
    }
}

