using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public enum ModelElementObserverState
    {
        UPDATE,
        WARMUP,
        INITIALIZED,
        BEFORE_REPLICATION,
        AFTER_REPLICATION
    }
}
