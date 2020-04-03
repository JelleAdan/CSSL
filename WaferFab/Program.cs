using System;
using CSSL.Examples.DataCenterSimulation;
using CSSL.Examples.DataCenterSimulation.DataCenterObservers;
using CSSL.Modeling;
using CSSL.Reporting;
using CSSL.Utilities.Distributions;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CSSL.Examples.WaferFab;
using WaferFabSim.InputDataConversion;
using CSSL.Examples.WaferFab.Dispatchers;
using CSSL.Examples.WaferFab.Observers;

namespace WaferFabSim
{
    public class Program
    {
        static void Main(string[] args)
        {
            string inputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\";

            string outputDir = @"C:\CSSLWaferFab\";

            Simulation sim = new Simulation("WaferFab", outputDir);

            // Load data
            InputData inputData = new InputData(inputDir + "CSVs");

            // Experiment settings
            sim.MyExperiment.NumberOfReplications = 3;
            sim.MyExperiment.LengthOfReplication = 1 * 24 * 60 * 60; // 1 = number of days
            sim.MyExperiment.LengthOfWarmUp = 2;

            // Build the model
            WaferFab waferFab = new WaferFab(sim.MyModel, "WaferFab");

            //// LotStarts
            waferFab.LotStarts = inputData.LotStarts;

            //// LotSteps
            waferFab.LotSteps = inputData.LotSteps;

            //// WorkCenters
            foreach (var wc in inputData.WorkCentersData)
            {
                WorkCenter workCenter = new WorkCenter(waferFab, $"WorkCenter_{wc.Key}", new ExponentialDistribution(wc.Value), inputData.LotStepsPerWorkStation[wc.Key]);

                workCenter.SetDispatcher(new BQFDispatcher(workCenter, workCenter.Name + "_BQFDispatcher"));

                waferFab.AddWorkCenter(workCenter.Name, workCenter);
            }

            //// Sequences
            foreach (var sequence in inputData.SequencesData)
            {
                waferFab.AddSequence(sequence.Key, new Sequence(sequence.Key, sequence.Value));
            }

            //// LotGenerator
            waferFab.SetLotGenerator(new LotGenerator(waferFab, "LotGenerator", new ConstantDistribution(inputData.LotStartsFrequency * 60 * 60)));

            // Add observers
            foreach (var wc in waferFab.WorkCenters)
            {
                TotalQueueObserver totalQueueObs = new TotalQueueObserver(sim, wc.Key + "_TotalQueueObserver");
                SeperateQueuesObserver seperateQueueObs = new SeperateQueuesObserver(sim, wc.Value, wc.Key + "_SeperateQueuesObserver");

                wc.Value.Subscribe(totalQueueObs);
                wc.Value.Subscribe(seperateQueueObs);
            }

            // Run the simulation
            sim.Run();

            // Report summary
            SimulationReporter reporter = sim.MakeSimulationReporter();

            reporter.PrintSummaryToFile();
            reporter.PrintSummaryToConsole();

        }
    }

    internal interface Test
    {

    }
}
