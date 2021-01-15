﻿using CSSL.Modeling.Elements;
using CSSL.RL;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AccessController
{
    public class Dispatcher : RLElementBase
    {
        private ServerPool serverPool;

        private Customer customer;

        private double[] state;

        private int reward;

        public Dispatcher(
            ModelElementBase parent,
            string name,
            RLLayerBase reinforcementLearningLayer,
            ServerPool serverPool) : base(parent, name, reinforcementLearningLayer)
        {
            this.serverPool = serverPool;
            state = new double[2];
        }

        public void HandleCustomer(Customer customer)
        {
            this.customer = customer;
            state[0] = customer.Type;
            state[1] = serverPool.FreeServers;
            GetAction(state, reward);
        }

        public override void Act(int action)
        {
            switch (action)
            {
                case 0:
                    reward = 0;
                    return;
                case 1:
                    reward = serverPool.AcceptCustomer(customer);
                    return;
            }
            throw new Exception();
        }
    }
}
