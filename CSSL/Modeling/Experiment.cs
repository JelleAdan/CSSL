using CSSL.Utilities;
using System;
using System.Collections.Generic;
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
        public Experiment(string name, int numberOfReplications = 1, double lengthOfReplication = double.PositiveInfinity, double lengthOfWarmUp = 0.0)
        {
            Name = name;
            NumberOfReplications = numberOfReplications;
            LengthOfReplication = lengthOfReplication;
            LengthOfWarmUp = lengthOfWarmUp;
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
            get { return LengthOfWarmUp; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be positive.");
                }
                lengthOfWarmUp = value;
            }
        }

        /// <summary>
        /// The current number of finished replications for this experiment.
        /// </summary>
        private int currentReplication;


    }
}
