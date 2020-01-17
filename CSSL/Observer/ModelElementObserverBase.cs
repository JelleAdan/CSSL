﻿using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Observer
{
    public abstract class ModelElementObserverBase : IObserver<ModelElement> 
    {
        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public void OnNext(ModelElement modelElement)
        {
            switch (modelElement.ObserverState)
            {
                case ModelElementObserverState.WARMUP:
                    OnWarmUp(modelElement);
                    break;
                case ModelElementObserverState.UPDATE:
                    OnUpdate(modelElement);
                    break;
            }
        }

        protected virtual void OnWarmUp(ModelElement modelElement)
        {
        }

        protected virtual void OnUpdate(ModelElement modelElement)
        {
        }

        private IDisposable cancellation;

        public void Subscribe(IObservable<ModelElement> observable)
        {
            cancellation = observable.Subscribe(this);
        }

        public void Unsubscribe()
        {
            cancellation.Dispose();
        }
    }
}
