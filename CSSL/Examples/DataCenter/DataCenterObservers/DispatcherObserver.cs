using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenter.DataCenterObservers
{
    public class DispatcherObserver : ModelElementObserverBase<Dispatcher>
    {
        private double sumQueueLength;

        private double sumTime;

        private double oldTime;

        private double currentTime;

        public DispatcherObserver()
        {
            sumQueueLength = 0;
            sumTime = 0;
            oldTime = 0;
            currentTime = 0;
        }

        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnNext(Dispatcher dispatcher)
        {
            sumQueueLength += (dispatcher.GetTime() - oldTime) * dispatcher.QueueLength;
        }

    }
}
