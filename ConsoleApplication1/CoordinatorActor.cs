using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Routing;

namespace ConsoleApplication1
{
    public class CoordinatorActor : ReceiveActor
    {
        private readonly IActorRef creditInfoReportActor;
        private bool isReady = true;
        private readonly IActorRef statusActor;

        public CoordinatorActor()
        {
            statusActor = Context.ActorOf<StatusActor>("status");

            Props props = Props.Create<CreditInfoPlusReportActor>(statusActor)
                .WithRouter(new RoundRobinPool(10))
                .WithMailbox("my-mailbox");

            creditInfoReportActor = Context.ActorOf(props, "router");

            Receive<CreditInfoReportMessage>(msg =>
            {
                // Console.WriteLine("coordinator ...");
                statusActor.Tell(new StatusMessage {JobId = msg.JobId, Completed = false, Started = DateTime.Now});
                creditInfoReportActor.Forward(msg);
            });

            Receive<InitCompletedMessage>(msg => { isReady = true; });
        }
    }
}