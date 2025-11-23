using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Domain.Entities;

namespace MyBeast.ViewModels.Workout
{
    public partial class WorkoutDetailViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private WorkoutSession session;

        // Este método é chamado automaticamente pelo Shell quando passamos parâmetros
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("Session"))
            {
                Session = query["Session"] as WorkoutSession;
            }
        }

        [RelayCommand]
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}