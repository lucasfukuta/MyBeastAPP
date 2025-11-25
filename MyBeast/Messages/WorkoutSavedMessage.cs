using CommunityToolkit.Mvvm.Messaging.Messages;
using MyBeast.ViewModels.Workout; 

namespace MyBeast.Messages
{
    // Essa mensagem carrega um objeto do tipo WorkoutItemViewModel
    public class WorkoutSavedMessage : ValueChangedMessage<WorkoutItemViewModel>
    {
        public WorkoutSavedMessage(WorkoutItemViewModel value) : base(value)
        {
        }
    }
}