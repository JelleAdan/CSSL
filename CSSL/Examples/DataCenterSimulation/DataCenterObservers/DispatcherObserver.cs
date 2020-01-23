using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSSL.Examples.DataCenterSimulation.DataCenterObservers
{
    public class DispatcherObserver : ModelElementObserverBase
    {
        public DispatcherObserver(Simulation sim) : base(sim)
        {

        }

        private double sumQueueLength;

        private double sumTime;

        private double oldTime;

        private double currentTime;

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected sealed override void OnUpdate(ModelElementBase modelElement)
        {
            Dispatcher dispatcher = (Dispatcher)modelElement;
            currentTime = dispatcher.GetTime;
            int queueLength = dispatcher.QueueLength;
            sumQueueLength += (currentTime - oldTime) * (double)queueLength;
            sumTime += (currentTime - oldTime);
            oldTime = currentTime;

            Writer.WriteLine(queueLength);
            //Console.WriteLine($"Average queue length: {sumQueueLength / sumTime}");
        }

        protected sealed override void OnInitialized(ModelElementBase modelElement)
        {
            oldTime = modelElement.GetTime;
        }

        protected sealed override void OnWarmUp(ModelElementBase modelElement)
        {
        }
    }
}
