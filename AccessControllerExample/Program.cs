using CSSL.Examples.AccessController;
using CSSL.Modeling;
using CSSL.RL;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccessControllerExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            RLLayer layer = new RLLayer();
            RLController controller = new RLController(layer);
            await controller.Run(cts.Token);

            //

            //Stopwatch stopwatch = new Stopwatch();
            //int reps = 100;
            //double totalDuration = 0;

            //Random rnd = new Random();
            //ServerPool sp = layer.ac.ServerPool;

            //for (int i = 0; i < reps; i++)
            //{
            //    stopwatch.Restart();

            //    layer.Reset();
            //    for (int customers = 0; customers < 6000; customers++)
            //    {
            //        if (sp.FreeServers > 0)
            //        {
            //            layer.Act(rnd.Next(2));
            //        }
            //        else
            //        {
            //            layer.Act(0);
            //        }
            //    }

            //    totalDuration += stopwatch.Elapsed.TotalSeconds;
            //}

            //Console.WriteLine($"Average duration: {totalDuration / reps} seconds.");
        }
    }
}
