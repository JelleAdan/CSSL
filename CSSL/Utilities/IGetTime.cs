using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities
{
    public interface IGetTime
    {
        double GetTime { get; }

        double GetPreviousEventTime { get; }

        double GetWallClockTime { get; }
    }
}
