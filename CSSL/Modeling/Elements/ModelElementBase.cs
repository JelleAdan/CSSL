using CSSL.Observer;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public abstract class ModelElementBase : IIdentity, IName, IObservable<ModelElementBase>
    {
        /// <summary>
        /// Incremented to store the total number of created model elements.
        /// </summary>
        private static int modelElementCounter;

        public int Id { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// This constuctor called to construct any ModelElement.
        /// </summary>
        /// <param name="parent">A reference to the parent model element.</param>
        /// <param name="name">The name of the model element.</param>
        public ModelElementBase(ModelElementBase parent, string name)
        {
            ConstructorCalls(name);
            this.parent = parent ?? throw new ArgumentNullException($"Tried to construct ModelElement with name \"{name}\" but the parent ModelElement is null.");
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
            observers = new List<IObserver<ModelElementBase>>();
        }

        /// <summary>
        /// A reference to the parent model element.
        /// </summary>
        protected ModelElementBase parent;

        /// <summary>
        /// A reference to the overall model, the highest container for all model elements.
        /// </summary>
        public virtual Model MyModel { get; private set; }

        /// <summary>
        /// Retrieves the executive.
        /// </summary>
        /// <returns></returns>
        protected Executive GetExecutive => MyModel.MySimulation.MyExecutive;

        /// <summary>
        /// Retrieves the simulation.
        /// </summary>
        public Simulation GetSimulation => MyModel.MySimulation;

        /// <summary>
        /// Retrieves the current simulation time.
        /// </summary>
        /// <returns></returns>
        public double GetTime => GetExecutive.Time;

        /// <summary>
        /// 
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
            ModelElementBase oldParent = parent;
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
        public bool HasChildren => modelElements.Any();

        /// <summary>
        /// The observer state of the model element.
        /// </summary>
        public ModelElementObserverState ObserverState { get; private set; }

        /// <summary>
        /// This method contains logic to be performed prior to an experiment. 
        /// It is called once before the first replication. This method ensures that each contained model element has its StrictlyDoBeforeExperiment method called.
        /// It also calls the DoBeforeExperiment method which contains optional logic. 
        /// </summary>
        public void StrictlyDoBeforeExperiment()
        {
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
        /// This method contains logic to be performed prior to a replication. 
        /// It is called once before every replication. This method ensures that each contained model element has its StrictlyDoBeforeReplication method called.
        /// It also calls the DoBeforeReplication method which contains optional logic.
        /// </summary>
        public void StrictlyDoBeforeReplication()
        {
            if (LengthOfWarmUp > 0)
            {
                GetExecutive.ScheduleEvent(GetExecutive.Time, HandleEndWarmUp);
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
        /// The warm-up length of the model element, the default warm-up length is zero.
        /// A warm-up length of zero implies that the model element allowes its parent to call its warm-up action. 
        /// </summary>
        public virtual double LengthOfWarmUp { get; set; }

        private void HandleEndWarmUp(CSSLEvent e)
        {
            ObserverState = ModelElementObserverState.INITIALIZED;
            NotifyObservers(this);
            ObserverState = ModelElementObserverState.UPDATE;

            // Trigger the warm-up action in all children that allow.
            foreach (ModelElementBase modelElement in modelElements.Where(x => x.LengthOfWarmUp > 0))
            {
                modelElement.HandleEndWarmUp(e);
            }
        }

        private List<IObserver<ModelElementBase>> observers;

        public IDisposable Subscribe(IObserver<ModelElementBase> observer)
        {
            // Check whether observer is already registered. If not, add it.
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<IObserver<ModelElementBase>, ModelElementBase>(observers, observer, this);
        }

        protected void NotifyObservers(ModelElementBase info)
        {
            foreach (ModelElementObserverBase observer in observers)
            {
                observer.OnNext(info);
            }
        }
    }
}
