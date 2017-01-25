using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var xx = Enumerable.Range(0, 1000).Select(x => new Random().Next(100000)).ToList();

            Task.Run(async () =>
            {
                var facade = new ReportProcessorFacade();
                Random rnd = new Random();
                var tasks = Enumerable.Range(0, 300).Select(async i =>
                {
                    Thread.Sleep(rnd.Next(100));
                    var r = await facade.AddJob<CreditInfoReport>(new CreditInfoReportMessage());
                    //Console.WriteLine($"job {r.JobId} result - {r.Completed}");
                }).ToList();
                await Task.WhenAll(tasks);
                Console.WriteLine("all WS call completed by OK or timeout");
            }).Wait();

            Console.ReadLine();
        }
    }

    public class ReportProcessorFacade
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef coordinatorActor;

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
                JobCompletionReplyMessage result = await coordinatorActor.Ask<JobCompletionReplyMessage>(message, TimeSpan.FromSeconds(5));
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

    public class JobCompletionReplyMessage
    {
        public Guid JobId { get; set; }

        public bool IsSuccess { get; set; }
    }

    public abstract class JobMessage
    {
        public Guid JobId { get; set; }
    }

    public class CreditInfoReportMessage : JobMessage
    {
        public int CisId { get; set; }

        public int UserId { get; set; }

        public DateTime SnapshotDate { get; set; }
    }

    public class JobResult<TResult>
        where TResult : class
    {
        public bool Completed { get; set; }

        public Guid JobId { get; set; }

        public TResult Result { get; set; }
    }

    public class CreditInfoReport
    {
    }

    public class CoordinatorActor : ReceiveActor
    {
        private readonly IActorRef creditInfoReportActor;

        public CoordinatorActor()
        {
            Props props = Props.Create<CreditInfoPlusReportActor>()
                .WithRouter(new RoundRobinPool(10, new DefaultResizer(5, 1000)));
            creditInfoReportActor = Context.ActorOf(props);

            Receive<CreditInfoReportMessage>(msg => { creditInfoReportActor.Forward(msg); });
        }
    }

    public class CreditInfoPlusReportActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public CreditInfoPlusReportActor()
        {
            Receive<CreditInfoReportMessage>(m => ProcessMessage(m));
        }

        private void ProcessMessage(CreditInfoReportMessage msg)
        {
            _log.Info("job " + msg.JobId + " started");
            Thread.Sleep(10000);

            JobResultStorage.Instance.Save(msg.JobId, new CreditInfoReport());
            _log.Info("job " + msg.JobId + " finished");
            Sender.Tell(new JobCompletionReplyMessage { JobId = msg.JobId, IsSuccess = true});
        }
    }

    public class JobResultStorage
    {
        public static JobResultStorage Instance { get; } = new JobResultStorage();

        private readonly Dictionary<Guid, object> storage = new Dictionary<Guid, object>();

        public void Save(Guid id, object data) => storage.Add(id, data);

        public object Get(Guid id)
        {
            object value;
            return storage.TryGetValue(id, out value) ? value : null;
        }
    }
}