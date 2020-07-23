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
using System.Threading.Tasks;
using WaferFabSim.SnapshotData;
using Microsoft.VisualBasic.CompilerServices;

namespace WaferFabSim
{
    public class Program
    {
        static void Main(string[] args)
        {
            string inputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\";

            string outputDir = @"C:\CSSLWaferFab\";

            ShellModel WaferFabSim = new ShellModel(inputDir, outputDir);

            // Load WaferFab settings
            ManualDataReader reader = new ManualDataReader(inputDir + "CSVs");

            WaferFabSettings waferFabSettings = reader.ReadWaferFabSettings();

            waferFabSettings.SampleInterval = 12 * 60 * 60; // 12 hours

            // Experiment settings
            ExperimentSettings experimentSettings = new ExperimentSettings();

            experimentSettings.NumberOfReplications = 3;
            experimentSettings.LengthOfReplication = 20 * 24 * 60 * 60; // 2 = number of days
            experimentSettings.LengthOfWarmUp = 8 * 60 * 60;

            Settings.Output = true;

            // Connect settings
            WaferFabSim.MyWaferFabSettings = waferFabSettings;

            WaferFabSim.MyExperimentSettings = experimentSettings;

            // Run simulation
            WaferFabSim.RunSimulation();

            // Report summary
            SimulationReporter reporter = WaferFabSim.MySimulation.MakeSimulationReporter();

            reporter.PrintSummaryToFile();
            reporter.PrintSummaryToConsole();

        }

    }
}
