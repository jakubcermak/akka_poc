using System;
using System.Reflection;
using Akka.Actor;
using Akka.Persistence;
using Newtonsoft.Json;

namespace ConsoleApplication1.Persistence
{
    public class SqlPersistentRepresentation : IPersistentRepresentation
    {
        public IPersistentRepresentation WithPayload(object payload)
        {
            var clone = Clone();
            clone.Payload = payload;
            return clone;
        }

        public IPersistentRepresentation WithManifest(string manifest)
        {
            var clone = Clone();
            clone.Manifest = manifest;
            return clone;
        }

        public IPersistentRepresentation Update(long sequenceNr, string persistenceId, bool isDeleted, IActorRef sender, string writerGuid)
        {
            var clone = Clone();
            clone.SequenceNr = sequenceNr;
            clone.PersistenceId = persistenceId;
            clone.Sender = sender;
            clone.WriterGuid = writerGuid;
            clone.IsDeleted = isDeleted;
            return clone;
        }

        public object Payload { get; private set; }
        public string Manifest { get; private set; }
        public string PersistenceId { get; private set; }
        public long SequenceNr { get; private set; }
        public string WriterGuid { get; private set; }
        public bool IsDeleted { get; private set; } 
        public IActorRef Sender { get; private set; }

        public SqlPersistentRepresentation()
        {
            
        }

        public SqlPersistentRepresentation(AkkaStorageItem item, IActorContext context)
        {
            var payloadType = Type.GetType(item.PayloadType);
            Payload = JsonConvert.DeserializeObject(item.PayloadJson, payloadType);
            Manifest = item.Manifest;
            PersistenceId = item.PersistenceId;
            SequenceNr = item.SequenceNr;
            WriterGuid = item.WriterGuid;
            Sender = context.ActorSelection(item.Path)?.Anchor;
        }

        public SqlPersistentRepresentation Clone()
        {
            return new SqlPersistentRepresentation
            {
                Payload = this.Payload,
                Manifest = this.Manifest,
                PersistenceId = this.PersistenceId,
                SequenceNr = this.SequenceNr,
                WriterGuid = this.WriterGuid,
                Sender = this.Sender

            };
        }
    }
}