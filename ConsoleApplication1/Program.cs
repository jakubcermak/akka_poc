using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var xx = Enumerable.Range(0, 1000).Select(x => new Random().Next(100000)).ToList();

            Task.Run(async () =>
            {
                var facade = new ReportProcessorFacade();
                Console.ReadLine();
                Random rnd = new Random();
                var tasks = Enumerable.Range(0, 300).Select(async i =>
                {
                    Thread.Sleep(rnd.Next(100));
                    var r = await facade.AddJob<CreditInfoReport>(new CreditInfoReportMessage());
                    //Console.WriteLine($"job {r.JobId} result - {r.Completed}");
                }).ToList();
                await Task.WhenAll(tasks);
                Console.WriteLine("all WS call completed by OK or timeout");
            }).Wait();

            Console.ReadLine();
        }
    }
}