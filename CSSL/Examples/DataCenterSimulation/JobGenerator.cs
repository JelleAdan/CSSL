using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenterSimulation
{
    public class JobGenerator : EventGeneratorBase
    {
        public JobGenerator(ModelElementBase parent, string name, Distribution interEventTimeDistribution, Dispatcher dispatcher) : base(parent, name, interEventTimeDistribution)
        {
            this.dispatcher = dispatcher;
        }

        private Dispatcher dispatcher;

        protected override void HandleGeneration(CSSLEvent e)
        {
            // Schedule the next generation event.
            ScheduleEvent(NextEventTime(), HandleGeneration);

            // Instantiate a job.
            Job job = new Job(GetTime);

            // Send to dispatcher.
            dispatcher.HandleArrival(job);

        }
    }
}
