using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab.Dispatchers
{
    public abstract class DispatcherBase : SchedulingElementBase
    {
        public DispatcherBase(ModelElementBase workCenter, string name) : base(workCenter, name)
        {
            wc = (WorkCenter)workCenter;
        }

        protected WorkCenter wc { get; }

        public abstract void HandleArrival(Lot lot);

        public abstract void HandleDeparture();

        public abstract void HandleFirstDeparture();
    }
}
