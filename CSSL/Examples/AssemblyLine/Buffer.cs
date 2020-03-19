using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AssemblyLine
{
    public class Buffer : SchedulingElementBase
    {
        public Buffer(ModelElementBase parent, string name) : base(parent, name)
        {
        }

        public BufferState State;

        public double EventTime;

        public double Capacity;

        public double Content;

        public double NetSpeed;

        protected override void DoBeforeReplication()
        {
            base.DoBeforeReplication();

            if (NetSpeed > 0)
            {
                EventTime = Capacity / NetSpeed;
                State = BufferState.Neutral;
            }
            else
            {
                EventTime = double.MaxValue;
                State = BufferState.Empty;
            }
        }
    }

    public enum BufferState : byte
    {
        Empty = 0,
        Neutral = 1,
        Full = 2
    }
}
