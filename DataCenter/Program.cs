using CSSL.Examples.DataCenter;
using CSSL.Examples.DataCenter.DataCenterObservers;
using CSSL.Modeling;
using CSSL.Utilities.Distributions;
using System;

namespace DataCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation sim = new Simulation("SomeSimulation");

            // The model part...

            ServerpoolsDispatcher serverpoolDispatcher = new ServerpoolsDispatcher(sim.MyModel, "ServerpoolDispatcher");

            double lambda = 100;
            JobGenerator jobGenerator = new JobGenerator(serverpoolDispatcher, "JobGenerator", new ExponentialDistribution(1 / lambda, 1 / lambda / lambda), serverpoolDispatcher.Dispatcher);
            serverpoolDispatcher.SetJobGenerator(jobGenerator);

            int numberServerpools = 10;
            for (int i = 0; i < numberServerpools; i++)
            {
                serverpoolDispatcher.AddServerpool(new Serverpool(serverpoolDispatcher, $"Serverpool_{i}"));
            }

            double dispatchTime = 1E-3;
            Dispatcher dispatcher = new Dispatcher(serverpoolDispatcher, "Dispatcher", new ExponentialDistribution(1, 1), 2, serverpoolDispatcher.ServerPools, dispatchTime);
            serverpoolDispatcher.SetDispatcher(dispatcher);

            // The experiment part...

            sim.MyExperiment.LengthOfReplication = 100;
            sim.MyExperiment.NumberOfReplications = 3;

            // The observer part...
            DispatcherObserver dispatcherObserver = new DispatcherObserver();
            dispatcherObserver.Subscribe(serverpoolDispatcher.Dispatcher);



            Console.WriteLine("Test");
        }


    }
}
