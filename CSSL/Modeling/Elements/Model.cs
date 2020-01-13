using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public class Model : ModelElement
    {
        public Model(string name, Executive executive) : base(name)
        {
            Executive = executive;
        }

        public Executive Executive { get; }

        protected sealed override Model GetModel()
        {
            return this;
        }
    }
}
