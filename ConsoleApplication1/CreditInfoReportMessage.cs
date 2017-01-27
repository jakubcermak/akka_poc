using System;

namespace ConsoleApplication1
{
    public class CreditInfoReportMessage : JobMessage
    {
        public int CisId { get; set; }

        public int UserId { get; set; }

        public DateTime SnapshotDate { get; set; }
    }
}