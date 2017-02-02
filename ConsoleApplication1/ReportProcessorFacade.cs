using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Routing;

namespace ConsoleApplication1
{
    public class ReportProcessorFacade
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef coordinatorActor;
        private IActorRef statusActor;

        ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());

        public ReportProcessorFacade()
        {
            actorSystem = ActorSystem.Create("MySystem");

            coordinatorActor = actorSystem.ActorOf(Props.Create<CoordinatorActor>().WithRouter(new ClusterRouterPool(new RandomPool(10), new ClusterRouterPoolSettings(10,1, false))), "jobCoordinator");
        }

        public async Task<JobResult<TResult>> AddJob<TResult>(JobMessage message) where TResult : class
        {
            Guid id = Guid.NewGuid();
            message.JobId = id;
            message.Priority = random.Value.Next(10);
            try
            {
                JobCompletionReplyMessage result = await coordinatorActor.Ask<JobCompletionReplyMessage>(message, TimeSpan.FromSeconds(5000));
                return new JobResult<TResult>
                {
                    Result = (TResult) JobResultStorage.Instance.Get(id),
                    Completed = true,
                    JobId = id
                };
            }
            catch (TaskCanceledException)
            {
                return new JobResult<TResult>
                {
                    Completed = false,
                    JobId = id
                };
            }
        }
    }
}