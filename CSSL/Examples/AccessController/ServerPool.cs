using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AccessController
{
    public class ServerPool : SchedulingElementBase
    {
        public int TotalServers { get; }

        public int FreeServers => TotalServers - OccupiedServers;

        public int OccupiedServers { get; private set; }

        private Distribution serviceTimeDistribution;

        public ServerPool(ModelElementBase parent, string name, int totalServers, Distribution serviceTimeDistribution) : base(parent, name)
        {
            TotalServers = totalServers;
            this.serviceTimeDistribution = serviceTimeDistribution;
        }

        public int AcceptCustomer(Customer customer)
        {
            OccupiedServers++;
            ScheduleEvent(GetTime + serviceTimeDistribution.Next(), HandleDeparture);
            return customer.GetReward();
        }

        public void HandleDeparture(CSSLEvent e)
        {
            OccupiedServers--;
        }

        protected override void OnReplicationStart()
        {
            base.OnReplicationStart();
            OccupiedServers = 0;
        }
    }
}
