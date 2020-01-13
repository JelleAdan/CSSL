using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public interface IEventGenerator
    {
        bool IsOn { get; }

        void TurnOn();

        void TurnOff();
    }
}
