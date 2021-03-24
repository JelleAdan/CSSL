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
        static void Main(string[] args)
        {
            RLLayer layer = new RLLayer();

            RLController controller = new RLController(layer);

            controller.Run();

            // 

            //RLLayer layer = new RLLayer();

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
