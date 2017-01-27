using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using Akka.Persistence.Journal;
using Newtonsoft.Json;

namespace ConsoleApplication1.Persistence
{
    public class SqlJournal : AsyncWriteJournal
    {
        public override async Task ReplayMessagesAsync(IActorContext context, string persistenceId, long fromSequenceNr, long toSequenceNr, long max, Action<IPersistentRepresentation> recoveryCallback)
        {
            using (var dc = new ApplicationDbContext())
            {
                var items = await dc.AkkaStorageItems.Where(x => x.PersistenceId == persistenceId && x.SequenceNr >= fromSequenceNr && x.SequenceNr <= toSequenceNr).ToListAsync();
                foreach (var item in items)
                {
                    var presistentRepresentation = new SqlPersistentRepresentation(item, context);
                    recoveryCallback(presistentRepresentation);
                }
            }
        }

        public override Task<long> ReadHighestSequenceNrAsync(string persistenceId, long fromSequenceNr)
        {
            using (var dc = new ApplicationDbContext())
            {
                var result = dc.AkkaStorageItems.Where(x => x.PersistenceId == persistenceId && x.SequenceNr >= fromSequenceNr).OrderByDescending(x => x.SequenceNr).FirstOrDefault()?.SequenceNr;
                return Task.FromResult(result ?? 0);
            }
        }

        protected override async Task<IImmutableList<Exception>> WriteMessagesAsync(IEnumerable<Akka.Persistence.AtomicWrite> messages)
        {
            using (var dc = new ApplicationDbContext())
            {
                foreach (Akka.Persistence.AtomicWrite atomicWrite in messages)
                {
                    foreach (var message in (IEnumerable<IPersistentRepresentation>) atomicWrite.Payload)
                    {
                        var item = new AkkaStorageItem
                        {
                            PayloadJson = JsonConvert.SerializeObject(message.Payload),
                            PersistenceId = message.PersistenceId,
                            SequenceNr = message.SequenceNr,
                            Manifest = message.Manifest,
                            Path = message.Sender?.Path?.ToString(),
                            WriterGuid = message.WriterGuid,
                            IsDeleted = message.IsDeleted,
                            PayloadType = message.Payload.GetType().AssemblyQualifiedName
                        };
                        dc.AkkaStorageItems.Add(item);
                    }
                    await dc.SaveChangesAsync();
                }
            }
            return null;
        }

        protected override Task DeleteMessagesToAsync(string persistenceId, long toSequenceNr)
        {
            using (var dc = new ApplicationDbContext())
            {
                var item = dc.AkkaStorageItems.FirstOrDefault(x => x.PersistenceId == persistenceId && toSequenceNr == x.SequenceNr);
                if (item != null)
                {
                    dc.AkkaStorageItems.Remove(item);
                    dc.SaveChanges();

                }
                return Task.CompletedTask;
            }
        }
    }
}