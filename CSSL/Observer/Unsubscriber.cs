using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public class Unsubscriber : IDisposable
    {
        private List<IObserver<object>> observers;
        private IObserver<object> observer;
        public IObservable<object> observable;

        internal Unsubscriber(List<IObserver<object>> observers, IObserver<object> observer, IObservable<object> observable)
        {
            this.observers = observers;
            this.observer = observer;
            this.observable = observable;
        }

        public void Dispose()
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
    }
}