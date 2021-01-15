using CSSL.Examples.AccessController;
using CSSL.Modeling;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace AccessControllerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("testmap", 10000))
            {
                bool mutexCreated;
                Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write("Hello Python");
                }
                mutex.ReleaseMutex();

                Console.WriteLine("Start Process B and press ENTER to continue.");
                Console.ReadLine();

                Console.WriteLine("Start Process C and press ENTER to continue.");
                Console.ReadLine();

                mutex.WaitOne();
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    Console.WriteLine("Process A says: {0}", reader.ReadBoolean());
                    Console.WriteLine("Process B says: {0}", reader.ReadBoolean());
                    Console.WriteLine("Process C says: {0}", reader.ReadBoolean());
                }
                mutex.ReleaseMutex();
            }

            //RLLayer layer = new RLLayer();
            //layer.BuildTrainingEnvironment();
            //Random rnd = new Random();
            //ServerPool sp = layer.ac.ServerPool;
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //layer.Reset();
            //for (int i = 0; i < 6000; i++)
            //{
            //    if (sp.FreeServers > 0)
            //    {
            //        layer.Act(rnd.Next(2));
            //    }
            //    else
            //    {
            //        layer.Act(0);
            //    }
            //}
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
