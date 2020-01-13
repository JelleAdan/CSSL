using CSSL.Examples.ServerpoolsDispatcher;
using CSSL.Modeling;
using CSSL.Utilities.Distributions;
using System;

namespace SingleQueueDispatcherMultipleServerpools
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation sim = new Simulation("SomeSimulation");

            // The model part...

            ServerpoolsDispatcher serverpoolDispatcher = new ServerpoolsDispatcher(sim.MyModel, "ServerpoolDispatcher");

            double lambda = 100;
            JobGenerator jobGenerator = new JobGenerator(serverpoolDispatcher, "JobGenerator", new ExponentialDistribution(1 / lambda, 1 / lambda / lambda));
            serverpoolDispatcher.SetJobGenerator(jobGenerator);

            Dispatcher dispatcher = new Dispatcher(serverpoolDispatcher, "Dispatcher");
            serverpoolDispatcher.SetDispatcher(dispatcher);

            int numberServerpools = 10;
            for (int i = 0; i < numberServerpools; i++)
            {
                serverpoolDispatcher.AddServerpool(new Serverpool(serverpoolDispatcher, $"Serverpool_{i}"));
            }

            // The experiment part...

            sim.MyExperiment.LengthOfReplication = 100;
            sim.MyExperiment.NumberOfReplications = 3;



            Console.WriteLine("Test");
        }


    }
}
