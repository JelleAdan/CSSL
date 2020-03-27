using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Statistics
{
    public abstract class CollectorBase : IIdentity, IName, ICollector
    {
        /// <summary>
        /// Incremented to store the total number of created collectors.
        /// </summary>
        private static int collectorCounter;

        public int Id { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// This constructor is called to construct any collector.
        /// </summary>
        /// <param name="name">The name of the collector.</param>
        public CollectorBase(string name)
        {
            Name = name;
            Id = collectorCounter++;
        }

        public abstract void Collect(double value);

        public abstract void Collect(double value, double weight);

        public abstract void Reset();
    }
}
