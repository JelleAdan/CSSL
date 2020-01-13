using CSSL.Modeling;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenter
{
    public class ServerpoolsDispatcher : ModelElement
    {


        private JobGenerator jobGenerator;

        private Dispatcher dispatcher;

        private List<Serverpool> serverPools;

        public ServerpoolsDispatcher(ModelElement parent, string name) : base(parent, name)
        {
        }

        public void SetJobGenerator(JobGenerator jobGenerator)
        {
            this.jobGenerator = jobGenerator;
        }

        public void SetDispatcher(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void AddServerpool(Serverpool serverpool)
        {
            if (serverPools == null)
            {
                serverPools = new List<Serverpool>();
            }
            serverPools.Add(serverpool);
        }
    }
}
