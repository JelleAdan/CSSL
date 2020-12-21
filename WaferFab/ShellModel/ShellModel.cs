using CSSL.Examples.WaferFab;
using CSSL.Examples.WaferFab.Dispatchers;
using CSSL.Examples.WaferFab.Observers;
using CSSL.Examples.WaferFab.Utilities;
using CSSL.Modeling;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using WaferFabSim.Import;
using WaferFabSim.InputDataConversion;
using WaferFabSim.OutputDataConversion;
using WaferFabSim.SnapshotData;

namespace WaferFabSim
{
    public class ShellModel : INotifyPropertyChanged
    {
        public ShellModel(string inputDir, string outputDir)
        {
            this.inputDir = inputDir;
            this.outputDir = outputDir;

            DataImporter = new DataImporter();
        }

        private string inputDir { get; set; }
        private string outputDir { get; set; }

        public Simulation MySimulation { get; private set; }

        public DataReaderBase DataReader { get; set; }

        public DataImporter DataImporter { get; set; }

        public WaferFabSettings MyWaferFabSettings { get; set; }

        public ExperimentSettings MyExperimentSettings { get; set; }

        public Results MySimResults { get; set; }

        public RealSnapshotReader RealSnapshotReader { get; set; }

        public void RunSimulation()
        {
            MySimulation = new Simulation("WaferFab", outputDir);

            buildWaferFab();

            setExperiment();

            MySimulation.Run();

            ReadSimulationResults();
        }

        public void ReadRealSnaphots(string filename)
        {
            try
            {
                RealSnapshotReader = new RealSnapshotReader();
                RealSnapshotReader.Read(filename, 1);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            NotifyOfPropertyChange(nameof(RealSnapshotReader));
        }

        private void buildWaferFab()
        {
            // Build the model
            WaferFab waferFab = new WaferFab(MySimulation.MyModel, "WaferFab", new ConstantDistribution(MyWaferFabSettings.SampleInterval), MyWaferFabSettings.InitialTime);

            //// LotStarts
            waferFab.ManualLotStarts = MyWaferFabSettings.ManualLotStartQtys;

            //// LotSteps
            waferFab.LotSteps = MyWaferFabSettings.LotSteps;

            //// WorkCenters
            foreach (string wc in MyWaferFabSettings.WorkCenters)
            {
                WorkCenter workCenter = new WorkCenter(waferFab, $"WorkCenter_{wc}", MyWaferFabSettings.WCServiceTimeDistributions[wc], MyWaferFabSettings.LotStepsPerWorkStation[wc]);

                // Connect workcenter to WIPDependentDistribution
                if (MyWaferFabSettings.WCServiceTimeDistributions[wc] is EPTDistribution)
                {
                    var distr = (EPTDistribution)MyWaferFabSettings.WCServiceTimeDistributions[wc];

                    distr.WorkCenter = workCenter;
                }

                if (MyWaferFabSettings.WCDispatchers[wc] == "BQF")
                {
                    workCenter.SetDispatcher(new BQFDispatcher(workCenter, workCenter.Name + "_BQFDispatcher"));
                }
                else if (MyWaferFabSettings.WCDispatchers[wc] == "EPTOvertaking")
                {
                    workCenter.SetDispatcher(new EPTOvertakingDispatcher(workCenter, workCenter.Name + "_EPTOvertakingDispatcher", MyWaferFabSettings.WCOvertakingDistributions[wc]));

                    // Connect workcenter to OvertakingDistribution
                    MyWaferFabSettings.WCOvertakingDistributions[wc].WorkCenter = workCenter;
                }
                else if (MyWaferFabSettings.WCDispatchers[wc] == "Random")
                {
                    workCenter.SetDispatcher(new RandomDispatcher(workCenter, workCenter.Name + "_RandomDispatcher"));
                }

                waferFab.AddWorkCenter(workCenter.Name, workCenter);
            }

            //// Sequences
            foreach (var sequence in MyWaferFabSettings.Sequences)
            {
                waferFab.AddSequence(sequence.Key, sequence.Value);
            }

            //// LotGenerator
            waferFab.SetLotGenerator(new LotGenerator(waferFab, "LotGenerator", new ConstantDistribution(MyWaferFabSettings.LotStartsFrequency * 60 * 60), MyWaferFabSettings.UseRealLotStartsFlag));

            // Add real LotStarts, copied from fab data
            if (MyWaferFabSettings.UseRealLotStartsFlag)
            {
                waferFab.LotStarts = MyWaferFabSettings.GetLotStarts();
            }

            // Add intial lots by translating RealLots (from RealSnapshot) to Lots
            if (MyWaferFabSettings.InitialRealLots.Any() != default)
            {
                waferFab.InitialLots = MyWaferFabSettings.InitialRealLots.Select(x => x.ConvertToLot(0, waferFab.Sequences, false)).Where(x => x != null).ToList();
            }

            // Add observers
            WaferFabObserver waferFabObserver = new WaferFabObserver(MySimulation, "WaferFabObserver", waferFab);
            waferFab.Subscribe(waferFabObserver);

            foreach (var wc in waferFab.WorkCenters)
            {
                TotalQueueObserver totalQueueObs = new TotalQueueObserver(MySimulation, wc.Key + "_TotalQueueObserver");
                SeperateQueuesObserver seperateQueueObs = new SeperateQueuesObserver(MySimulation, wc.Value, wc.Key + "_SeperateQueuesObserver");

                wc.Value.Subscribe(totalQueueObs);
                wc.Value.Subscribe(seperateQueueObs);
            }
        }

        private void setExperiment()
        {
            MySimulation.MyExperiment.NumberOfReplications = MyExperimentSettings.NumberOfReplications;
            MySimulation.MyExperiment.LengthOfWarmUp = MyExperimentSettings.LengthOfWarmUp;
            MySimulation.MyExperiment.LengthOfReplication = MyExperimentSettings.LengthOfReplication;
            MySimulation.MyExperiment.LengthOfReplicationWallClock = MyExperimentSettings.LengthOfReplicationWallClock;
        }

        public void ReadSimulationResults()
        {
            // Get to last experiment directory
            string experimentDir = Directory.GetDirectories(outputDir).OrderBy(x => Directory.GetCreationTime(x)).Last();

            MySimResults = new Results(experimentDir);

            MySimResults.ReadResults();

            NotifyOfPropertyChange(nameof(MySimResults));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
