using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public class Unsubscriber<Observer, Observable> : IDisposable
    {
        private List<Observer> observers;
        private Observer observer;
        public IObservable<Observable> observable;

        internal Unsubscriber(List<Observer> observers, Observer observer, IObservable<Observable> observable)
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