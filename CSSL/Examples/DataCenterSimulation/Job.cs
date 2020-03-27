using CSSL.Modeling.CSSLQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenterSimulation
{
    public class Job : CSSLQueueObject<Job>
    {
        public Job(double creationTime) : base(creationTime)
        {
        }

        public double DepartureTime { get; set; }

        public double ServiceTime { get; set; }
    }
}
