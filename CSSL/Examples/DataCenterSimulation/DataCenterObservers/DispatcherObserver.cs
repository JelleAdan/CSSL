using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class DispatcherObserver : ModelElementObserverBase
    {
        private double sumQueueLength;

        private double sumTime;

        private double oldTime;

        private double currentTime;

        public override void OnCompleted()
        {
            
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected sealed override void OnUpdate(ModelElement modelElement)
        {
            Dispatcher dispatcher = (Dispatcher)modelElement;
            currentTime = dispatcher.GetTime;
            int queueLength = dispatcher.QueueLength;
            sumQueueLength += (currentTime - oldTime) * (double)queueLength;
            sumTime += (currentTime - oldTime);
            oldTime = currentTime;

            Console.WriteLine($"Average queue length: {sumQueueLength / sumTime}");
        }

        protected sealed override void OnInitialized(ModelElement modelElement)
        {
            oldTime = modelElement.GetTime;
        }

        protected sealed override void OnWarmUp(ModelElement modelElement)
        {
        }
    }
}
