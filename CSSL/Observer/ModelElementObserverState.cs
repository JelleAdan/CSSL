using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public enum ModelElementObserverState
    {
        EXPERIMENT_START,
        REPLICATION_START,
        WARMUP,
        INITIALIZED,
        UPDATE,
        REPLICATION_END,
        EXPERIMENT_END
    }
}
