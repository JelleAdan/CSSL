using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenter
{
    public class JobGenerator : EventGenerator
    {
        public JobGenerator(ModelElement parent, string name, Distribution interEventTime) : base(parent, name, interEventTime)
        {
        }

        private Distribution interEventTime;

        public void HandleGeneration(CSSLEvent e)
        {
            // Schedule the next generation event
            ScheduleEvent(NextEventTime(), HandleGeneration);

            // Instantiate a job
            Job job = new Job(GetTime());

            // Send to queue
        }
    }
}
