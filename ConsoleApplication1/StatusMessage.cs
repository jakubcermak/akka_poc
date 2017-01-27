using System;

namespace ConsoleApplication1
{
    public class StatusMessage
    {
        public bool Completed { get; set; }

        public DateTime? Started { get; set; }

        public Guid JobId { get; set; }
    }
}