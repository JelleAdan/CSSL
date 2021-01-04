using CSSL.Calendar;
using CSSL.Examples.WaferFab;
using CSSL.Examples.WaferFab.Dispatchers;
using CSSL.Examples.WaferFab.Observers;
using CSSL.Examples.WaferFab.Utilities;
using CSSL.Modeling;
using CSSL.Reporting;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WaferFabSim;
using WaferFabSim.InputDataConversion;

namespace WaferAreaSim
{
    public class Program
    {
        static void Main(string[] args)
        {

            #region Parameters
            string wc = "PHOTOLITH";

            string inputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\";

            string outputDir = @"C:\CSSLWaferFabArea\";
            #endregion

            #region Initializing simulation
            Simulation simulation = new Simulation("CSSLWaferFabArea", outputDir);
            #endregion

            #region Experimemt settings
            simulation.MyExperiment.NumberOfReplications = 1;
            simulation.MyExperiment.LengthOfReplication = 60 * 60 * 24 * 60;
            simulation.MyExperiment.LengthOfWarmUp = 60 * 60 * 24 * 30;
            DateTime intialDateTime = new DateTime(2019, 10, 30);
            #endregion

            #region Make starting lots
            AutoDataReader reader = new AutoDataReader(Path.Combine(inputDir, "Auto"), Path.Combine(inputDir, "SerializedFiles"));

            WaferFabSettings waferfabsettings = reader.ReadWaferFabSettings("WaferFabSettings_PHOTOLITH_WithLotStarts.dat");

            #endregion

            #region Building the model
            WaferFab waferFab = new WaferFab(simulation.MyModel, "WaferFab", new ConstantDistribution(60 * 60 * 24), intialDateTime);

            WorkCenter workCenter = new WorkCenter(waferFab, $"WorkCenter_{wc}", waferfabsettings.WCServiceTimeDistributions[wc], waferfabsettings.LotStepsPerWorkStation[wc]);

            // Connect workcenter to WIPDependentDistribution
            EPTDistribution distr = (EPTDistribution)waferfabsettings.WCServiceTimeDistributions[wc];

            distr.WorkCenter = workCenter;

            EPTOvertakingDispatcher dispatcher = new EPTOvertakingDispatcher(workCenter, workCenter.Name + "_EPTOvertakingDispatcher", waferfabsettings.WCOvertakingDistributions[wc]);

            workCenter.SetDispatcher(dispatcher);

            // Connect workcenter to OvertakingDistribution
            waferfabsettings.WCOvertakingDistributions[wc].WorkCenter = workCenter;

            waferFab.AddWorkCenter(workCenter.Name, workCenter);

            // Sequences
            foreach (var sequence in waferfabsettings.Sequences)
            {
                waferFab.AddSequence(sequence.Key, sequence.Value);
            }

            // LotSteps
            waferFab.LotSteps = waferFab.Sequences.Select(x => x.Value).Select(x => x.GetCurrentStep(0)).ToDictionary(x => x.Name);

            // LotGenerator
            waferFab.SetLotGenerator(new LotGenerator(waferFab, "LotGenerator", new ConstantDistribution(60), true));

            // Add lotstarts
            waferFab.LotStarts = waferfabsettings.LotStarts;

            // Add observers
            LotOutObserver lotOutObserver = new LotOutObserver(simulation, wc + "_LotOutObserver");
            dispatcher.Subscribe(lotOutObserver);

            WaferFabObserver waferFabObserver = new WaferFabObserver(simulation, "WaferFabObserver", waferFab);
            waferFab.Subscribe(waferFabObserver);

            TotalQueueObserver totalQueueObs = new TotalQueueObserver(simulation, wc + "_TotalQueueObserver");
            SeperateQueuesObserver seperateQueueObs = new SeperateQueuesObserver(simulation, workCenter, wc + "_SeperateQueuesObserver");

            workCenter.Subscribe(totalQueueObs);
            workCenter.Subscribe(seperateQueueObs);
            #endregion


            simulation.Run();

            #region Reporting
            SimulationReporter reporter = simulation.MakeSimulationReporter();

            reporter.PrintSummaryToConsole();
            #endregion

            int stop = 0;
        }
    }
}
