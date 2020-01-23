using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public class Unsubscriber<observerClass, observableClass> : IDisposable
    {
        private List<observerClass> observers;
        private observerClass observer;
        private IObservable<observableClass> observable;

        internal Unsubscriber(List<observerClass> observers, observerClass observer, IObservable<observableClass> observable)
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
