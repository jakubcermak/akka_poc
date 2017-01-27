namespace ConsoleApplication1
{
    public class InitCompletedMessage
    {
        public ActorRole Role { get; }

        public enum ActorRole
        {
            StatusActor
        };

        public InitCompletedMessage(ActorRole role)
        {
            Role = role;
        }
    }
}