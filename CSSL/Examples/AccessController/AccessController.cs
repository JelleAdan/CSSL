using CSSL.Modeling.Elements;
using CSSL.RL;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AccessController
{
    public class AccessController : ModelElementBase
    {
        public RLLayerBase RLLayer { get; set; }

        public CustomerGenerator CustomerGenerator { get; set; }

        public Dispatcher Dispatcher { get; set; }

        public ServerPool ServerPool { get; set; }

        public AccessController(ModelElementBase parent, string name, RLLayer RLLayer) : base(parent, name)
        {
            ServerPool = new ServerPool(this, "Server_pool", 10, new UniformDistribution(10, 20));
            Dispatcher = new Dispatcher(this, "Dispatcher", RLLayer, ServerPool);
            CustomerGenerator = new CustomerGenerator(this, "Customer_generator", new ConstantDistribution(1), new UniformDistribution(0, 4), Dispatcher);
        }
    }
}
