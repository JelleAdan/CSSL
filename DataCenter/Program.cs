using CSSL.Examples.DataCenterSimulation;
using CSSL.Examples.DataCenterSimulation.DataCenterObservers;
using CSSL.Modeling;
using CSSL.Utilities.Distributions;
using System;

namespace DataCenterSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation sim = new Simulation("SomeSimulation");

            // The model part...

            DataCenter dataCenter = new DataCenter(sim.MyModel, "DataCenter");

            double lambda = 100;
            JobGenerator jobGenerator = new JobGenerator(dataCenter, "JobGenerator", new ExponentialDistribution(1 / lambda, 1 / lambda / lambda), dataCenter.Dispatcher);
            dataCenter.SetJobGenerator(jobGenerator);

            int numberServerpools = 10;
            for (int i = 0; i < numberServerpools; i++)
            {
                dataCenter.AddServerpool(new Serverpool(dataCenter, $"Serverpool_{i}"));
            }

            double dispatchTime = 1E-3;
            Dispatcher dispatcher = new Dispatcher(dataCenter, "Dispatcher", new ExponentialDistribution(1, 1), 2, dataCenter.ServerPools, dispatchTime);
            dataCenter.SetDispatcher(dispatcher);

            // Attach model to simulation...



            // The experiment part...

            sim.MyExperiment.LengthOfReplication = 100;
            sim.MyExperiment.NumberOfReplications = 3;

            // The observer part...
            DispatcherObserver dispatcherObserver = new DispatcherObserver();
            dispatcherObserver.Subscribe(dataCenter.Dispatcher);

            sim.Run();

            Console.WriteLine("Test");
        }
    }
}
