using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.Elements
{
    public class Model : ModelElement
    {
        public Model(string name, Simulation simulation) : base(name)
        {
            MySimulation = simulation;
        }

        public Simulation MySimulation { get; }

        public override double LengthOfWarmUp => MySimulation.MyExperiment.LengthOfWarmUp;

        protected sealed override Model GetModel => this;

        internal void Initialize()
        {
            StrictlyDoBeforeReplication();
            DoBeforeReplication();
            ScheduleEndEvent();
        }

        private void ScheduleEndEvent()
        {
            GetExecutive.ScheduleEvent(GetTime, GetExecutive.HandleEndEvent);
        }

        protected override void DoBeforeExperiment()
        {
            if (!HasChildren)
            {
                throw new Exception("The model does not contain any child model elements.");
            }
        }
    }
}
