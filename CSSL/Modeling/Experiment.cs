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
        /// <param name="lengthOfExperimentWallClock">The maximum total wall clock time in seconds.</param>
        /// <param name="lengthOfReplication">The maximum simulation clock time per replication in seconds.</param>
        public Experiment(string name, string outputDirectory, int numberOfReplications = 1, double lengthOfWarmUp = 0.0, double lengthOfReplicationWallClock = double.PositiveInfinity, double lengthOfExperimentWallClock = double.PositiveInfinity, double lengthOfReplication = double.PositiveInfinity, bool fixSeed = false)
        {
            Name = name;
            this.outputDirectory = outputDirectory;
            NumberOfReplications = numberOfReplications;
            LengthOfWarmUp = lengthOfWarmUp;
            LengthOfReplicationWallClock = lengthOfReplicationWallClock;
            LengthOfExperimentWallClock = lengthOfExperimentWallClock;
            LengthOfReplication = LengthOfReplication;
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

        private double lengthOfExperimentWallClock;

        /// <summary>
        /// A threshold on the maximum computational wall clock time of the experiment.
        /// </summary>
        public double LengthOfExperimentWallClock
        {
            get { return lengthOfExperimentWallClock; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                lengthOfExperimentWallClock = value;
            }
        }

        /// <summary>
        /// The current number of finished replications for this experiment.
        /// </summary>
        private int currentReplicationNumber;

        /// <summary>
        /// Resets the current replication number to zero. 
        /// </summary>
        internal void ResetCurrentReplicationNumber()
        {
            currentReplicationNumber = 0;
        }

        internal bool HasMoreReplications => currentReplicationNumber < numberOfReplications;

        /// <summary>
        /// Increments the current replication number by one.
        /// </summary>
        internal int IncrementCurrentReplicationNumber()
        {
            Console.WriteLine($"Replication number: {currentReplicationNumber}");

            return currentReplicationNumber++;
        }

        public int GetCurrentReplicationNumber()
        {
            return currentReplicationNumber;
        }

        private string outputDirectory;

        internal string ExperimentOutputDirectory;

        internal void CreateExperimentOutputDirectory()
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

        internal string ReplicationOutputDirectory;

        internal void CreateReplicationOutputDirectory()
        {
            ReplicationOutputDirectory = Path.Combine(ExperimentOutputDirectory, $"rep_{currentReplicationNumber}");

            if (!Directory.Exists(ReplicationOutputDirectory))
            {
                Directory.CreateDirectory(ReplicationOutputDirectory);
            }
        }
    }
}
