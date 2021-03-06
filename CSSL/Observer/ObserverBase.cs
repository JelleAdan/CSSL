﻿using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSSL.Observer
{
    /// <summary>
    /// A base class for all CSSL observers
    /// </summary>
    /// <typeparam name="T">The observable object that is passed for notification information.</typeparam>
    public abstract class ObserverBase : IIdentity, IName, IObserver<object>, IDisposable, IGetTime
    {
        /// <summary>
        /// Incremented to store the total number of observers.
        /// </summary>
        private static int observerCounter;

        protected void StrictlyOnExperimentStart()
        {
        }

        protected void StrictlyOnReplicationStart()
        {
        }

        protected void StrictlyOnReplicationEnd()
        {
            writer?.Dispose();
            writer = null;
        }

        protected void StrictlyOnExperimentEnd()
        {
            experimentWriter?.Dispose();
            experimentWriter = null;
        }

        public ObserverBase(Simulation mySimulation)
        {
            Id = observerCounter++;
            Name = $"{Id}_{GetType().Name}";
            MySimulation = mySimulation;
            cancellations = new List<Unsubscriber>();
        }

        public ObserverBase(Simulation mySimulation, string name)
        {
            Id = observerCounter++;
            Name = $"{Id}_{name}";
            MySimulation = mySimulation;
            cancellations = new List<Unsubscriber>();
        }

        public int Id { get; }

        public string Name { get; }

        public double GetTime => MySimulation.MyExecutive.Time;

        public double GetPreviousEventTime => MySimulation.MyExecutive.PreviousEventTime;

        public double GetWallClockTime => MySimulation.MyExecutive.WallClockTime;

        protected readonly Simulation MySimulation;

        private StreamWriter writer { get; set; }

        protected StreamWriter Writer
        {
            get
            {
                return Settings.WriteOutput ? writer ??= new StreamWriter(Path.Combine(MySimulation.MyExperiment.ReplicationOutputDirectory, Name + ".txt")) : null;
            }
            private set
            {
                writer = value;
            }
        }

        private StreamWriter experimentWriter;

        protected StreamWriter ExperimentWriter
        {
            get
            {
                return Settings.WriteOutput ? experimentWriter ??= new StreamWriter(Path.Combine(MySimulation.MyExperiment.ExperimentOutputDirectory, Name + ".txt")) : null;
            }
            private set
            {
                experimentWriter = value;
            }
        }
        protected List<Unsubscriber> cancellations;

        /// <summary>
        /// Subscribes the observer to an observable.
        /// </summary>
        /// <param name="observable">The model element to observe.</param>
        internal void Subscribe(IObservable<object> observable)
        {
            cancellations.Add((Unsubscriber)observable.Subscribe(this));
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
        public void Unsubscribe(IObservable<object> observable)
        {
            cancellations.Where(x => x.observable == observable).First().Dispose();
        }

        /// <summary>
        /// On purpose not an abstract method to prevent obligatory implementation in derived classes. 
        /// </summary>
        public void OnCompleted()
        {
        }

        public abstract void OnError(Exception error);

        public abstract void OnNext(object value);

        public void Dispose()
        {
        }
    }
}
