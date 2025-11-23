using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Domain.Entities;
using MyBeast.Views.Workout; // Para referenciar rotas se necessário

namespace MyBeast.ViewModels.Workout
{
    public partial class WorkoutSummaryViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private WorkoutSession session;

        [ObservableProperty]
        private string motivationalMessage;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("Session"))
            {
                Session = query["Session"] as WorkoutSession;
                GenerateMessage();
            }
        }

        private void GenerateMessage()
        {
            if (Session.TotalVolume > 5000)
                MotivationalMessage = "Você é uma besta enjaulada! Treino insano!";
            else if (Session.TotalVolume > 2000)
                MotivationalMessage = "Ótimo trabalho! Continue focado.";
            else
                MotivationalMessage = "Treino concluído! O importante é a constância.";
        }

        [RelayCommand]
        public async Task GoHomeAsync()
        {
            // Volta para a raiz da navegação (provavelmente a lista ou a Home)
            await Shell.Current.GoToAsync("///MainPage");
            // Nota: Se sua lista de treinos for uma aba, ajuste a rota conforme seu AppShell
        }
    }
}