using CSSL.Examples.DataCenterSimulation;
using CSSL.Observer;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public abstract class ModelElementBase : IIdentity, IName, IObservable<object>, IGetTime
    {
        /// <summary>
        /// Incremented to store the total number of created model elements.
        /// </summary>
        private static int modelElementCounter;

        public int Id { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// This constuctor is called to construct any ModelElement.
        /// </summary>
        /// <param name="parent">A reference to the parent model element.</param>
        /// <param name="name">The name of the model element.</param>
        public ModelElementBase(ModelElementBase parent, string name)
        {
            ConstructorCalls(name);
            Parent = parent ?? throw new ArgumentNullException($"Tried to construct ModelElement with name \"{name}\" but the parent ModelElement is null.");
            parent.AddModelElement(this);
            MyModel = parent.MyModel;
        }

        /// <summary>
        /// This constuctor is only called by Model so that Model does not require a parent model element.
        /// </summary>
        /// <param name="name">The name of the model.</param>
        internal ModelElementBase(string name)
        {
            ConstructorCalls(name);
        }

        /// <summary>
        /// Default constructor calls.
        /// </summary>
        /// <param name="name">The name of the model.</param>
        private void ConstructorCalls(string name)
        {
            Name = name;
            Id = modelElementCounter++;
            modelElements = new List<ModelElementBase>();
            observers = new List<IObserver<object>>();
        }

        /// <summary>
        /// A reference to the parent model element.
        /// </summary>
        public ModelElementBase Parent { get; private set; }

        /// <summary>
        /// A reference to the overall model, the highest container for all model elements.
        /// </summary>
        protected virtual Model MyModel { get; private set; }

        /// <summary>
        /// Retrieves the executive.
        /// </summary>
        /// <returns></returns>
        protected Executive GetExecutive => MyModel.MySimulation.MyExecutive;

        /// <summary>
        /// Retrieves the simulation.
        /// </summary>
        private Simulation GetSimulation => MyModel.MySimulation;

        /// <summary>
        /// Retrieves the current elapsed time.
        /// </summary>
        public double GetTime => GetExecutive.Time;

        /// <summary>
        /// Retrieves the time at which the previous event was executed.
        /// </summary>
        public double GetPreviousEventTime => GetExecutive.PreviousEventTime;

        /// <summary>
        /// Retrieves the current elapsed wall clock time.
        /// </summary>
        public double GetWallClockTime => GetExecutive.WallClockTime;

        /// <summary>
        /// All children modelElements.
        /// </summary>
        private List<ModelElementBase> modelElements;

        /// <summary>
        /// Adds the supplied model element as a child to this model element. 
        /// </summary>
        /// <param name="modelElement">The model element to be added.</param>
        private void AddModelElement(ModelElementBase modelElement)
        {
            modelElements.Add(modelElement);
        }

        /// <summary>
        /// Removes the supplied child model element from this model element.
        /// </summary>
        /// <param name="modelElement">The model element to be removed.</param>
        private void RemoveModelElement(ModelElementBase modelElement)
        {
            modelElements.Remove(modelElement);
        }

        /// <summary>
        /// Changes the parent model element of this model element to the supplied model element. 
        /// </summary>
        /// <param name="newParent">The parent for this model element.</param>
        private void ChangeParentModelElement(ModelElementBase newParent)
        {
            ModelElementBase oldParent = Parent;
            if (oldParent != newParent)
            {
                oldParent.RemoveModelElement(this);
                newParent.AddModelElement(this);
                MyModel = newParent.MyModel;
            }
        }

        /// <summary>
        /// Returns true if the model element contains any child model elements.
        /// </summary>
        protected bool HasChildren => modelElements.Any();

        /// <summary>
        /// The observer state of the model element.
        /// </summary>
        internal ModelElementObserverState ObserverState { get; private set; }

        /// <summary>
        /// This method contains logic to be performed prior to an experiment. 
        /// It is called once before the first replication. This method ensures that each contained model element has its StrictlyDoBeforeExperiment method called.
        /// It also calls the DoBeforeExperiment method which contains optional logic. 
        /// </summary>
        internal void StrictlyDoBeforeExperiment()
        {
            ObserverState = ModelElementObserverState.BEFORE_EXPERIMENT;
            NotifyObservers(this);

            DoBeforeExperiment();

            if (modelElements.Any())
            {
                foreach (ModelElementBase modelElement in modelElements)
                {
                    modelElement.StrictlyDoBeforeExperiment();
                }
            }
        }

        /// <summary>
        /// This method should be overridden by derived classes that need logic to be performed prior to an experiment. 
        /// </summary>
        protected virtual void DoBeforeExperiment()
        {
        }

        /// <summary>
        /// This method contains logic to be performed after an experiment. 
        /// It is called once after the first replication. This method ensures that each contained model element has its StrictlyDoAfterExperiment method called.
        /// It also calls the DoBeforeExperiment method which contains optional logic. 
        /// </summary>
        internal void StrictlyDoAfterExperiment()
        {
            ObserverState = ModelElementObserverState.AFTER_EXPERIMENT;
            NotifyObservers(this);

            DoAfterExperiment();

            if (modelElements.Any())
            {
                foreach (ModelElementBase modelElement in modelElements)
                {
                    modelElement.StrictlyDoBeforeExperiment();
                }
            }
        }

        /// <summary>
        /// This method should be overridden by derived classes that need logic to be performed after an experiment. 
        /// </summary>
        protected virtual void DoAfterExperiment()
        {
        }

        /// <summary>
        /// This method contains logic to be performed prior to a replication. 
        /// It is called once before every replication. This method ensures that each contained model element has its StrictlyDoBeforeReplication method called.
        /// It also calls the DoBeforeReplication method which contains optional logic.
        /// </summary>
        internal void StrictlyDoBeforeReplication()
        {
            ObserverState = ModelElementObserverState.BEFORE_REPLICATION;
            NotifyObservers(this);

            if (LengthOfWarmUp > 0)
            {
                GetExecutive.ScheduleEvent(GetExecutive.Time + LengthOfWarmUp, HandleEndWarmUp);
                ObserverState = ModelElementObserverState.WARMUP;
                NotifyObservers(this);
            }
            else
            {
                ObserverState = ModelElementObserverState.INITIALIZED;
                NotifyObservers(this);
                ObserverState = ModelElementObserverState.UPDATE;
            }

            DoBeforeReplication();

            if (modelElements.Any())
            {
                foreach (ModelElementBase modelElement in modelElements)
                {
                    modelElement.StrictlyDoBeforeReplication();
                }
            }
        }

        /// <summary>
        /// This method should be overridden by derived classes that need additional logic to be performed prior to a replication. 
        /// </summary>
        protected virtual void DoBeforeReplication()
        {
        }

        /// <summary>
        /// This method contains logic to be performed after a replication. 
        /// It is called once after every replication. This method ensures that each contained model element has its StrictlyDoAfterReplication method called.
        /// It also calls the DoAfterReplication method which contains optional logic.
        /// </summary>
        internal void StrictlyDoAfterReplication()
        {
            ObserverState = ModelElementObserverState.AFTER_REPLICATION;
            NotifyObservers(this);

            DoAfterReplication();

            if (modelElements.Any())
            {
                foreach (ModelElementBase modelElement in modelElements)
                {
                    modelElement.StrictlyDoAfterReplication();
                }
            }
        }

        /// <summary>
        /// This method should be overridden by derived classes that need additional logic to be performed after a replication. 
        /// </summary>
        protected virtual void DoAfterReplication()
        {
        }

        /// <summary>
        /// The warm-up length of the model element, the default warm-up length is zero.
        /// A warm-up length of zero implies that the model element allowes its parent to call its warm-up action. 
        /// </summary>
        private double LengthOfWarmUp => GetSimulation.MyExperiment.LengthOfWarmUp;

        private void HandleEndWarmUp(CSSLEvent e)
        {
            ObserverState = ModelElementObserverState.INITIALIZED;
            NotifyObservers(this);
            ObserverState = ModelElementObserverState.UPDATE;
        }

        private List<IObserver<object>> observers;

        public IDisposable Subscribe(IObserver<object> observer)
        {
            // Check if observer is permitted.
            try
            {
                ModelElementObserverBase modelElementObserver = (ModelElementObserverBase)observer;
            }
            catch
            {
                throw new Exception($"Tried to attach an observer of class {observer.GetType().Name} of the wrong type to {Name}");
            }

            // Check whether observer is already registered. If not, add it.
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber(observers, observer, this);
        }

        protected void NotifyObservers(object info)
        {
            foreach (ObserverBase observer in observers)
            {
                observer.OnNext(info);
            }
        }
    }
}
