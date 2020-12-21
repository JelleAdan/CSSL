using CSSL.Utilities;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Resources;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Transactions;

namespace CSSL.Examples.WaferFab.Utilities
{
    [Serializable]
    /// <summary>
    /// WIP-dependent overtaking distribution. Contains emperical distribution per WIP range.
    /// </summary>
    public class OvertakingDistribution : OvertakingDistributionBase
    {
        public OvertakingDistribution(List<OvertakingRecord> records, OvertakingDistributionParameters parameters) : base(records)
        {
            this.records = records.OrderBy(x => x.WIPIn).ToList();
            this.parameters = parameters;
            LB = records.Select(x => x.WIPIn).Min();
            UB = records.Select(x => x.WIPIn).Max();

            distributions = new Dictionary<int, EmpericalDistribution>();
            WIPMapping = new Dictionary<int, int>();

            buildDistributions();
            buildWIPMapping();

            //writeToConsole();
        }

        public override WorkCenter WorkCenter { get; set; }

        private OvertakingDistributionParameters parameters { get; }

        /// <summary>
        /// Records ordered on WIPIn 
        /// </summary>
        private List<OvertakingRecord> records { get; }

        /// <summary>
        /// Lower bound on WIP in records
        /// </summary>
        private int LB { get; }

        /// <summary>
        /// Upper bound on WIP in records
        /// </summary>
        private int UB { get; }

        /// <summary>
        /// Dictionary with the empirical distributions per WIP range. The key is the inclusive lower bound of the WIP range.
        /// </summary>
        private Dictionary<int, EmpericalDistribution> distributions { get; }

        /// <summary>
        /// Ordered dictionary with mapping of all WIP values where LB >= WIP >= UB, to key of dictionary distributions.
        /// Key = WIP. Value = WIPkey, lower inclusive bound of interval.
        /// </summary>
        private Dictionary<int, int> WIPMapping { get; }

        public override double Next()
        {
            // Get WIP right before arrival. TotalQueueLength includes this arrived lot, so this has to be substracted.
            int WIP = WorkCenter.TotalQueueLength - 1;

            double sample = distributions[getWIPKey(WIP)].Next();

            // Make sure that overtaken lots <= WIP
            return Math.Min(sample, WIP);
        }

        /// <summary>
        /// Used for initialization
        /// </summary>
        /// <param name="queuelength"></param>
        /// <returns></returns>
        public override double Next(Lot lot, int WIP)
        {
            double sample = distributions[getWIPKey(WIP)].Next();

            // Make sure that overtaken lots <= WIP
            return Math.Min(sample, WIP);
        }

        private int getWIPKey(int WIP)
        {
            if (WIP < LB)
            {
                return WIPMapping[LB];
            }
            else if (WIP > UB)
            {
                return WIPMapping[UB];
            }
            else
            {
                return WIPMapping[WIP];
            }
        }

        private void buildDistributions()
        {
            int recordCount = records.Count();

            int intervals;

            // Determine number of intervals
            for (intervals = 1; intervals < parameters.MaxNrIntervals; intervals++)
            {
                if ((double)recordCount / intervals < parameters.MinRecordsPerInterval)
                {
                    if (intervals > 1)
                    {
                        intervals = intervals - 1;
                    }
                    break;
                }
            }

            // Determine number of records per interval
            int RecordsPerInterval = recordCount / intervals;

            // Loop through records and select subgroup to create Emperical distribution
            for (int i = 0; i < recordCount; i = i + RecordsPerInterval)
            {
                IEnumerable<OvertakingRecord> selectedRecords = records.Skip(i).Take(RecordsPerInterval);

                if (selectedRecords.Count() >= parameters.MinRecordsPerInterval || i == 0)
                {
                    if (!distributions.ContainsKey(records[i].WIPIn))
                    {
                        distributions.Add(records[i].WIPIn, new EmpericalDistribution(selectedRecords.Select(x => (double)x.OvertakenLots).ToArray()));
                    }
                    // This WIP level already exists in distribution, merge new records with these
                    else
                    {
                        double[] mergedRecords = selectedRecords.Select(x => (double)x.OvertakenLots).Concat(distributions[records[i].WIPIn].Records).ToArray();

                        distributions[records[i].WIPIn] = new EmpericalDistribution(mergedRecords);
                    }

                }
                // This is the last interval and it does not have the minimum required records. Therefore, last added distribution is removed and these records are joined with these.
                else
                {
                    distributions.Remove(records[i - RecordsPerInterval].WIPIn);

                    selectedRecords = records.Skip(i - RecordsPerInterval);

                    distributions.Add(records[i - RecordsPerInterval].WIPIn, new EmpericalDistribution(selectedRecords.Select(x => (double)x.OvertakenLots).ToArray()));
                }
            }
        }

        private void buildWIPMapping()
        {
            List<int> WIPkeys = distributions.Keys.OrderBy(x => x).ToList();

            for (int wip = LB; wip <= UB; wip++)
            {
                // Search for corresponding WIP key
                for (int i = 0; i < WIPkeys.Count(); i++)
                {
                    int key = WIPkeys[i];

                    if (WIPkeys[i] > wip)
                    {
                        WIPMapping.Add(wip, WIPkeys[i - 1]);
                        break;
                    }

                    if (i == WIPkeys.Count() - 1)
                    {
                        WIPMapping.Add(wip, WIPkeys[i]);
                        break;
                    }
                }
            }
        }

        private void writeToConsole()
        {
            foreach(var dist in distributions)
            {
                Console.WriteLine(dist.Key);

                foreach(var value in dist.Value.Records)
                {
                    Console.Write(value + " ");
                }
                Console.Write("\n\n");
            }
        }
    }
}
