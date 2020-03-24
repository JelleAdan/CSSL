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
    public abstract class ModelElementObserverBase : ObserverBase
    {
        public ModelElementObserverBase(Simulation mySimulation) : base(mySimulation)
        {
        }

        public override void OnNext(object info)
        {
            ModelElementBase modelElement = (ModelElementBase)info;

            switch (modelElement.ObserverState)
            {
                case ModelElementObserverState.BEFORE_EXPERIMENT:
                    OnExperimentStart(modelElement);
                    break;
                case ModelElementObserverState.BEFORE_REPLICATION:
                    OnReplicationStart(modelElement);
                    break;
                case ModelElementObserverState.WARMUP:
                    OnWarmUp(modelElement);
                    break;
                case ModelElementObserverState.INITIALIZED:
                    OnInitialized(modelElement);
                    break;
                case ModelElementObserverState.UPDATE:
                    OnUpdate(modelElement);
                    break;
                case ModelElementObserverState.AFTER_REPLICATION:
                    OnReplicationEnd(modelElement);
                    break;
                case ModelElementObserverState.AFTER_EXPERIMENT:
                    OnExperimentEnd(modelElement);
                    break;
            }
        }

        protected abstract void OnExperimentStart(ModelElementBase modelElement);

        protected abstract void OnReplicationStart(ModelElementBase modelElement);

        protected abstract void OnWarmUp(ModelElementBase modelElement);

        protected abstract void OnInitialized(ModelElementBase modelElement);

        protected abstract void OnUpdate(ModelElementBase modelElement);

        protected abstract void OnReplicationEnd(ModelElementBase modelElement);

        protected abstract void OnExperimentEnd(ModelElementBase modelElement);
    }
}
