using System;

namespace ConsoleApplication1
{
    public class JobCompletionReplyMessage
    {
        public Guid JobId { get; set; }

        public bool IsSuccess { get; set; }
    }
}