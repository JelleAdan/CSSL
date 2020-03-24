using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public enum ModelElementObserverState
    {
        BEFORE_EXPERIMENT,
        BEFORE_REPLICATION,
        WARMUP,
        INITIALIZED,
        UPDATE,
        AFTER_REPLICATION,
        AFTER_EXPERIMENT
    }
}
