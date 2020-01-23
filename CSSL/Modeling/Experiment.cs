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
        /// <param name="numberOfReplications">The number of replication.</param>
        /// <param name="lengthOfReplication">The simulation length of a single replication.</param>
        /// <param name="lengthOfWarmUp">The warm up time of a replication.</param>
        /// <param name="maxCompuationalTimeTotal">The maximum total computational time in seconds.</param>
        /// <param name="maxComputationalTimePerReplication">The maximum computational time per replication in seconds.</param>
        public Experiment(string name, string outputDirectory, int numberOfReplications = 1, double lengthOfReplication = double.PositiveInfinity, double lengthOfWarmUp = 0.0, double maxComputationalTimePerReplication = double.PositiveInfinity, double maxCompuationalTimeTotal = double.PositiveInfinity)
        {
            Name = name;
            this.outputDirectory = outputDirectory;
            NumberOfReplications = numberOfReplications;
            LengthOfReplication = lengthOfReplication;
            LengthOfWarmUp = lengthOfWarmUp;
            MaxComputationalTimePerReplication = maxComputationalTimePerReplication;
            MaxComputationalTimeTotal = maxCompuationalTimeTotal;
        }

        public string Name { get; }

        private int numberOfReplications;

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

        private double lengthOfReplication;

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

        private double lengthOfWarmUp;

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

        private double maxCompuationalTimePerReplication;

        public double MaxComputationalTimePerReplication
        {
            get { return maxCompuationalTimePerReplication; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                maxCompuationalTimePerReplication = value;
            }
        }

        private double maxCompuationalTimeTotal;

        public double MaxComputationalTimeTotal
        {
            get { return maxCompuationalTimeTotal; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                maxCompuationalTimeTotal = value;
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
            currentReplicationNumber = -1;
        }

        internal bool HasMoreReplications => currentReplicationNumber < numberOfReplications;

        /// <summary>
        /// Increments the current replication number by one.
        /// </summary>
        internal int IncrementCurrentReplicationNumber()
        {
            return currentReplicationNumber++;
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
        }

        internal string ReplicationOutputDirectory;

        internal void CreateReplicationOutputDirectory()
        {
            ReplicationOutputDirectory = Path.Combine(ExperimentOutputDirectory, $"rep_{currentReplicationNumber}");
        }
    }
}
