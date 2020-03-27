using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public abstract class EventGeneratorBase : SchedulingElementBase, IEventGenerator
    {
        private Distribution interEventTimeDistribution;

        public EventGeneratorBase(ModelElementBase parent, string name, Distribution interEventTimeDistribution) : base(parent, name)
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

        protected double NextEventTime()
        {
            return GetTime + interEventTimeDistribution.Next();
        }

        protected abstract void HandleGeneration(CSSLEvent e);

        protected override void DoBeforeReplication()
        {
            ScheduleEvent(NextEventTime(), HandleGeneration);
        }
    }
}
