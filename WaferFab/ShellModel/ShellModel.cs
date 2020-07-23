using CSSL.Examples.WaferFab;
using CSSL.Examples.WaferFab.Dispatchers;
using CSSL.Examples.WaferFab.Observers;
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
        }

        private string inputDir { get; set; }
        private string outputDir{ get; set; }

        public Simulation MySimulation { get; private set; }

        public DataReaderBase DataImporter { get; set; }

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

        public void ReadRealSnaphots(string fileDirectory)
        {
            try
            {
                RealSnapshotReader = new RealSnapshotReader();
                RealSnapshotReader.Read(fileDirectory, 25);
            }
            catch
            {
                throw new Exception("Cannot read selected file");
            }

            NotifyOfPropertyChange(nameof(RealSnapshotReader));
        }

        private void buildWaferFab()
        {
            // Build the model
            WaferFab waferFab = new WaferFab(MySimulation.MyModel, "WaferFab", new ConstantDistribution(MyWaferFabSettings.SampleInterval));

            //// LotStarts
            waferFab.LotStarts = MyWaferFabSettings.LotStartQtys;

            //// LotSteps
            waferFab.LotSteps = MyWaferFabSettings.LotSteps;

            //// WorkCenters
            foreach (var wc in MyWaferFabSettings.WorkCenters)
            {
                WorkCenter workCenter = new WorkCenter(waferFab, $"WorkCenter_{wc}", MyWaferFabSettings.WorkCenterDistributions[wc], MyWaferFabSettings.LotStepsPerWorkStation[wc]);

                workCenter.SetDispatcher(new BQFDispatcher(workCenter, workCenter.Name + "_BQFDispatcher"));

                waferFab.AddWorkCenter(workCenter.Name, workCenter);
            }

            //// Sequences
            foreach (var sequence in MyWaferFabSettings.Sequences)
            {
                waferFab.AddSequence(sequence.Key, sequence.Value);
            }

            //// LotGenerator
            waferFab.SetLotGenerator(new LotGenerator(waferFab, "LotGenerator", new ConstantDistribution(MyWaferFabSettings.LotStartsFrequency * 60 * 60)));

            // Add intial lots
            if (MyWaferFabSettings.InitialLots.Any() != default)
            {
                waferFab.InitialLots = MyWaferFabSettings.InitialLots.Select(x => x.ConvertToLot(0, waferFab.Sequences)).Where(x => x != null).ToList();
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
