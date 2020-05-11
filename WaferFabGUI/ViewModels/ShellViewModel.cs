using Caliburn.Micro;
using CSSL.Examples.WaferFab;
using CSSL.Modeling;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml.Serialization;
using WaferFabGUI.Models;
using WaferFabSim;
using WaferFabSim.InputDataConversion;
using Action = System.Action;

namespace WaferFabGUI.ViewModels
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        public ShellViewModel()
        {
            // Initialize properties
            WorkCenters = new ObservableCollection<WorkCenterData>();
            LotStartQtys = new ObservableCollection<LotStartQty>();
            WIPBarChart = new PlotModel { Title = "WIP balance" };
            inputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\";
            outputDir = @"C:\CSSLWaferFab\";
            sim = new Simulation("WaferFab", outputDir);
            dialogCoordinator = DialogCoordinator.Instance;

            // Link relay commands
            RunSimulationCommand = new RelayCommand(RunSimulation, () => true);

            // Load data
            waferFabSettings = new WaferFabSettings(inputDir + "CSVs");

            // Initialize settings
            NumberOfReplications = 3;
            LengthOfReplication = 2 * 24;   // hours
            LengthOfWarmUp = 8;             // hours
            SampleInterval = 10;            // minutes
            Settings.Output = true;
            initializeSettings();

            // Bar chart
            IEnumerable<string> axisLabels = new List<string>() { "OD Photo", "OD Etch", "OD Test" };

            // A ColumnSeries requires a CategoryAxis on the x-axis.

            WIPData = readWIPSnapshots(outputDir);

            WIPBarChart.Axes.Add(new OxyPlot.Axes.CategoryAxis { ItemsSource = WIPData.First().LotSteps, Angle = 60 });

            var series = new ColumnSeries();

            foreach (var wip in WIPData.Last().WIPlevels)
            {
                series.Items.Add(new ColumnItem(wip));
            }

            WIPBarChart.Series.Add(series);
        }

        private Simulation sim;
        private WaferFabSettings waferFabSettings;
        private string inputDir;
        private string outputDir;
        private bool _isMulitpleSnapshots;
        private int _selectedReplication;
        private ObservableCollection<WorkCenterData> _workCenters;
        private ObservableCollection<LotStartQty> _lotStartQtys;
        private ObservableCollection<WIPSnapshot> _wipSnapshots;
        private PlotModel _wipBarChart;
        private WIPSnapshot _wipDataSelected;
        private IDialogCoordinator dialogCoordinator;


        public async void RunSimulation()
        {
            // To do: build await
            ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Allocating wafers", "Running algorithm...");
            controller.SetIndeterminate();

            // Build model
            updateSettings();
            sim = Program.AddModelAndObservers(sim, waferFabSettings);

            // Run simulation
            sim.Run();

            // Read WIPSnapshots
            WIPData = readWIPSnapshots(outputDir);

            await controller.CloseAsync();
        }
        public void ClearLastWIPData()
        {
            WIPBarChart.Series.RemoveAt(WIPBarChart.Series.Count);
            WIPBarChart.InvalidatePlot(true);
        }
        public void ClearAllWIPData()
        {
            WIPBarChart.Series.RemoveAt(WIPBarChart.Series.Count);
            WIPBarChart.InvalidatePlot(true);
        }
        private void initializeSettings()
        {
            // Initialize workcenters
            foreach (var wc in waferFabSettings.WorkCentersData)
            {
                WorkCenters.Add(new WorkCenterData(wc.Key, 1 / wc.Value));
            }
            // Initialize lotstarts
            foreach (var lotStart in waferFabSettings.LotStartQtys)
            {
                LotStartQtys.Add(new LotStartQty(lotStart.Key, lotStart.Value));
            }
        }
        private void updateSettings()
        {
            foreach (var wc in WorkCenters)
            {
                waferFabSettings.WorkCentersData[wc.Name] = wc.ExponentialRate;
            }
            foreach (var lotStart in LotStartQtys)
            {
                waferFabSettings.LotStartQtys[lotStart.LotType] = lotStart.Quantity;
            }
        }
        private void plotSelectedWIPData()
        {
            if (!IsMultipleSnapshots)
            {
                WIPBarChart.Series.Clear();
            }

            var series = new ColumnSeries();

            foreach (var wip in WIPDataSelected.WIPlevels)
            {
                series.Items.Add(new ColumnItem(wip));
            }

            WIPBarChart.Series.Add(series);

            WIPBarChart.InvalidatePlot(true);
        }


        public ObservableCollection<WIPSnapshot> WIPData
        {
            get { return _wipSnapshots; }
            set
            {
                _wipSnapshots = value;
                NotifyOfPropertyChange();
            }
        }
        public ObservableCollection<int> AllReplications
        {
            get
            {
                var allReps = new ObservableCollection<int>();

                for (int i = 0; i < NumberOfReplications; i++)
                {
                    allReps.Add(i + 1);
                }
                return allReps;
            }
        }
        public List<WIPSnapshot> WIPDataPerReplication
        {
            get { return WIPData.Where(x => x.Replication == SelectedReplication).ToList(); }
        }
        public ObservableCollection<WorkCenterData> WorkCenters
        {
            get
            {
                return _workCenters;
            }
            set
            {
                _workCenters = value;
                NotifyOfPropertyChange();
            }
        }
        public ObservableCollection<LotStartQty> LotStartQtys
        {
            get
            {
                return _lotStartQtys;
            }
            set
            {
                _lotStartQtys = value;
                NotifyOfPropertyChange();
            }
        }

        public WIPSnapshot WIPDataSelected
        {
            get { return _wipDataSelected; }
            set
            {
                _wipDataSelected = value;
                NotifyOfPropertyChange();
                plotSelectedWIPData();
            }
        }

        public PlotModel WIPBarChart
        {
            get { return _wipBarChart; }
            set { _wipBarChart = value; }
        }
        public int NumberOfReplications
        {
            get
            {
                return sim.MyExperiment.NumberOfReplications;
            }
            set
            {
                sim.MyExperiment.NumberOfReplications = value;
                NotifyOfPropertyChange();
            }
        }
        public int LotStartFrequency
        {
            get { return waferFabSettings.LotStartsFrequency; }
            set
            {
                waferFabSettings.LotStartsFrequency = value;
                NotifyOfPropertyChange();
            }
        }
        public int SelectedReplication
        {
            get { return _selectedReplication; }
            set
            {
                _selectedReplication = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(WIPDataPerReplication));
            }
        }
        public double LengthOfReplication   // In hours
        {
            get
            {
                return sim.MyExperiment.LengthOfReplication / (60 * 60);
            }
            set
            {
                sim.MyExperiment.LengthOfReplication = value * 60 * 60;
                NotifyOfPropertyChange();
            }
        }
        public double LengthOfReplicationWallClock
        {
            get
            {
                return sim.MyExperiment.LengthOfReplicationWallClock >= double.MaxValue ? -1 : sim.MyExperiment.LengthOfReplicationWallClock;
            }
            set
            {
                if (value == -1)
                {
                    sim.MyExperiment.LengthOfReplicationWallClock = double.MaxValue;
                }
                else
                {
                    sim.MyExperiment.LengthOfReplicationWallClock = value;
                }
                NotifyOfPropertyChange();
            }
        }
        public double LengthOfWarmUp        // In hours
        {
            get
            {
                return sim.MyExperiment.LengthOfWarmUp / (60 * 60);
            }
            set
            {
                sim.MyExperiment.LengthOfWarmUp = value * 60 * 60;
                NotifyOfPropertyChange();
            }
        }
        public double SampleInterval        // In minutes
        {
            get
            {
                return waferFabSettings.sampleInterval / 60;
            }
            set
            {
                waferFabSettings.sampleInterval = value * 60;
                NotifyOfPropertyChange();
            }
        }
        public bool IsMultipleSnapshots
        {
            get { return _isMulitpleSnapshots; }
            set
            {
                _isMulitpleSnapshots = value;
                NotifyOfPropertyChange();
            }
        }

        public RelayCommand RunSimulationCommand { get; }


        private ObservableCollection<WIPSnapshot> readWIPSnapshots(string outputDir)
        {
            ObservableCollection<WIPSnapshot> wipSnapshots = new ObservableCollection<WIPSnapshot>();

            // Get to experiment directory
            string experimentDir = Directory.GetDirectories(outputDir).OrderBy(x => Directory.GetCreationTime(x)).Last();

            // Get to replication directories
            foreach (string replicationDir in Directory.GetDirectories(experimentDir))
            {
                var test = replicationDir.ToCharArray();

                int replicationNumber = Convert.ToInt32(replicationDir.Substring(replicationDir.Length - 1));

                // Get to waferFabObserver
                var waferFabObserverDir = Directory.GetFiles(replicationDir).Where(x => x.Contains("WaferFabObserver")).First();

                // Read data
                using (var reader = new StreamReader(waferFabObserverDir))
                {
                    string header = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string data = reader.ReadLine();

                        wipSnapshots.Add(new WIPSnapshot(replicationNumber, header, data));
                    }
                }
            }
            return wipSnapshots;
        }

        private void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public class RelayCommand : ICommand
        {
            private Func<bool> _canExecute;
            private Action _execute;

            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                if (execute == null)
                {
                    throw new ArgumentNullException(nameof(execute));
                }
                _execute = execute;
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return _canExecute != null ? _canExecute() : true;
            }

            public void Execute(object parameter)
            {
                _execute();
            }

            public void NotifyOfCanExecuteChange()
            {
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}
