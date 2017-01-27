using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;

namespace ConsoleApplication1
{
    public class CoordinatorActor : ReceiveActor
    {
        private readonly IActorRef creditInfoReportActor;
        private IActorRef statusActor;
        private bool isReady=true;

        public CoordinatorActor()
        {
            statusActor = Context.ActorOf<StatusActor>("status");

            Props props = Props.Create<CreditInfoPlusReportActor>(()=>new CreditInfoPlusReportActor(statusActor))
                .WithRouter(new RoundRobinPool(10, new DefaultResizer(5, 1000)))
                .WithDispatcher("worker-dispatcher")
                ;
            creditInfoReportActor = Context.ActorOf(props);
       
            Receive<CreditInfoReportMessage>(msg =>
            {
                    statusActor.Tell(new StatusMessage { JobId = msg.JobId, Completed = false, Started = DateTime.Now });
                    creditInfoReportActor.Forward(msg);
            });

            Receive<InitCompletedMessage>(msg =>
            {
                isReady = true;
            });
        }
    }
}