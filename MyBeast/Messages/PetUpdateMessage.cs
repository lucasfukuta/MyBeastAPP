using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyBeast.Messages
{
    // Enum para saber o tipo de ação
    public enum PetActionType
    {
        WorkoutFinished, 
        MealFinished     
    }

    // A mensagem carrega o tipo de ação
    public class PetUpdateMessage : ValueChangedMessage<PetActionType>
    {
        public PetUpdateMessage(PetActionType value) : base(value)
        {
        }
    }
}