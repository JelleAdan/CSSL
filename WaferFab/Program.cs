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

namespace WaferFabSim
{
    public class Program
    {
        static void Main(string[] args)
        {
            string directory = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\";

            // Load data

            InputData inputData = new InputData(directory + "CSVs");

            // Build the model

            Simulation sim = new Simulation("WaferFab", directory + "Results");

            WaferFab waferFab = new WaferFab(sim.MyModel, "WaferFab");

            waferFab.LotStarts = inputData.LotStarts;

            waferFab.LotSteps = inputData.LotSteps;

            foreach(var wc in inputData.WorkCentersData)
            {
                WorkCenter workCenter = new WorkCenter(waferFab, $"WorkCenter_{wc.Key}", new ExponentialDistribution(wc.Value), inputData.LotStepsPerWorkStation[wc.Key]);

                workCenter.SetDispatcher(new BQFDispatcher(workCenter, workCenter.Name + "_BQFDispatcher"));

                waferFab.AddWorkCenter(workCenter.Name, workCenter);
            }

            //foreach(var sequence in inputData.SequencesData)
            //waferFab.AddSequence()

            //int stop = 0;
        }
    }
}
