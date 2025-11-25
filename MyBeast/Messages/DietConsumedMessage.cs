using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyBeast.Messages
{
    // Mensagem enviada quando o usuário consome uma refeição
    public class DietConsumedMessage : ValueChangedMessage<int>
    {
        public DietConsumedMessage(int calories) : base(calories)
        {
        }
    }
}