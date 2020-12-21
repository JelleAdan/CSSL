using CSSL.Modeling.CSSLQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    [Serializable]
    public class Lot : CSSLQueueObject<Lot>
    {
        public string LotID { get; set; }

        public string ProductType => Sequence.ProductType;

        public string ProductGroup => Sequence.ProductGroup;

        public Sequence Sequence { get; }

        /// <summary>
        /// Lot steps from 0 to number of steps. Initialized on -1 before released in fab.
        /// </summary>
        /// <param name="currentStepCount"></param>
        /// <returns></returns>
        public int CurrentStepCount { get; private set; }

        /// <summary>
        /// Simulation time which the lot got released in the fab.
        /// </summary>
        public double StartTime { get; private set; }
        public double EndTime { get; private set; }

        /// <summary>
        /// Temporary used for WaferAreaSim
        /// </summary>
        public int WIPIn { get; set; }

        #region Data from original real lot activity
        /// <summary>
        /// Arrival time stamp of original lot activity in workcenter. Used for initial lots, for non-initial lot this is null.
        /// </summary>
        public DateTime? ArrivalReal { get; set; }

        /// <summary>
        /// Departure time stamp of original lot activity in workcenter. Used for initial lots, for non-initial lot this is null.
        /// </summary>
        public DateTime? DepartureReal { get; set; }

        /// <summary>
        /// Overtaken lots of original Lotactivity in workcenter
        /// </summary>
        public int OvertakenLotsReal { get; set; }

        /// <summary>
        /// WIP right before arrival of original lot activity in workcenter. Used for initial lots, for non-initial lot this is -1.
        /// </summary>
        public int WIPInReal { get; set; } = -1;

        /// <summary>
        /// Cycle time of original real lot activity
        /// </summary>
        public double CycleTimeReal
        {
            get
            {
                if (DepartureReal != null && ArrivalReal != null)
                {
                    TimeSpan cycle = (TimeSpan)(DepartureReal - ArrivalReal);

                    return cycle.TotalSeconds;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int? PlanDayReal { get; set; }

        public int ClipWeekReal { get; set; }
        #endregion

        public WorkCenter GetCurrentWorkCenter => Sequence.GetCurrentWorkCenter(CurrentStepCount);

        public WorkCenter GetNextWorkCenter => Sequence.GetNextWorkCenter(CurrentStepCount);

        public LotStep GetCurrentStep => Sequence.GetCurrentStep(CurrentStepCount);

        public LotStep GetNextStep => Sequence.GetNextStep(CurrentStepCount);

        public bool HasNextStep => Sequence.HasNextStep(CurrentStepCount);

        public void SendToNextWorkCenter()
        {
            // If has next step, send to next work station. Otherwise, do nothing and lot will dissapear from system.
            if (Sequence.HasNextStep(CurrentStepCount))
            {
                WorkCenter nextWorkCenter = GetNextWorkCenter;

                CurrentStepCount++;

                if (CurrentStepCount == 0)
                { // Means it is the first step, it is now released in fab so start time can be saved.
                    StartTime = nextWorkCenter.GetTime;
                }

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

            Sequence = sequence;
        }

        /// <summary>
        /// Deep copy lot, such that original initial lot does not change its current step count in a replication.
        /// </summary>
        /// <param name="lotToDeepCopy">Original lot to make deep copy of.</param>
        public Lot(Lot lotToDeepCopy)
        {
            LotID = lotToDeepCopy.LotID;
            Sequence = lotToDeepCopy.Sequence;
            CurrentStepCount = lotToDeepCopy.CurrentStepCount;
            StartTime = lotToDeepCopy.StartTime;
            EndTime = lotToDeepCopy.EndTime;
            ArrivalReal = lotToDeepCopy.ArrivalReal;
            DepartureReal = lotToDeepCopy.DepartureReal;
            WIPInReal = lotToDeepCopy.WIPInReal;
            OvertakenLotsReal = lotToDeepCopy.OvertakenLotsReal;
            PlanDayReal = lotToDeepCopy.PlanDayReal;
            ClipWeekReal = lotToDeepCopy.ClipWeekReal;
        }

        public Lot()
        {

        }
    }
}
