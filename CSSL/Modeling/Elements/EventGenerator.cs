using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public class EventGenerator : SchedulingElement, IEventGenerator
    {
        private Distribution interEventTimeDistribution;

        public EventGenerator(ModelElement parent, string name, Distribution interEventTimeDistribution) : base(parent, name)
        {
            this.interEventTimeDistribution = interEventTimeDistribution;
        }

        public bool IsOn { get; private set; }

        public void TurnOff()
        {
            IsOn = false;
        }

        public void TurnOn()
        {
            IsOn = true;
        }

        //protected double NextEventTime()
        //{
        //    return GetTime + interEventTimeDistribution.Next();
        //}

        //private void HandleGeneration(CSSLEvent e)
        //{
        //    // Schedule the next generation event
        //    ScheduleEvent(NextEventTime(), HandleGeneration);

        //    // Instantiate a job
        //    Job job = new Job(GetTime);

        //    // Send to dispatcher
        //    dispatcher.HandleArrival(job);
        //}

        //protected override void DoBeforeReplication()
        //{
        //    ScheduleEvent(NextEventTime(), )
        //}
    }
}
