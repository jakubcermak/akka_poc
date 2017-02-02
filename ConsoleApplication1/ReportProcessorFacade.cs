using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace ConsoleApplication1
{
    public class ReportProcessorFacade
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef coordinatorActor;
        private IActorRef statusActor;

        public ReportProcessorFacade()
        {
            actorSystem = ActorSystem.Create("MySystem");

            coordinatorActor = actorSystem.ActorOf<CoordinatorActor>("jobCoordinator");
        }

        public async Task<JobResult<TResult>> AddJob<TResult>(JobMessage message) where TResult : class
        {
            Guid id = Guid.NewGuid();
            message.JobId = id;
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