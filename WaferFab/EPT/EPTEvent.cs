using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabSim.EPT
{
    public class EPTEvent
    {
        public int ArrivalNumber { get; private set; }  // = 0, if this lot has no arrival event

        public DateTime Time { get; private set; }

        public char EventType { get; private set; }     // A = Arrival, D = Departure

        public string LotId { get; private set; }

        public EPTEvent()
        {

        }
    }
}
