using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Notifications
{
    public partial class NotificationsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Notification> _notifications; // Renomear para _notifications

        public NotificationsViewModel()
        {
            _notifications = new ObservableCollection<Notification>(); // Usar _notifications aqui
            LoadNotifications();
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        private void LoadNotifications()
        {
            // Dados de exemplo (serão substituídos por dados reais de um serviço)
            _notifications.Add(new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Conquista",
                Title = "Conquista Desbloqueada!",
                Message = "Você completou 10 treinos de perna.",
                Timestamp = DateTime.Now.AddHours(-1),
                Icon = "trophy_icon.png"
            });
            _notifications.Add(new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Comentário",
                Title = "Novo comentário no seu post",
                Message = "'Ótimo treino!' por João.",
                Timestamp = DateTime.Now.AddHours(-2),
                Icon = "comment_icon.png"
            });
            _notifications.Add(new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Treino",
                Title = "Lembrete de Treino",
                Message = "Seu treino de peito está agendado para hoje.",
                Timestamp = DateTime.Now.AddDays(-1),
                Icon = "dumbbell_icon.png"
            });
            _notifications.Add(new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Type = "Dieta",
                Title = "Alerta de Dieta",
                Message = "Não se esqueça da sua refeição pós-treino.",
                Timestamp = DateTime.Now.AddDays(-2),
                Icon = "food_icon.png"
            });
        }
    }
}
