using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    [Serializable]
    public class LotGenerator : EventGeneratorBase
    {
        public LotGenerator(ModelElementBase parent, string name, Distribution interEventTimeDistribution, bool useRealLotStartsFlag) : base(parent, name, interEventTimeDistribution)
        {
            waferFab = (WaferFab)parent;
            UseRealLotStartsFlag = useRealLotStartsFlag;
            previousEventTime = waferFab.InitialDateTime;
        }

        private WaferFab waferFab { get; }

        private DateTime previousEventTime { get; set; }

        private int indexFrom { get; set; } = 0;

        private int indexUntil { get; set; } = 0;


        public bool UseRealLotStartsFlag { get; }

        protected override void HandleGeneration(CSSLEvent e)
        {
            // Schedule next generation
            ScheduleEvent(NextEventTime(), HandleGeneration);

            if (!UseRealLotStartsFlag)
            {
                StartManualLotStarts();
            }
            else
            {
                StartRealLotStarts();
            }
        }

        private void StartManualLotStarts()
        {
            // Create lots according to preset quantities in LotStarts and send all lots to first workstation
            foreach (KeyValuePair<string, int> lotStart in waferFab.ManualLotStarts)
            {
                Sequence sequence = waferFab.Sequences[lotStart.Key];

                for (int i = 0; i < lotStart.Value; i++)
                {
                    Lot newLot = new Lot(GetTime, sequence);

                    newLot.SendToNextWorkCenter();
                }
            }
        }

        private void StartRealLotStarts()
        {
            DateTime currentEventTime = waferFab.GetDateTime;

            // Find from and until indexes
            List<DateTime> dates = waferFab.LotStarts.Select(x => x.Item1).ToList();

            indexFrom = dates.BinarySearch(previousEventTime);
            if (indexFrom < 0) indexFrom = ~indexFrom;

            indexUntil = dates.BinarySearch(currentEventTime);
            if (indexUntil < 0) indexUntil = ~indexUntil;

            // Create lots according to preset quantities in LotStarts and send all lots to first workstation
            foreach (Tuple<DateTime, Lot> lot in waferFab.LotStarts.Skip(indexFrom).Take(indexUntil - indexFrom))
            {
                Lot newLot = lot.Item2;

                Lot deepCopiedLot = new Lot(newLot);

                deepCopiedLot.SendToNextWorkCenter();
            }

            previousEventTime = currentEventTime;
        }

        protected override void OnExperimentStart()
        {
            // Order lot starts on date because StartRealLots utilizes an ordered list to speed up process
            if (waferFab.LotStarts.Any())
            {
                waferFab.LotStarts = waferFab.LotStarts.OrderBy(x => x.Item1).ToList();
            }
        }
    }
}
