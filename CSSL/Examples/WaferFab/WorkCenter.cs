using CSSL.Modeling.CSSLQueue;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class WorkCenter : SchedulingElementBase
    {

        private WorkCenterQueue queue { get; set; }

        public WorkCenter(ModelElementBase parent, string name) : base(parent, name)
        {
            queue = new WorkCenterQueue(this, name + "_Queue");
        }


        private class WorkCenterQueue : CSSLQueue<Lot>
        {
            public WorkCenterQueue(ModelElementBase parent, string name) : base(parent, name)
            {
            }
        }
    }
}
