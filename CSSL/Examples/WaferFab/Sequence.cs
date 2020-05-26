using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class Sequence
    {
        public Sequence(LotType type, List<LotStep> lotSteps)
        {
            Type = type;
            this.lotSteps = lotSteps;
        }

        public Sequence(LotType type)
        {
            Type = type;
            lotSteps = new List<LotStep>();
        }

        private List<LotStep> lotSteps { get; set; }

        public int stepCount => lotSteps.Count;

        public LotType Type { get; }

        public bool HasNextStep(int currentStepCount)
        {
            return currentStepCount + 1 < lotSteps.Count;
        }

        public LotStep GetCurrentStep(int currentStepCount)
        {
            return lotSteps[currentStepCount];
        }

        public LotStep GetNextStep(int currentStepCount)
        {
            if (HasNextStep(currentStepCount))
            {
                return lotSteps[currentStepCount + 1];
            }
            else
            {
                return null;
            }
        }

        public WorkCenter GetCurrentWorkCenter(int currentStepCount)
        {
            return GetCurrentStep(currentStepCount).WorkCenter;
        }

        public WorkCenter GetNextWorkCenter(int currentStepCount)
        {
            if (HasNextStep(currentStepCount))
            {
                return GetNextStep(currentStepCount).WorkCenter;
            }
            else
            {
                return null;
            }
        }
               
        public void AddStep(LotStep lotstep)
        {
            lotSteps.Add(lotstep);
        }


    }
}
