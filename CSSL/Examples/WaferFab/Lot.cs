using CSSL.Modeling.CSSLQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class Lot : CSSLQueueObject<Lot>
    {
        private static int lotCount;

        public readonly LotType Type;

        public Sequence Sequence { get; }

        public int CurrentStepCount { get; set; }

        public double EndTime { get; private set; }

        public WorkCenter GetCurrentWorkCenter => Sequence.GetCurrentWorkCenter(CurrentStepCount);

        public WorkCenter GetNextWorkCenter => Sequence.GetNextWorkCenter(CurrentStepCount);

        public LotStep GetCurrentStep => Sequence.GetCurrentStep(CurrentStepCount);

        public LotStep GetNextStep => Sequence.GetNextStep(CurrentStepCount);

        public void SendToNextWorkCenter()
        {
            // If has next step, send to next work station. Otherwise, do nothing and lot will dissapear from system.
            if (Sequence.HasNextStep(CurrentStepCount))
            {
                WorkCenter nextWorkCenter = GetNextWorkCenter;

                nextWorkCenter.HandleArrival(this);

                CurrentStepCount++;
            }
            else
            {
                EndTime = GetCurrentWorkCenter.GetTime;
            }
        }

        public Lot(double creationTime, Sequence sequence) : base(creationTime, lotCount++)
        {
            CurrentStepCount = -1;

            Type = sequence.Type;

            Sequence = sequence;
        } 
    }
}
