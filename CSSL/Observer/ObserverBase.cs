using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSSL.Observer
{
    public abstract class ObserverBase<T> : IIdentity, IName, IObserver<T>, IDisposable
    {
        /// <summary>
        /// Incremented to store the total number of observers.
        /// </summary>
        private static int observerCounter;

        public ObserverBase(Simulation mySimulation)
        {
            Id = observerCounter++;
            Name = GetType().Name + "_" + Id;
            mySimulation.MyObservers.Add(this);
            Writer = new StreamWriter(Path.Combine(mySimulation.MyExperiment.ReplicationOutputDirectory, Name + ".txt"));
        }

        internal void StrictlyDoBeforeReplication()
        {

        }

        protected void DoBeforeReplicatio()
        {
        }

        protected readonly StreamWriter Writer;

        protected List<Unsubscriber<ObserverBase<T>, IObservable<T>>> cancellations;

        /// <summary>
        /// Subscribes the observer to an observable.
        /// </summary>
        /// <param name="observable">The model element to observe.</param>
        public void Subscribe(IObservable<T> observable)
        {
            cancellations.Add((Unsubscriber<ObserverBase<T>, IObservable<T>>)observable.Subscribe(this));
        }

        /// <summary>
        /// Unsubscribe observer from all subscribed observables.
        /// </summary>
        public void UnsubscribeAll()
        {
            foreach (IDisposable cancellation in cancellations)
            {
                cancellation.Dispose();
            }
        }

        /// <summary>
        /// Unsubsribe from specific observable.
        /// </summary>
        /// <param name="observable">The observable to unsubscribe from.</param>
        public void Unsubscribe(IObservable<T> observable)
        {
            cancellations.Where(x => x.observable == observable).First().Dispose();
        }

        public int Id { get; }

        public string Name { get; }

        public virtual void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public virtual void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public virtual void OnNext(T value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Writer.Dispose();
        }
    }
}
