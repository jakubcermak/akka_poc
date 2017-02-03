using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using ConsoleApplication1;

namespace Node
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = typeof(ReportProcessorFacade);

            var actorSystem = ActorSystem.Create("MySystem");

            Console.ReadLine();
        }
    }
}
