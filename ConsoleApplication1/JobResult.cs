using System;

namespace ConsoleApplication1
{
    public class JobResult<TResult>
        where TResult : class
    {
        public bool Completed { get; set; }

        public Guid JobId { get; set; }

        public TResult Result { get; set; }
    }
}