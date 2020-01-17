using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Observer
{
    public abstract class ModelElementObserverBase<T> : IObserver<T>
    {
        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public abstract void OnNext(T value);

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
