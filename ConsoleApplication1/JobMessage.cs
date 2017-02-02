using System;

namespace ConsoleApplication1
{
    public abstract class JobMessage
    {
        public Guid JobId { get; set; }

        public int Priority { get; set; } = 1;
    }
}