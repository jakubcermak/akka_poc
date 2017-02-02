using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;

namespace ConsoleApplication1
{
    public class FoxyMailbox : UnboundedPriorityMailbox
    {
        public FoxyMailbox(Settings settings, Config config) : base(settings, config)
        {
        }

        protected override int PriorityGenerator(object message)
        {
            return (message as JobMessage)?.Priority ?? -1;
        }
    }
}