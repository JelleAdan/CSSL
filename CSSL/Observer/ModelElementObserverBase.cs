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

        public ModelElementObserverBase(Simulation mySimulation, string name) : base(mySimulation, name)
        {
        }

        public sealed override void OnNext(object info)
        {
            ModelElementBase modelElement = (ModelElementBase)info;

            switch (modelElement.ObserverState)
            {
                case ModelElementObserverState.EXPERIMENT_START:
                    StrictlyOnExperimentStart();
                    OnExperimentStart(modelElement);
                    break;
                case ModelElementObserverState.REPLICATION_START:
                    StrictlyOnReplicationStart();
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
                case ModelElementObserverState.REPLICATION_END:
                    StrictlyOnReplicationEnd();
                    OnReplicationEnd(modelElement);
                    break;
                case ModelElementObserverState.EXPERIMENT_END:
                    StrictlyOnExperimentEnd();
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
