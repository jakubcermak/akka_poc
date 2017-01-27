using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication1.Persistence
{
    public class AkkaStorageItem
    {
        public int Id { get; set; }

        [Index()]
        [StringLength(300)]
        public string PersistenceId { get; set; }
        public long SequenceNr { get; set; }
        public string Manifest { get; set; }

        public string PayloadJson { get; set; }
        public string Path { get; set; }
        public string WriterGuid { get; set; }

        public bool IsDeleted { get; set; }
        [StringLength(300)]
        public string PayloadType { get; set; }
    }
}