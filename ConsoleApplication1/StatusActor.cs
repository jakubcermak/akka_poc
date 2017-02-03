using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Akka.Event;
using Akka.Persistence;

namespace ConsoleApplication1
{
    public class StatusActor : ReceivePersistentActor
    {
        public ConcurrentDictionary<Guid, StatusMessage> statuses = new ConcurrentDictionary<Guid, StatusMessage>();
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public StatusActor()
        {
            Command<StatusMessage>(message =>
            {
                Persist(message, ProcessStatusMessage);
            });

            Command<RecoveryCompleted>(message =>
            {
            });

            Recover<StatusMessage>(message =>
            {
                ProcessStatusMessage(message);
            });
        }

        private void ProcessStatusMessage(StatusMessage m)
        {
            StatusMessage existing;
            statuses.AddOrUpdate(m.JobId, m, (id, old) =>
            {
                _log.Warning($"A job {m.JobId} changed status to {m.Completed}");
                old.Completed = m.Completed;
                return old;
            });
            //if (statuses.TryGetValue(message.JobId, out existing))
            //{
            //    m.Completed = message.Completed;
            //}
            //else
            //{
            //    _log.Warning($"A new job {m.JobId} with status {m.Completed}");
            //    statuses[message.JobId] = m;
            //}
        }

        //protected override void Unhandled(object message)
        //{
        //    base.Unhandled(message);

        //    if (message is RecoveryCompleted)
        //    {
        //        Context.ActorSelection("/usr/jobCoordinator").Tell(new InitCompletedMessage(InitCompletedMessage.ActorRole.StatusActor));
        //    }
        //}

        
        public override string PersistenceId => "StatusActor";
    }
}