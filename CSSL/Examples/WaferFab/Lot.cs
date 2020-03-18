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

        public WorkCenter GetCurrentWorkCenter => Sequence.GetCurrentWorkCenter(CurrentStepCount);

        public WorkCenter GetNextWorkCenter => Sequence.GetNextWorkCenter(CurrentStepCount);

        public Lot(double creationTime, Sequence sequence) : base(creationTime, lotCount++)
        {
            Type = sequence.Type;

            Sequence = sequence;
        } 
    }
}
