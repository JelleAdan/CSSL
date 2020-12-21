using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    [Serializable]
    public class Sequence
    {
        public Sequence(string productType, string productGroup, List<LotStep> lotSteps)
        {
            ProductType = productType;
            ProductGroup = productGroup;
            this.lotSteps = lotSteps;
        }

        public Sequence(string productType, string productGroup)
        {
            ProductType = productType;
            ProductGroup = productGroup;
            this.lotSteps = new List<LotStep>();
        }

        public Sequence (LotStep lotStep)
        {
            ProductGroup = "SingleStep_" + lotStep.Name;
            ProductType = "SingleStep_" + lotStep.Name;
            this.lotSteps = new List<LotStep>();
            lotSteps.Add(lotStep);
        }

        public string ProductGroup { get; }

        public string ProductType { get; 
        }
        private List<LotStep> lotSteps { get; set; }

        public int stepCount => lotSteps.Count;


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
