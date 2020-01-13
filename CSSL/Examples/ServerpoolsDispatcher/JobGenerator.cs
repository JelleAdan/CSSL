using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.ServerpoolsDispatcher
{
    public class JobGenerator : EventGenerator
    {
        public JobGenerator(ModelElement parent, string name, Distribution interEventTime, Distribution serviceTime) : base(parent, name, interEventTime)
        {
            this.serviceTime = serviceTime;
        }

        private Distribution serviceTime;
    }
}
