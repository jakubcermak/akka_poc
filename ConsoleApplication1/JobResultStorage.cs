using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class JobResultStorage
    {
        public static JobResultStorage Instance { get; } = new JobResultStorage();

        private readonly Dictionary<Guid, object> storage = new Dictionary<Guid, object>();

        public void Save(Guid id, object data) => storage.Add(id, data);

        public object Get(Guid id)
        {
            object value;
            return storage.TryGetValue(id, out value) ? value : null;
        }
    }
}