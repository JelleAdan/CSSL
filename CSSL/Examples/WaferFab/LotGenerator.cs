using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class LotGenerator : EventGeneratorBase
    {
        public LotGenerator(ModelElementBase parent, string name, Distribution interEventTimeDistribution) : base(parent, name, interEventTimeDistribution)
        {
            waferFab = (WaferFab)parent;
        }

        private WaferFab waferFab { get; }

        protected override void HandleGeneration(CSSLEvent e)
        {
            // Schedule next generation
            ScheduleEvent(NextEventTime(), HandleGeneration);

            // Create lots according to preset quantities in LotStarts and send all lots to first workstation
            foreach(KeyValuePair<string, int> lotStart in waferFab.LotStarts)
            {
                Sequence sequence = waferFab.Sequences[lotStart.Key];

                for (int i = 0; i < lotStart.Value; i++)
                {
                    Lot newLot = new Lot(GetTime, sequence);

                    newLot.SendToNextWorkCenter();
                }
            }
        }
    }
}
