using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public class Model : ModelElementBase
    {
        public Model(string name, Simulation simulation) : base(name)
        {
            MySimulation = simulation;
        }

        public Simulation MySimulation { get; }

        protected override Model MyModel => this;

        protected override void DoBeforeExperiment()
        {
            if (!HasChildren)
            {
                throw new Exception("The model does not contain any child model elements.");
            }
        }
    }
}