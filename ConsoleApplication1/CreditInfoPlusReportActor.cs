using System;
using System.Threading;
using Akka.Actor;
using Akka.Event;

namespace ConsoleApplication1
{
    public class CreditInfoPlusReportActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        private IActorRef statusActor;

        public CreditInfoPlusReportActor(IActorRef statusActor)
        {
            this.statusActor = statusActor;
            Receive<CreditInfoReportMessage>(m => ProcessMessage(m));
        }

        private void ProcessMessage(CreditInfoReportMessage m)
        {
            _log.Info("job " + m.JobId + " started");
            
            Thread.Sleep(10000);
            JobResultStorage.Instance.Save(m.JobId, new CreditInfoReport());
            _log.Info("job " + m.JobId + " finished");
            Sender.Tell(new JobCompletionReplyMessage { JobId = m.JobId, IsSuccess = true });
            statusActor.Tell(new StatusMessage { JobId = m.JobId, Completed = true });
        }
    }
}