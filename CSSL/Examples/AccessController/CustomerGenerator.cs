using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AccessController
{
    public class CustomerGenerator : EventGeneratorBase
    {
        private Distribution customerDistribution;

        private Dispatcher dispatcher;

        private int customers;

        public CustomerGenerator(
            ModelElementBase parent,
            string name,
            Distribution interEventTimeDistribution,
            Distribution customerDistribution,
            Dispatcher dispatcher) : base(parent, name, interEventTimeDistribution)
        {
            this.customerDistribution = customerDistribution;
            this.dispatcher = dispatcher;
        }

        protected override void HandleGeneration(CSSLEvent e)
        {
            Customer customer = new Customer((int)customerDistribution.Next());
            dispatcher.HandleCustomer(customer);
            if (customers < 600)
            {
                ScheduleEvent(NextEventTime(), HandleGeneration);
            }
        }

        protected override void OnReplicationStart()
        {
            base.OnReplicationStart();
            customers = 0;
        }
    }
}
