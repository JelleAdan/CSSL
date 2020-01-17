﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public class Model : ModelElement
    {
        public Model(string name, Simulation simulation) : base(name)
        {
            MySimulation = simulation;
        }

        public Simulation MySimulation { get; }

        public override double LengthOfWarmUp => MySimulation.MyExperiment.LengthOfWarmUp;

        protected sealed override Model GetModel()
        {
            return this;
        }
    }
}
