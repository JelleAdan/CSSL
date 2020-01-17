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

        public double NextEventTime()
        {
            return GetTime() + interEventTimeDistribution.Next();
        }

        protected override void DoBeforeReplication()
        {
            ScheduleEvent(GetExecutive())
        }
    }
}
