using Caliburn.Micro;
using CSSL.Examples.WaferFab;
using CSSL.Modeling;
using CSSL.Utilities.Distributions;
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
using WaferFabSim.Import;
using WaferFabSim.InputDataConversion;
using WaferFabSim.SnapshotData;
using Action = System.Action;

namespace WaferFabGUI.ViewModels
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        public ShellViewModel()
        {
            string inputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\";
            string outputDir = @"C:\CSSLWaferFab\";

            WaferFabSim = new ShellModel(inputDir + @"\SerializedFiles", outputDir);
            WaferFabSim.ReadSimulationResults();
            WaferFabSim.PropertyChanged += ShellModel_PropertyChanged;

            // Link relay commands
            RunSimulationCommand = new RelayCommand(RunSimulationAsync, () => true);
            ClearAllWIPDataCommand = new RelayCommand(ClearAllWIPPlots, () => CanClearWIPData);
            ClearLastWIPDataCommand = new RelayCommand(ClearLastWIPPlot, () => CanClearWIPData);
            PlayAnimationCommand = new RelayCommand(PlayAnimationAsync, () => (SnapshotsToAnimate != null && SnapshotsToAnimate.Any() && !isAnimationRunning) || AnimationPaused);
            PauseAnimationCommand = new RelayCommand(PauseAnimation, () => isAnimationRunning && !AnimationPaused);
            StopAnimationCommand = new RelayCommand(StopAnimation, () => isAnimationRunning);
            LoadRealSnapshotsCommand = new RelayCommand(LoadRealSnapshotsAsync, () => true);

            // Initialize properties
            WorkCenters = new ObservableCollection<WorkCenterData>();
            LotStartQtys = new ObservableCollection<LotStartQty>();
            XAxisLotSteps = new ObservableCollection<CheckBoxLotStep>();
            WIPBarChart = new PlotModel { Title = "WIP balance" };
            dialogCoordinator = DialogCoordinator.Instance;
            inputDirectory = inputDir;
            FPSAnimation = 5;
            FPSMax = 5;
            waferThresholdQty = 25;

            // Initialize Experiment Settings
            experimentSettings = new ExperimentSettings();
            experimentSettings.NumberOfReplications = 1;
            experimentSettings.LengthOfReplication = 100 * 24 * 60 * 60;
            experimentSettings.LengthOfWarmUp = 12 * 60 * 60;
            Settings.Output = true;

            // Initialize Waferfab Settings
            waferFabReader = new AutoDataReader(inputDirectory + @"\Auto");
            waferFabSettings = waferFabReader.ReadWaferFabSettings();
            initializeGUIWaferFabSettings(waferFabSettings);

            // TEMPORARY
            var reader = new ManualDataReader(inputDirectory + @"\Manual");
            var settings = reader.ReadWaferFabSettings();

            // Initialize bar chart
            SimulationSnapshots = new ObservableCollection<SimulationSnapshot>(WaferFabSim.MySimResults.SimulationSnapshots);
            foreach (var step in waferFabSettings.LotSteps.OrderBy(x => x.Value.Id))
            {
                XAxisLotSteps.Add(new CheckBoxLotStep(settings.LotSteps.ContainsKey(step.Key), step.Key));
            }
            xAxis = new CategoryAxis() { Angle = 60, ItemsSource = XAxisLotSteps.Where(x => x.Selected).Select(x => x.Name) };
            WIPBarChart.Axes.Add(xAxis);
            yAxis = new LinearAxis() { Minimum = 0 };
            WIPBarChart.Axes.Add(yAxis);
        }

        public ShellModel WaferFabSim { get; set; }

        private Stopwatch stopwatch = new Stopwatch();
        private LinearAxis yAxis;
        private CategoryAxis xAxis;
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
        private int waferThresholdQty;
        private ObservableCollection<CheckBoxLotStep> _xAxisLotSteps;
        private ObservableCollection<WorkCenterData> _workCenters;
        private ObservableCollection<LotStartQty> _lotStartQtys;
        private ObservableCollection<SimulationSnapshot> _simulationSnapshots;
        private ObservableCollection<RealSnapshot> _realSnapshots;
        private PlotModel _wipBarChart;
        private WIPSnapshotBase _snapshotSelected;
        private RealSnapshot _startState;
        private IDialogCoordinator dialogCoordinator;
        private DataReaderBase waferFabReader;

        public bool AnimationPaused = false;
        public bool AnimationResumed = false;
        public bool AnimationStopped = false;
        public bool CanClearWIPData => WIPBarChart.Series.Any();
        public async void RunSimulationAsync()
        {
            ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Simulate", "Running the waferfab simulation");
            controller.SetIndeterminate();

            // Build simulation model
            updateWaferFabSettings();

            WaferFabSim.MyWaferFabSettings = waferFabSettings;

            WaferFabSim.MyExperimentSettings = experimentSettings;

            // Run simulation
            await Task.Run(() => WaferFabSim.RunSimulation());

            // Update WIP data
            ClearAllWIPPlots();

            // Close...
            await controller.CloseAsync();
        }
        public async void LoadRealSnapshotsAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\SerializedFiles";
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Loading", "Reading snapshot data...");
                    controller.SetIndeterminate();

                    await Task.Run(() => WaferFabSim.ReadRealSnaphots(openFileDialog.FileName));

                    await controller.CloseAsync();
                }
            }
            catch
            {
                throw new Exception("Cannot read selected file");
            }
        }
        public void ClearLastWIPPlot()
        {
            WIPBarChart.Series.RemoveAt(WIPBarChart.Series.Count - 1);
            WIPBarChart.InvalidatePlot(true);
            ClearAllWIPDataCommand.NotifyOfCanExecuteChange();
            ClearLastWIPDataCommand.NotifyOfCanExecuteChange();
        }
        public void ClearAllWIPPlots()
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

        private void initializeGUIWaferFabSettings(WaferFabSettings settings)
        {
            // Initialize workcenters
            foreach (var wc in settings.WorkCenters)
            {
                WorkCenters.Add(new WorkCenterData(wc, settings.WCServiceTimeDistributions[wc].Mean));
            }
            // Initialize lotstarts
            foreach (var lotStart in settings.ManualLotStartQtys)
            {
                LotStartQty newLotStart = new LotStartQty(lotStart.Key, lotStart.Value);
                newLotStart.PropertyChanged += LotStartQtys_PropertyChanged;

                LotStartQtys.Add(newLotStart);
            }
        }
        private void updateWaferFabSettings()
        {
            //foreach (var wc in WorkCenters)
            //{
            //    waferFabSettings.WorkCenterDistributions[wc.Name] = new ExponentialDistribution(wc.ExponentialRate);
            //}
            foreach (var lotStart in LotStartQtys)
            {
                waferFabSettings.ManualLotStartQtys[lotStart.LotType] = lotStart.Quantity;
            }

            if (IsStartStateSelected)
            {
                waferFabSettings.InitialRealLots = WaferFabSim.RealSnapshotReader.RealLots.Where(x => x.SnapshotTime == StartState.Time && x.Qty >= waferThresholdQty).ToList();
            }
            else
            {
                waferFabSettings.InitialRealLots = new List<RealLot>();
            }
        }
        private void plotSelectedWIPData()
        {
            if (!IsMultipleSnapshots)
            {
                WIPBarChart.Series.Clear();
            }

            var series = new ColumnSeries();

            foreach (var lotStep in XAxisLotSteps.Where(x => x.Selected))
            {

                if (SnapshotSelected.WIPlevels.ContainsKey(lotStep.Name))                        // Remark: plots 0 if lotStep is unknown in data.
                {
                    series.Items.Add(new ColumnItem(SnapshotSelected.WIPlevels[lotStep.Name]));
                }
                else
                {
                    series.Items.Add(new ColumnItem(0));
                }
            }

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
        public ObservableCollection<CheckBoxLotStep> XAxisLotSteps
        {
            get
            {
                return _xAxisLotSteps;
            }
            set
            {
                _xAxisLotSteps = value;
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

        public double TotalDailyLotStarts
        {
            get
            {
                return LotStartQtys.Select(x => x.Quantity).Sum() / (LotStartFrequency / 24.0);
            }
        }
        public double TotalMonthlyLotStarts => TotalDailyLotStarts * 30;
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
                NotifyOfPropertyChange(nameof(TotalDailyLotStarts));
                NotifyOfPropertyChange(nameof(TotalMonthlyLotStarts));
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

        public bool IsRealLotStartsSelected
        {
            get { return waferFabSettings.UseRealLotStartsFlag; }
            set
            {
                waferFabSettings.UseRealLotStartsFlag = value;
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

        private void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShellModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MySimResults")
            {
                SimulationSnapshots = new ObservableCollection<SimulationSnapshot>(WaferFabSim.MySimResults.SimulationSnapshots);
            }

            if (e.PropertyName == "RealSnapshotReader")
            {
                RealSnapshots = new ObservableCollection<RealSnapshot>(WaferFabSim.RealSnapshotReader.RealSnapshots);
            }
        }

        private void LotStartQtys_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Quantity")
            {
                NotifyOfPropertyChange(nameof(TotalDailyLotStarts));
                NotifyOfPropertyChange(nameof(TotalMonthlyLotStarts));
            }
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

        public class CheckBoxLotStep
        {
            public CheckBoxLotStep(bool selected, string name)
            {
                Selected = selected;
                Name = name;
            }
            public bool Selected { get; set; }
            public string Name { get; set; }
        }
    }
}
