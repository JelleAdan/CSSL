using CSSL.Examples.DataCenterSimulation;
using CSSL.Examples.DataCenterSimulation.DataCenterObservers;
using CSSL.Modeling;
using CSSL.Reporting;
using CSSL.Utilities.Distributions;
using System;
using System.IO;
using System.Linq;

namespace DataCenterSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation sim = new Simulation("DataCenterSimulation", @"C:\CSSL");

            // Parameters...

            double dispatchTime = 1E-3;
            double lambda = 100;
            int numberServerpools = 10;
            int numberServerpoolsToChooseFrom = 10;

            // The model part...

            DataCenter dataCenter = new DataCenter(sim.MyModel, "DataCenter");

            for (int i = 0; i < numberServerpools; i++)
            {
                dataCenter.AddServerpool(new ServerPool(dataCenter, $"Serverpool_{i}"));
            }

            Dispatcher dispatcher = new Dispatcher(dataCenter, "Dispatcher", new ExponentialDistribution(1), double.PositiveInfinity, dataCenter.ServerPools, dispatchTime, numberServerpoolsToChooseFrom);
            dataCenter.SetDispatcher(dispatcher);

            JobGenerator jobGenerator = new JobGenerator(dataCenter, "JobGenerator", new ExponentialDistribution(lambda), dispatcher);
            dataCenter.SetJobGenerator(jobGenerator);

            // The experiment part...

            sim.MyExperiment.NumberOfReplications = 3;
            sim.MyExperiment.LengthOfReplication = 20;
            sim.MyExperiment.LengthOfWarmUp = 2;

            // The observer part...

            DispatcherObserver dispatcherObserver = new DispatcherObserver(sim);
            dispatcherObserver.Subscribe(dispatcher);

            DataCenterObserver dataCenterObserver = new DataCenterObserver(sim);
            dataCenterObserver.Subscribe(dataCenter);

            foreach (ServerPool serverpool in dataCenter.ServerPools)
            {
                ServerPoolObserver serverpoolObserver = new ServerPoolObserver(sim);
                serverpoolObserver.Subscribe(serverpool);
            }

            // Run...

            sim.Run();

            // The reporting part...

            SimulationReporter reporter = sim.MakeSimulationReporter();

            reporter.PrintSummaryToFile();
            reporter.PrintSummaryToConsole();
        }
    }
}
