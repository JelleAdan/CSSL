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

            Settings.Output = true;
            Settings.FixSeed = true;

            ShellModel WaferFabSim = new ShellModel(inputDir, outputDir);

            // Load WaferFab settings
            //ManualDataReader reader = new ManualDataReader(inputDir + "CSVs");
            AutoDataReader reader = new AutoDataReader(inputDir + "Auto");

            WaferFabSettings waferFabSettings = reader.ReadWaferFabSettings();

            waferFabSettings.SampleInterval = 1 * 60 * 60; // seconds
            waferFabSettings.LotStartsFrequency = 1; // hours
            waferFabSettings.UseRealLotStartsFlag = true;

            // Read Initial Lots
            WaferFabSim.ReadRealSnaphots(inputDir + @"SerializedFiles\RealSnapshots_2019-12-1_2020-1-1_1h.dat");

            waferFabSettings.InitialRealLots = WaferFabSim.RealSnapshotReader.RealSnapshots.First().RealLots;

            // Experiment settings
            ExperimentSettings experimentSettings = new ExperimentSettings();

            experimentSettings.NumberOfReplications = 1;
            experimentSettings.LengthOfReplication = 1 * 24 * 60 * 60; // seconds
            experimentSettings.LengthOfWarmUp = 0 * 60 * 60;  // seconds

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
