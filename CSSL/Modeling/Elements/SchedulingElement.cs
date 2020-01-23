using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSSL.Modeling.Elements.CSSLEvent;

namespace CSSL.Modeling.Elements
{
    public abstract class SchedulingElement : ModelElementBase
    {
        public SchedulingElement(ModelElementBase parent, string name) : base(parent, name)
        {
        }

        protected void ScheduleEvent(double time, CSSLEventAction action)
        {
            GetExecutive.ScheduleEvent(time, action);
        }
    }
}
