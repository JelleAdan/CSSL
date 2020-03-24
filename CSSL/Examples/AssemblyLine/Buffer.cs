using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AssemblyLine
{
    public class Buffer : ModelElementBase
    {
        /// <summary>
        /// Construct a buffer.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="index">Position of the buffer in the assembly line.</param>
        /// <param name="capacity">The maximum capacity of the buffer.</param>
        public Buffer(ModelElementBase parent, string name, int index, double capacity) : base(parent, name)
        {
            try
            {
                assemblyLine = (AssemblyLine)parent;
            }
            catch
            {
                throw new Exception("Buffer must be a constituent of an assembly line.");
            }
            Index = index;
            Capacity = capacity;
        }

        private AssemblyLine assemblyLine;

        public int Index { get; }

        public double Capacity { get; }

        public BufferState State { get; set; }

        public double Content { get; set; }

        public double NetSpeed { get; set; }

        public double EventTime { get; set; }

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

        public void HandleBufferEvent()
        {
            Buffer[] Buffers = assemblyLine.Buffers;
            Machine[] Machines = assemblyLine.Machines;

            int i;

            Buffer b;
            Machine m;

            if (NetSpeed < 0)
            {
                State = BufferState.Empty;
            }
            else if (NetSpeed > 0)
            {
                State = BufferState.Full;
            }

            NetSpeed = 0;

            EventTime = double.MaxValue;

            if (State == BufferState.Empty)
            {
                for (i = Index; i < assemblyLine.Length && Buffers[i].State == BufferState.Empty; i++)
                {
                    m = Machines[i];

                    m.ActualSpeed = Machines[i - 1].ActualSpeed;

                    if (m.ActualSpeed <= 0)
                    {
                        m.ResidualTime = m.EventTime - GetTime;

                        m.State = MachineState.Starved;

                        m.EventTime = double.MaxValue;
                    }
                }
                if (i < assemblyLine.Length)
                {
                    b = Buffers[i];

                    if (b.State == BufferState.Full)
                    {
                        if (Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed >= 0)
                        {
                            b.NetSpeed = 0;
                        }
                        else
                        {
                            b.NetSpeed = Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed;

                            b.EventTime = GetTime - b.Content / b.NetSpeed;

                            b.State = BufferState.Neutral;
                        }
                    }
                    else
                    {
                        b.NetSpeed = Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed;

                        if (b.NetSpeed < 0)
                        {
                            b.EventTime = GetTime - b.Content / b.NetSpeed;
                        }
                        else if (b.NetSpeed > 0)
                        {
                            b.EventTime = GetTime + (b.Capacity - b.Content) / b.NetSpeed;
                        }
                        else
                        {
                            b.EventTime = double.MaxValue;
                        }
                    }
                }
            }
            else if (State == BufferState.Full)
            {
                for (i = Index; i > 0 && Buffers[i].State == BufferState.Full; i--)
                {
                    m = Machines[i - 1];

                    m.ActualSpeed = Machines[i].ActualSpeed;

                    if (m.ActualSpeed <= 0)
                    {
                        m.ResidualTime = m.EventTime - GetTime;

                        m.State = MachineState.Blocked;

                        m.EventTime = double.MaxValue;
                    }
                }
                if (i > 0)
                {
                    b = Buffers[i];

                    if (b.State == BufferState.Empty)
                    {
                        if (Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed <= 0)
                        {
                            b.NetSpeed = 0;
                        }
                        else
                        {
                            b.NetSpeed = Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed;

                            b.EventTime = GetTime + (b.Capacity - b.Content) / b.NetSpeed;

                            b.State = BufferState.Neutral;
                        }
                    }
                    else
                    {
                        b.NetSpeed = Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed;

                        if (b.NetSpeed < 0)
                        {
                            b.EventTime = GetTime - b.Content / b.NetSpeed;
                        }
                        else if (b.NetSpeed > 0)
                        {
                            b.EventTime = GetTime + (b.Capacity - b.Content) / b.NetSpeed;
                        }
                        else
                        {
                            b.EventTime = double.MaxValue;
                        }
                    }
                }
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
