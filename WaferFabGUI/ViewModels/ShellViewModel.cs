using Caliburn.Micro;
using CSSL.Examples.WaferFab;
using CSSL.Modeling;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
using WaferFabSim.SnapshotData;
using Action = System.Action;

namespace WaferFabGUI.ViewModels
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        public ShellViewModel()
        {
            // Link relay commands
            RunSimulationCommand = new RelayCommand(RunSimulationAsync, () => true);
            ClearAllWIPDataCommand = new RelayCommand(ClearAllWIPData, () => CanClearWIPData);
            ClearLastWIPDataCommand = new RelayCommand(ClearLastWIPData, () => CanClearWIPData);
            PlayAnimationCommand = new RelayCommand(PlayAnimationAsync, () => (SnapshotsToAnimate != null && SnapshotsToAnimate.Any() && !isAnimationRunning) || AnimationPaused);
            PauseAnimationCommand = new RelayCommand(PauseAnimation, () => isAnimationRunning && !AnimationPaused);
            StopAnimationCommand = new RelayCommand(StopAnimation, () => isAnimationRunning);
            LoadRealSnapshotsCommand = new RelayCommand(LoadRealSnapshotsAsync, () => true);

            // Initialize properties
            WorkCenters = new ObservableCollection<WorkCenterData>();
            LotStartQtys = new ObservableCollection<LotStartQty>();
            WIPBarChart = new PlotModel { Title = "WIP balance" };
            dialogCoordinator = DialogCoordinator.Instance;
            inputDirectory = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\";
            FPSAnimation = 5;
            FPSMax = 5;


            // Initialize Experiment Settings
            experimentSettings = new ExperimentSettings();
            experimentSettings.OutputDirectory = @"C:\CSSLWaferFab\";
            experimentSettings.NumberOfReplications = 1;
            experimentSettings.LengthOfReplication = 100 * 24 * 60 * 60;
            experimentSettings.LengthOfWarmUp = 12 * 60 * 60;
            Settings.Output = true;

            // Initialize Waferfab Settings
            waferFabSettings = new WaferFabSettings(inputDirectory + "CSVs");
            initializeWaferFabSettings(waferFabSettings);

            // Initialize bar chart
            SimulationSnapshots = readWIPSnapshots(experimentSettings.OutputDirectory);
            xAxisLotSteps = SimulationSnapshots.First().LotSteps;
            xAxis = new CategoryAxis() { Angle = 60, ItemsSource = xAxisLotSteps };
            WIPBarChart.Axes.Add(xAxis);
            yAxis = new LinearAxis() { Minimum = 0 };
            WIPBarChart.Axes.Add(yAxis);
        }

        private Stopwatch stopwatch = new Stopwatch();
        private LinearAxis yAxis;
        private CategoryAxis xAxis;
        private string[] xAxisLotSteps;
        private Simulation sim;
        private ExperimentSettings experimentSettings;
        private WaferFabSettings waferFabSettings;
        private string inputDirectory;
        private bool isAnimationRunning;
        private bool _isRealData;
        private bool _isMultipleSnapshots;
        private bool _isStartStateSelected;
        private int _selectedReplication;
        private int animationCounter;
        private ObservableCollection<WorkCenterData> _workCenters;
        private ObservableCollection<LotStartQty> _lotStartQtys;
        private ObservableCollection<SimulationSnapshot> _simulationSnapshots;
        private ObservableCollection<RealSnapshot> _realSnapshots;
        private PlotModel _wipBarChart;
        private WIPSnapshotBase _snapshotSelected;
        private RealSnapshot _startState;
        private IDialogCoordinator dialogCoordinator;
        private RealSnapshotReader realSnapshotReader;

        public bool AnimationPaused = false;
        public bool AnimationResumed = false;
        public bool AnimationStopped = false;
        public bool CanClearWIPData => WIPBarChart.Series.Any();
        public async void RunSimulationAsync()
        {
            ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Simulate", "Running the waferfab simulation");
            controller.SetIndeterminate();

            // Build simulation model
            sim = new Simulation("WaferFab", experimentSettings.OutputDirectory);
            experimentSettings.UpdateSettingsInSimulation(sim);

            // Build model (and initialize waferfab with real snapshot)
            updateWaferFabSettings();

            if (IsStartStateSelected)
            {
                sim = Program.AddModelAndObservers(sim, waferFabSettings, realSnapshotReader.AllRealLots.Where(x => x.SnapshotTime == StartState.Time).ToList());
            }
            else
            {
                sim = Program.AddModelAndObservers(sim, waferFabSettings, new List<RealLot>());
            }


            // Run simulation
            await Task.Run(() => sim.Run());

            // Update WIP data
            ClearAllWIPData();
            SimulationSnapshots.Clear();
            SimulationSnapshots = readWIPSnapshots(experimentSettings.OutputDirectory);

            // Close...
            await controller.CloseAsync();
        }
        public void ClearLastWIPData()
        {
            WIPBarChart.Series.RemoveAt(WIPBarChart.Series.Count - 1);
            WIPBarChart.InvalidatePlot(true);
            ClearAllWIPDataCommand.NotifyOfCanExecuteChange();
            ClearLastWIPDataCommand.NotifyOfCanExecuteChange();
        }
        public void ClearAllWIPData()
        {
            WIPBarChart.Series.Clear();
            WIPBarChart.InvalidatePlot(true);
            ClearAllWIPDataCommand.NotifyOfCanExecuteChange();
            ClearLastWIPDataCommand.NotifyOfCanExecuteChange();
        }
        public async void PlayAnimationAsync()
        {
            if (AnimationPaused)
            {
                resumeAnimation();
            }
            else
            {
                isAnimationRunning = true;
                IsMultipleSnapshots = false;
                updateCommandsCanExecute();

                stopwatch.Start();

                await Task.Run(() => playAnimation());

                isAnimationRunning = false;
                AnimationStopped = false;
                updateCommandsCanExecute();
            }
        }
        private void playAnimation()
        {
            double spf;
            int counterInterval = 1;
            animationCounter = 0;
            int snapShotCount = SnapshotsToAnimate.Count();

            if (FPSAnimation <= FPSMax)
            {
                spf = 1 / FPSAnimation;
            }
            else
            {
                spf = (double)1 / FPSMax;
                counterInterval = Convert.ToInt32(FPSAnimation / FPSMax);
            }

            while (animationCounter < snapShotCount && !AnimationStopped)
            {
                stopwatch.Restart();

                while (animationCounter < snapShotCount)
                {
                    if (AnimationPaused)
                    {
                        stopwatch.Stop();
                    }

                    if (AnimationResumed)
                    {
                        stopwatch.Start();
                        AnimationResumed = false;
                    }

                    if (AnimationStopped)
                    {
                        stopwatch.Stop();
                        break;
                    }

                    if (stopwatch.Elapsed.TotalSeconds > spf)
                    {
                        SnapshotSelected = SnapshotsToAnimate.ElementAt(animationCounter);
                        animationCounter += counterInterval;
                        break;
                    }
                }

            }
        }
        private void resumeAnimation()
        {
            AnimationPaused = false;
            AnimationResumed = true;
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlayAnimationCommand.NotifyOfCanExecuteChange();
                PauseAnimationCommand.NotifyOfCanExecuteChange();
            });
        }
        public void PauseAnimation()
        {
            AnimationPaused = true;
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlayAnimationCommand.NotifyOfCanExecuteChange();
                PauseAnimationCommand.NotifyOfCanExecuteChange();
            });
        }
        public void StopAnimation()
        {
            AnimationStopped = true;
            AnimationPaused = false;
            animationCounter = 0;
        }
        public async void LoadRealSnapshotsAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab";
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Loading", "Reading snapshot data...");
                    controller.SetIndeterminate();

                    realSnapshotReader = new RealSnapshotReader();
                    await Task.Run(() => RealSnapshots = new ObservableCollection<RealSnapshot>(realSnapshotReader.Read(openFileDialog.FileName, 25)));

                    await controller.CloseAsync();
                }
            }
            catch
            {
                throw new Exception("Cannot read selected file");
            }
        }
        private void initializeWaferFabSettings(WaferFabSettings settings)
        {
            // Initialize workcenters
            foreach (var wc in settings.WorkCentersData)
            {
                WorkCenters.Add(new WorkCenterData(wc.Key, 1 / wc.Value));
            }
            // Initialize lotstarts
            foreach (var lotStart in settings.LotStartQtys)
            {
                LotStartQtys.Add(new LotStartQty(lotStart.Key, lotStart.Value));
            }
        }
        private void updateWaferFabSettings()
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

            foreach (var lotStep in xAxisLotSteps)
            {
                if (SnapshotSelected.WIPlevels.ContainsKey(lotStep))                        // Remark: plots 0 if lotStep is unknown in data.
                {
                    series.Items.Add(new ColumnItem(SnapshotSelected.WIPlevels[lotStep]));
                }
                else
                {
                    series.Items.Add(new ColumnItem(0));
                }
            }

            //WIPBarChart.Series.ElementAt(0);


            WIPBarChart.Series.Add(series);
            WIPBarChart.InvalidatePlot(true);

            if (!isAnimationRunning)
            {
                ClearAllWIPDataCommand.NotifyOfCanExecuteChange();
                ClearLastWIPDataCommand.NotifyOfCanExecuteChange();
            }
        }
        private void updateCommandsCanExecute()
        {
            ClearAllWIPDataCommand.NotifyOfCanExecuteChange();
            ClearLastWIPDataCommand.NotifyOfCanExecuteChange();
            PlayAnimationCommand.NotifyOfCanExecuteChange();
            PauseAnimationCommand.NotifyOfCanExecuteChange();
            StopAnimationCommand.NotifyOfCanExecuteChange();
        }

        public ObservableCollection<RealSnapshot> RealSnapshots
        {
            get { return _realSnapshots; }
            set
            {
                _realSnapshots = value;
                NotifyOfPropertyChange();
            }
        }
        public ObservableCollection<SimulationSnapshot> SimulationSnapshots
        {
            get { return _simulationSnapshots; }
            set
            {
                _simulationSnapshots = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(SnapshotsToAnimate));
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
        public IEnumerable<WIPSnapshotBase> SnapshotsToAnimate
        {
            get
            {
                if (IsRealData)
                {
                    return RealSnapshots;
                }
                else
                {
                    return SimulationSnapshots.Where(x => x.Replication == SelectedReplication);
                }
            }
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

        public WIPSnapshotBase SnapshotSelected
        {
            get { return _snapshotSelected; }
            set
            {
                _snapshotSelected = value;
                NotifyOfPropertyChange();
                if (value != null)
                {
                    plotSelectedWIPData();
                }
            }
        }
        public RealSnapshot StartState
        {
            get { return _startState; }
            set
            {
                _startState = value;
                NotifyOfPropertyChange();
            }
        }
        public PlotModel WIPBarChart
        {
            get { return _wipBarChart; }
            set
            {
                _wipBarChart = value;

            }
        }
        public int FPSMax { get; set; } = 10;
        public int NumberOfReplications
        {
            get
            {
                return experimentSettings.NumberOfReplications;
            }
            set
            {
                experimentSettings.NumberOfReplications = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(AllReplications));
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
                SnapshotSelected = null;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(SnapshotsToAnimate));
                PlayAnimationCommand.NotifyOfCanExecuteChange();
            }
        }
        public double LengthOfReplication   // In hours
        {
            get
            {
                return experimentSettings.LengthOfReplication / (60 * 60);
            }
            set
            {
                if (value == -1)
                {
                    experimentSettings.LengthOfReplication = double.MaxValue;
                }
                else
                {
                    experimentSettings.LengthOfReplication = value * 60 * 60;
                }
                NotifyOfPropertyChange();
            }
        }
        public double LengthOfReplicationWallClock
        {
            get
            {
                return experimentSettings.LengthOfReplicationWallClock >= double.MaxValue ? -1 : experimentSettings.LengthOfReplicationWallClock;
            }
            set
            {
                if (value == -1)
                {
                    experimentSettings.LengthOfReplicationWallClock = double.MaxValue;
                }
                else
                {
                    experimentSettings.LengthOfReplicationWallClock = value;
                }
                NotifyOfPropertyChange();
            }
        }
        public double LengthOfWarmUp        // In hours
        {
            get
            {
                return experimentSettings.LengthOfWarmUp / (60 * 60);
            }
            set
            {
                experimentSettings.LengthOfWarmUp = value * 60 * 60;
                NotifyOfPropertyChange();
            }
        }
        public double SampleInterval        // In minutes
        {
            get
            {
                return waferFabSettings.SampleInterval / 60;
            }
            set
            {
                waferFabSettings.SampleInterval = value * 60;
                NotifyOfPropertyChange();
            }
        }
        public double yAxisMinimum
        {
            get
            {
                return yAxis.Minimum == double.NaN ? -1 : yAxis.Minimum;
            }
            set
            {
                if (value == -1)
                {
                    yAxis.Minimum = double.NaN;
                }
                else
                {
                    yAxis.Minimum = value;
                }
                WIPBarChart.InvalidatePlot(true);
            }
        }
        public double yAxisMaximum
        {
            get
            {
                return yAxis.Maximum == double.NaN ? -1 : yAxis.Maximum;
            }
            set
            {
                if (value == -1)
                {
                    yAxis.Maximum = double.NaN;
                }
                else
                {
                    yAxis.Maximum = value;
                }
                WIPBarChart.InvalidatePlot(true);
            }
        }
        private double _fpsAnimation;
        public double FPSAnimation
        {
            get
            {
                return _fpsAnimation;
            }
            set
            {
                if (value <= FPSMax)
                {
                    _fpsAnimation = value;
                }
                else
                {
                    if (value > _fpsAnimation)
                    {
                        _fpsAnimation = Math.Ceiling(value / FPSMax) * FPSMax;
                    }
                    else
                    {
                        _fpsAnimation = Math.Floor(value / FPSMax) * FPSMax;
                    }
                }
                NotifyOfPropertyChange();
            }
        }
        public bool IsRealData
        {
            get { return _isRealData; }
            set
            {
                _isRealData = value;
                NotifyOfPropertyChange();
                PlayAnimationCommand.NotifyOfCanExecuteChange();
            }
        }
        public bool IsMultipleSnapshots
        {
            get { return _isMultipleSnapshots; }
            set
            {
                _isMultipleSnapshots = value;
                NotifyOfPropertyChange();
            }
        }
        public bool IsStartStateSelected
        {
            get { return _isStartStateSelected; }
            set
            {
                _isStartStateSelected = value;
                NotifyOfPropertyChange();
            }
        }
        public RelayCommand RunSimulationCommand { get; }
        public RelayCommand ClearLastWIPDataCommand { get; }
        public RelayCommand ClearAllWIPDataCommand { get; }
        public RelayCommand PlayAnimationCommand { get; }
        public RelayCommand PauseAnimationCommand { get; }
        public RelayCommand StopAnimationCommand { get; }
        public RelayCommand LoadRealSnapshotsCommand { get; }

        private ObservableCollection<SimulationSnapshot> readWIPSnapshots(string outputDir)
        {
            ObservableCollection<SimulationSnapshot> wipSnapshots = new ObservableCollection<SimulationSnapshot>();

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

                        wipSnapshots.Add(new SimulationSnapshot(replicationNumber, header, data));
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
