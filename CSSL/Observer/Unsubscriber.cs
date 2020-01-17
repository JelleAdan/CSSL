using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public class Unsubscriber<IObserver> : IDisposable
    {
        private List<IObserver> observers;
        private IObserver observer;

        internal Unsubscriber(List<IObserver> observers, IObserver observer)
        {
            this.observers = observers;
            this.observer = observer;
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
