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
            WaferFabSettings waferFabSettings = new WaferFabSettings(inputDir + "CSVs");

            waferFabSettings.sampleInterval = 10 * 60; // 10 minutes

            // Experiment settings
            sim.MyExperiment.NumberOfReplications = 3;
            sim.MyExperiment.LengthOfReplication = 2 * 24 * 60 * 60; // 2 = number of days
            sim.MyExperiment.LengthOfWarmUp = 8 * 60 * 60;

            Settings.Output = true;

            // Build the model and add observers
            sim = AddModelAndObservers(sim, waferFabSettings);

            // Run the simulation
            sim.Run();

            // Report summary
            SimulationReporter reporter = sim.MakeSimulationReporter();

            reporter.PrintSummaryToFile();
            reporter.PrintSummaryToConsole();

        }

        public static Simulation AddModelAndObservers(Simulation sim, WaferFabSettings inputData)
        {
            // Build the model
            WaferFab waferFab = new WaferFab(sim.MyModel, "WaferFab", new ConstantDistribution(inputData.sampleInterval));

            //// LotStarts
            waferFab.LotStarts = inputData.LotStartQtys;

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
            WaferFabObserver waferFabObserver = new WaferFabObserver(sim, "WaferFabObserver", waferFab);
            waferFab.Subscribe(waferFabObserver);

            foreach (var wc in waferFab.WorkCenters)
            {
                TotalQueueObserver totalQueueObs = new TotalQueueObserver(sim, wc.Key + "_TotalQueueObserver");
                SeperateQueuesObserver seperateQueueObs = new SeperateQueuesObserver(sim, wc.Value, wc.Key + "_SeperateQueuesObserver");

                wc.Value.Subscribe(totalQueueObs);
                wc.Value.Subscribe(seperateQueueObs);
            }

            return sim;
        }
    }
}
