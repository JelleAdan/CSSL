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
        private Distribution interEventTime;

        public EventGenerator(ModelElement parent, string name, Distribution interEventTime) : base(parent, name)
        {
            this.interEventTime = interEventTime;
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

        public void Generate()
        {
            double time = GetTime() + interEventTime.Next();
            //ScheduleEvent(time, );
        }
    }
}
