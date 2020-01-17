using CSSL.Observer;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public abstract class ModelElement : IIdentity, IName, IObservable<ModelElement>
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
        public ModelElement(ModelElement parent, string name)
        {
            ConstructorCalls(name);
            this.parent = parent ?? throw new ArgumentNullException($"Tried to construct ModelElement with name \"{name}\" but the parent ModelElement is null.");
            parent.AddModelElement(this);
            myModel = parent.GetModel;
        }

        /// <summary>
        /// This constuctor is only called by Model so that Model does not require a parent model element.
        /// </summary>
        /// <param name="name">The name of the model.</param>
        public ModelElement(string name)
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
            modelElements = new List<ModelElement>();
            observers = new List<IObserver<ModelElement>>();
        }

        /// <summary>
        /// A reference to the parent model element.
        /// </summary>
        protected ModelElement parent;

        /// <summary>
        /// Retrieves the main model. The highest container for all model elements.
        /// </summary>
        /// <returns></returns>
        protected virtual Model GetModel => parent.GetModel;

        /// <summary>
        /// A reference to the overall model.
        /// </summary>
        protected Model myModel;

        /// <summary>
        /// Retrieves the executive.
        /// </summary>
        /// <returns></returns>
        protected Executive GetExecutive => myModel.MySimulation.MyExecutive;

        /// <summary>
        /// Retrieves the current simulation time.
        /// </summary>
        /// <returns></returns>
        public double GetTime => GetExecutive.Time;

        /// <summary>
        /// 
        /// </summary>
        private List<ModelElement> modelElements;

        /// <summary>
        /// Adds the supplied model element as a child to this model element. 
        /// </summary>
        /// <param name="modelElement">The model element to be added.</param>
        private void AddModelElement(ModelElement modelElement)
        {
            modelElements.Add(modelElement);
        }

        /// <summary>
        /// Removes the supplied child model element from this model element.
        /// </summary>
        /// <param name="modelElement">The model element to be removed.</param>
        private void RemoveModelElement(ModelElement modelElement)
        {
            modelElements.Remove(modelElement);
        }

        /// <summary>
        /// Changes the parent model element of this model element to the supplied model element. 
        /// </summary>
        /// <param name="newParent">The parent for this model element.</param>
        protected void ChangeParentModelElement(ModelElement newParent)
        {
            ModelElement oldParent = parent;
            if (oldParent != newParent)
            {
                oldParent.RemoveModelElement(this);
                newParent.AddModelElement(this);
                myModel = newParent.GetModel;
            }
        }


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

            if (!modelElements.Any())
            {
                foreach (ModelElement modelElement in modelElements)
                {
                    modelElement.StrictlyDoBeforeExperiment();
                    modelElement.DoBeforeExperiment();
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
                GetExecutive.ScheduleEvent(GetExecutive.Time, HandleWarmUp);
            }

            DoBeforeReplication();

            if (!modelElements.Any())
            {
                foreach (ModelElement modelElement in modelElements)
                {
                    modelElement.StrictlyDoBeforeReplication();
                    modelElement.DoBeforeReplication();
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

        private void HandleWarmUp(CSSLEvent e)
        {
            ObserverState = ModelElementObserverState.WARMUP;

            NotifyObservers(this);

            // Trigger the warm-up action in all children that allow.
            foreach (ModelElement modelElement in modelElements.Where(x => x.LengthOfWarmUp > 0))
            {
                modelElement.HandleWarmUp(e);
            }
        }

        private List<ModelElementObserverBase> observers;

        public IDisposable Subscribe(ModelElementObserverBase observer)
        {
            // Check whether observer is already registered. If not, add it.
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<ModelElementObserverBase>(observers, observer);
        }

        protected void NotifyObservers(ModelElement info)
        {
            foreach (ModelElementObserverBase observer in observers)
            {
                observer.OnNext(info);
            }
        }
    }
}
