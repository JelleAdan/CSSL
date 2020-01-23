using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Observer
{
    public abstract class ModelElementObserverBase : ObserverBase<ModelElementBase>
    {
        public ModelElementObserverBase(Simulation mySimulation) : base(mySimulation)
        {
        }

        public override void Subscribe(IObservable<ModelElementBase> observable)
        {
            cancellations.Add(observable.Subscribe(this));
        }

        public override void OnNext(ModelElementBase modelElement)
        {
            switch (modelElement.ObserverState)
            {
                case ModelElementObserverState.UPDATE:
                    OnUpdate(modelElement);
                    break;
                case ModelElementObserverState.WARMUP:
                    OnWarmUp(modelElement);
                    break;
                case ModelElementObserverState.INITIALIZED:
                    OnInitialized(modelElement);
                    break;
            }
        }

        protected abstract void OnUpdate(ModelElementBase modelElement);

        protected abstract void OnWarmUp(ModelElementBase modelElement);

        protected abstract void OnInitialized(ModelElementBase modelElement);
    }
}
