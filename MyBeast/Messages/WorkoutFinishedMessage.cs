using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyBeast.Messages
{
    // Objeto que leva os dados do treino
    public class WorkoutResult
    {
        public int Calories { get; set; }      
        public TimeSpan Duration { get; set; } 
    }

    public class WorkoutFinishedMessage : ValueChangedMessage<WorkoutResult>
    {
        public WorkoutFinishedMessage(WorkoutResult value) : base(value) { }
    }
}