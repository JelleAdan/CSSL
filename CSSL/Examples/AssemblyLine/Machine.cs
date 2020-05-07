using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AssemblyLine
{
    public class Machine : ModelElementBase
    {
        /// <summary>
        /// Construct a machine.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="maxSpeed">The maximum production speed of the machine.</param>
        /// <param name="upTimeDistribution">Uptime distribution.</param>
        /// <param name="downTimeDistribution">Downtime distribution.</param>
        public Machine(ModelElementBase parent, string name, int index, double maxSpeed, Distribution upTimeDistribution, Distribution downTimeDistribution) : base(parent, name)
        {
            try
            {
                assemblyLine = (AssemblyLine)parent;
            }
            catch
            {
                throw new Exception("Machine must be a constituent of an assembly line.");
            }
            Index = index;
            MaxSpeed = maxSpeed;
            this.upTimeDistribution = upTimeDistribution;
            this.downTimeDistribution = downTimeDistribution;
        }

        private AssemblyLine assemblyLine;

        public int Index { get; }

        public double MaxSpeed { get; }

        private Distribution upTimeDistribution;

        private Distribution downTimeDistribution;

        public MachineState State { get; set; }

        public double ActualSpeed { get; set; }

        public double ResidualTime { get; set; }

        public double EventTime { get; set; }

        protected override void OnReplicationStart()
        {
            State = MachineState.Up;
            EventTime = upTimeDistribution.Next();
        }

        public void HandleMachineEvent()
        {
            Buffer[] Buffers = assemblyLine.Buffers;
            Machine[] Machines = assemblyLine.Machines;

            int i;

            Buffer b;
            Machine m;

            if (State == MachineState.Up)
            {
                State = MachineState.Down;

                EventTime = GetTime + downTimeDistribution.Next();

                ActualSpeed = 0;

                // Update upstream

                if (Index > 0) // If this machine is not the first in the line
                {
                    for (i = Index; i > 0 && Buffers[i].State == BufferState.Full; i--)
                    {
                        b = Buffers[i];

                        b.NetSpeed = 0;

                        b.EventTime = double.MaxValue;

                        m = Machines[i - 1];

                        // Check
                        if (m.State != MachineState.Up)
                        {
                            throw new Exception("Not possible.");
                        }

                        m.ActualSpeed = 0;

                        m.ResidualTime = m.EventTime - GetTime;

                        m.State = MachineState.Blocked;

                        m.EventTime = double.MaxValue;
                    }
                    if (i > 0)
                    {
                        b = Buffers[i];

                        b.NetSpeed = Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed;

                        if (b.NetSpeed > 0)
                        {
                            b.EventTime = GetTime + (b.Capacity - b.Content) / b.NetSpeed;

                            b.State = BufferState.Neutral;
                        }
                        else // The net speed is zero
                        {
                            b.EventTime = double.MaxValue;
                        }
                    }
                }

                // Update downstream

                if (Index < Machines.Length - 1) // If this machine is not the last in the line
                {
                    for (i = Index; i < Machines.Length - 1 && Buffers[i + 1].State == BufferState.Empty; i++)
                    {
                        b = Buffers[i + 1];

                        b.NetSpeed = 0;

                        b.EventTime = double.MaxValue;

                        m = Machines[i + 1];

                        // Check
                        if (m.State != MachineState.Up)
                        {
                            throw new Exception("Not possible.");
                        }

                        m.ActualSpeed = 0;

                        m.ResidualTime = m.EventTime - GetTime;

                        m.State = MachineState.Starved;

                        m.EventTime = double.MaxValue;
                    }
                    if (i < Machines.Length - 1)
                    {
                        b = Buffers[i + 1];

                        b.NetSpeed = Machines[i].ActualSpeed - Machines[i + 1].ActualSpeed;

                        if (b.NetSpeed < 0)
                        {
                            b.EventTime = GetTime - b.Content / b.NetSpeed;

                            b.State = BufferState.Neutral;
                        }
                        else // The net speed is zero
                        {
                            b.EventTime = double.MaxValue;
                        }
                    }
                }
            }
            else if (State == MachineState.Down)
            {
                if (Index > 0 && Buffers[Index].State == BufferState.Empty)
                {
                    State = MachineState.Starved;

                    ResidualTime = upTimeDistribution.Next();

                    EventTime = double.MaxValue;
                }
                else if (Index < Machines.Length - 1 && Buffers[Index + 1].State == BufferState.Full)
                {
                    State = MachineState.Blocked;

                    ResidualTime = upTimeDistribution.Next();

                    EventTime = double.MaxValue;
                }
                else
                {
                    State = MachineState.Up;

                    EventTime = GetTime + upTimeDistribution.Next();

                    ActualSpeed = MaxSpeed;

                    if (Index > 0)
                    {
                        // Update upstream

                        for (i = Index; i > 0 && Buffers[i].State == BufferState.Full; i--)
                        {
                            m = Machines[i - 1];

                            m.ActualSpeed = Math.Min(m.MaxSpeed, Machines[i].ActualSpeed);

                            m.State = MachineState.Up;

                            m.EventTime = GetTime + m.ResidualTime;

                            b = Buffers[i];

                            b.NetSpeed = m.ActualSpeed - Machines[i].ActualSpeed;

                            if (b.NetSpeed < 0)
                            {
                                b.EventTime = GetTime - b.Content / b.NetSpeed;

                                b.State = BufferState.Neutral;
                            }
                            else // Net speed is zero
                            {
                                b.EventTime = double.MaxValue;
                            }
                        }
                        if (i > 0)
                        {
                            b = Buffers[i];

                            b.NetSpeed = Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed;

                            if (b.NetSpeed > 0)
                            {
                                b.EventTime = GetTime + (b.Capacity - b.Content) / b.NetSpeed;
                            }
                            else if (b.NetSpeed < 0)
                            {
                                b.EventTime = GetTime - b.Content / b.NetSpeed;
                            }
                            else
                            {
                                b.EventTime = double.MaxValue;
                            }
                        }
                    }
                    if (Index < Machines.Length - 1)
                    {
                        // Update downstream

                        for (i = Index; i < Machines.Length - 1 && Buffers[i + 1].State == BufferState.Empty; i++)
                        {
                            m = Machines[i + 1];

                            m.ActualSpeed = Math.Min(Machines[i].ActualSpeed, m.MaxSpeed);

                            m.State = MachineState.Up;

                            m.EventTime = GetTime + m.ResidualTime;

                            b = Buffers[i + 1];

                            b.NetSpeed = Machines[i].ActualSpeed - m.ActualSpeed;

                            if (b.NetSpeed > 0)
                            {
                                b.EventTime = GetTime + b.Capacity / b.NetSpeed;

                                b.State = BufferState.Neutral;
                            }
                            else
                            {
                                b.EventTime = double.MaxValue;
                            }
                        }
                        if (i < Machines.Length - 1)
                        {
                            b = Buffers[i + 1];

                            b.NetSpeed = Machines[i].ActualSpeed - Machines[i + 1].ActualSpeed;

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
    }

    public enum MachineState : byte
    {
        Up = 0,
        Down = 1,
        Starved = 2,
        Blocked = 3
    }
}
