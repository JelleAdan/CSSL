using CSSL.Modeling.CSSLQueue;
using WaferFabSim.SnapshotData;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class Lot : CSSLQueueObject<Lot>
    {
        public readonly LotType LotType;

        public Sequence Sequence { get; }

        public int CurrentStepCount { get; private set; }

        public double EndTime { get; private set; }

        public int? PlanDay { get; internal set; }

        public int ClipWeek { get; internal set; }

        public string LotID { get; internal set; }

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

                CurrentStepCount++;

                nextWorkCenter.HandleArrival(this);
            }
            else
            {
                EndTime = GetCurrentWorkCenter.GetTime;
            }
        }

        public void SetCurrentStepCount(int i)
        {
            CurrentStepCount = i;
        }

        public Lot(double creationTime, Sequence sequence) : base(creationTime)
        {
            CurrentStepCount = -1;

            LotType = sequence.Type;

            Sequence = sequence;
        }


        public Lot(Lot lotToDeepCopy)
        {
            LotType = lotToDeepCopy.LotType;
            Sequence = lotToDeepCopy.Sequence;
            CurrentStepCount = lotToDeepCopy.CurrentStepCount;
            EndTime = lotToDeepCopy.EndTime;
        }
    }
}
