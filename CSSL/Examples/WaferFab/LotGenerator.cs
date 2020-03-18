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

        private LotType GetLotType()
        {
            Random rnd = new Random();

            int rndd = rnd.Next(waferFab.Sequences.Count);

            //return waferFab.Sequences.Values;

            return new LotType();
        }

        protected override void HandleGeneration(CSSLEvent e)
        {
            // Schedule next generation
            ScheduleEvent(NextEventTime(), HandleGeneration);


            // Create lot

            // Send lot to first workstation

        }
    }
}
