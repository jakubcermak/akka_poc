using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AtomicWrite = Akka.Persistence.AtomicWrite;

namespace ConsoleApplication1.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AkkaStorageItem> AkkaStorageItems { get; set; }

        public ApplicationDbContext() : base("Default")
        {
            
        }
    }
}