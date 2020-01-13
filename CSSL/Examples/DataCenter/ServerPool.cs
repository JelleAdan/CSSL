using CSSL.Modeling;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenter
{
    public class Serverpool : SchedulingElement
    {
        public Serverpool(ModelElement parent, string name) : base(parent, name)
        {
        }

        private int jobCount;
    }
}
