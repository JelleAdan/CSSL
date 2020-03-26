using CSSL.Modeling;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Examples.DataCenterSimulation
{
    public class DataCenter : ModelElementBase
    {
        public JobGenerator JobGenerator { get; private set; }

        public Dispatcher Dispatcher { get; private set; }

        public List<ServerPool> ServerPools { get; private set; }

        public DataCenter(ModelElementBase parent, string name) : base(parent, name)
        {
        }

        public void SetJobGenerator(JobGenerator jobGenerator)
        {
            JobGenerator = jobGenerator;
        }

        public void SetDispatcher(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public void AddServerpool(ServerPool serverPool)
        {
            if (ServerPools == null)
            {
                ServerPools = new List<ServerPool>();
            }
            ServerPools.Add(serverPool);
        }

        public void HandleArrival()
        {
            NotifyObservers(this);
        }

        public void HandleDeparture()
        {
            NotifyObservers(this);
        }

    }
}
