using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public class Experiment : IName
    {
        /// <summary>
        /// Constructs an experiment.
        /// </summary>
        /// <param name="name">The name of the experiment.</param>
        /// <param name="outputDirectory">The output directory the the results have to be written to.</param>
        /// <param name="numberOfReplications">The number of replications.</param>
        /// <param name="lengthOfWarmUp">The warm up time of a replication in simulation clock time.</param>
        /// <param name="lengthOfReplicationWallClock">The maximum wall clock time per replication in seconds.</param>
        /// <param name="lengthOfReplication">The maximum simulation clock time per replication in seconds.</param>
        public Experiment(string name, string outputDirectory, int numberOfReplications = 1, double lengthOfWarmUp = 0.0, double lengthOfReplicationWallClock = double.PositiveInfinity, double lengthOfReplication = double.PositiveInfinity)
        {
            Name = name;
            this.outputDirectory = outputDirectory;
            NumberOfReplications = numberOfReplications;
            LengthOfWarmUp = lengthOfWarmUp;
            LengthOfReplicationWallClock = lengthOfReplicationWallClock;
            LengthOfReplication = lengthOfReplication;
        }

        public string Name { get; }

        private int numberOfReplications;

        /// <summary>
        /// The number of replications.
        /// </summary>
        public int NumberOfReplications
        {
            get { return numberOfReplications; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                numberOfReplications = value;
            }
        }

        private double lengthOfWarmUp;

        /// <summary>
        /// Length of warm up in seconds in simulation clock time.
        /// </summary>
        public double LengthOfWarmUp
        {
            get { return lengthOfWarmUp; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                lengthOfWarmUp = value;
            }
        }

        public static double lengthOfReplication;

        /// <summary>
        /// Length of replication (including warm-up) in seconds in simulation clock time.
        /// </summary>
        public double LengthOfReplication
        {
            get { return lengthOfReplication; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                lengthOfReplication = value;
            }
        }

        private double lengthOfReplicationWallClock;

        /// <summary>
        /// Length of replication in seconds in wall clock time.
        /// </summary>
        public double LengthOfReplicationWallClock
        {
            get { return lengthOfReplicationWallClock; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                lengthOfReplicationWallClock = value;
            }
        }

        private int currentReplicationNumber;

        private void ResetCurrentReplicationNumber()
        {
            currentReplicationNumber = 0;
        }

        internal bool HasMoreReplications => currentReplicationNumber < numberOfReplications;

        internal int IncrementCurrentReplicationNumber()
        {
            Console.WriteLine($"Replication number: {currentReplicationNumber}"); // TODO

            return currentReplicationNumber++;
        }

        internal int GetCurrentReplicationNumber()
        {
            return currentReplicationNumber;
        }

        private string outputDirectory;

        internal string ExperimentOutputDirectory { get; private set; }

        private void CreateExperimentOutputDirectory()
        {
            if (Settings.Output)
            {
                ExperimentOutputDirectory = Path.Combine(outputDirectory, Name);
                if (Directory.Exists(ExperimentOutputDirectory))
                {
                    int counter = 1;
                    while (Directory.Exists(ExperimentOutputDirectory + $"_{counter}"))
                    {
                        counter++;
                    }
                    ExperimentOutputDirectory = ExperimentOutputDirectory + $"_{counter}";
                }
                Directory.CreateDirectory(ExperimentOutputDirectory);
            }
        }

        internal string ReplicationOutputDirectory { get; private set; }

        private void CreateReplicationOutputDirectory()
        {
            if (Settings.Output)
            {
                ReplicationOutputDirectory = Path.Combine(ExperimentOutputDirectory, $"rep_{currentReplicationNumber}");

                if (!Directory.Exists(ReplicationOutputDirectory))
                {
                    Directory.CreateDirectory(ReplicationOutputDirectory);
                }
            }
        }

        private void CheckReplicationLengths()
        {
            if (lengthOfReplication == double.PositiveInfinity && lengthOfReplicationWallClock == double.PositiveInfinity)
            {
                throw new Exception("The experiment has an infinite horizon. Specify LengthOfReplication or LengthOfReplicationWallClock.");
            }
        }

        internal void StrictlyOnExperimentStart()
        {
            CheckReplicationLengths();
            ResetCurrentReplicationNumber();
            CreateExperimentOutputDirectory();
        }

        internal void StrictlyOnReplicationStart()
        {
            CreateReplicationOutputDirectory();
        }

        private class ExperimentReporter
        {
            private readonly Simulation simulation;
            private readonly Experiment experiment;
            private List<string> summary { get; set; }

            public ExperimentReporter()
            {
                this.simulation = simulation ?? throw new ArgumentNullException("simulation", "Cannot make simulation reporter since simulation is null.")
    ;
                experiment = simulation.MyExperiment;
                summary = new List<string>();
            }

            private void BuildSummary()
            {
                summary.Add($"\nSIMULATION REPORT FOR {simulation.Name}\n");

                summary.Add("EXPERIMENT");
                summary.Add($"Maximum number of replications: {experiment.NumberOfReplications}");
                summary.Add($"Maximum computational time per replication: {experiment.LengthOfReplicationWallClock} s");
                summary.Add($"Length of warm-up in simulation clock time: {experiment.LengthOfWarmUp} s");
                summary.Add($"Length of replication in simulation clock time: {experiment.LengthOfReplication} s");
                summary.Add($"Length of replication in wall clock time: {experiment.LengthOfReplicationWallClock} s\n");

                summary.Add("EXECUTION SUMMARY");
                summary.Add($"Number of replications: {experiment.GetCurrentReplicationNumber()}");
                summary.Add($"Stopped in state: {simulation.GetEndStateIndicator}");
                TimeSpan time = simulation.GetWallClockTimeSpan;
                summary.Add($"Total computational time:{time.Days}d:{time.Hours}h:{time.Minutes}m:{time.Seconds}s:{time.Milliseconds}ms");
            }

            internal void PrintSummaryToFile()
            {
                if (!summary.Any()) { BuildSummary(); }

                using (StreamWriter writer = new StreamWriter(experiment.ExperimentOutputDirectory + @"\Summary.txt"))
                {
                    foreach (string line in summary)
                    {
                        writer.WriteLine(line);
                    }
                }
            }

            internal void PrintSummaryToConsole()
            {
                if (!summary.Any()) { BuildSummary(); }

                foreach (string line in summary)
                {
                    Console.WriteLine(line);
                }
            }

        }
    }
}
