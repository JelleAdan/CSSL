using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Modeling
{
    internal class Observers : IDisposable
    {
        internal List<ObserverBase> MyObservers { get; set; }

        public Observers()
        {
            MyObservers = new List<ObserverBase>();
        }

        internal void Add(ObserverBase observer)
        {
            MyObservers.Add(observer);
        }

        internal void StrictlyDoBeforeReplication()
        {
            foreach (ObserverBase observer in MyObservers)
            {
                observer.StrictlyDoBeforeReplication();
            }
        }

        internal void StrictlyDoAfterReplication()
        {
            foreach (ObserverBase observer in MyObservers)
            {
                observer.StrictlyDoAfterReplication();
            }
        }

        public void Dispose()
        {
            foreach (ObserverBase observer in MyObservers)
            {
                observer.Dispose();
            }
        }
    }
}
