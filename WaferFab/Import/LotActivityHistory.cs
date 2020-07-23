using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WaferFabSim.InputDataConversion
{
    [Serializable]
    public class LotActivityHistory
    {

        public string WorkCenter { get; set; }

        List<LotActivity> LotActivities { get; set; }
        
        Tuple<DateTime, int>[] WIPTrace { get; set; }
        
        
        private class LotActivity
        {
            public string LotID { get; set; }

            public DateTime Arrival { get; set; }     

            public DateTime Departure { get; set; }

            public TimeSpan CycleTime => Departure - Arrival;

            public int WIPin { get; set; }

            public int WIPout { get; set;}

            public int LotQuantity { get; set; }

        }

    }
}
