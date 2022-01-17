using CSSL.Modeling.Elements;
using CSSL.RL;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Modeling.Elements
{
    public abstract class RLElementBase : SchedulingElementBase
    {
        protected RLLayerBase reinforcementLearningLayer;

        public RLElementBase(ModelElementBase parent, string name, RLLayerBase reinforcementLearningLayer) : base(parent, name)
        {
            this.reinforcementLearningLayer = reinforcementLearningLayer;
        }

        public abstract void Act(int action);

        public abstract bool TryAct(int action);

        protected void GetAction(double[] state, double reward)
        {
            reinforcementLearningLayer.GetAction(this, state, reward);
            SchedulePauseEvent();
        }

        protected void GetAction(double[][] state, double reward)
        {
            reinforcementLearningLayer.GetAction(this, state, reward);
            SchedulePauseEvent();
        }

        protected void SendFinalResponse(double[] state, double reward)
        {
            reinforcementLearningLayer.GetAction(this, state, reward);
            ScheduleEndEventNow();
        }

        protected void SendFinalResponse(double[][] state, double reward)
        {
            reinforcementLearningLayer.GetAction(this, state, reward);
            ScheduleEndEventNow();
        }

        protected void SchedulePauseEvent()
        {
            GetExecutive.SchedulePauseEventNow();
        }
    }
}
