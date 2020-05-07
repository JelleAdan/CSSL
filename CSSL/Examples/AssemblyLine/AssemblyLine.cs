using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Examples.AssemblyLine
{
    public class AssemblyLine : SchedulingElementBase
    {
        /// <summary>
        /// Container for all elements (i.e. machines and buffers) in the assembly line. 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="length">The length (i.e. number of machines) in the assembly line.</param>
        public AssemblyLine(ModelElementBase parent, string name, int length) : base(parent, name)
        {
            Machines = new Machine[length];
            Buffers = new Buffer[length];
        }

        public int Length => Machines.Length;

        public Machine[] Machines { get; private set; }

        public Buffer[] Buffers { get; private set; }

        public void AddMachine(int index, double maxSpeed, Distribution upTimeDistribution, Distribution downTimeDistribution)
        {
            Machines[index] = new Machine(this, $"Machine_{index}", index, maxSpeed, upTimeDistribution, downTimeDistribution);
        }

        public void AddBuffer(int index, double capacity)
        {
            if (index == 0)
            {
                throw new Exception("There is no zeroth buffer.");
            }
            else
            {
                Buffers[index] = new Buffer(this, $"Buffer_{index}", index, capacity);
            }
        }

        protected override void OnExperimentStart()
        {
            base.OnExperimentStart();

            for (int i = 0; i < Length; i++)
            {
                if (Machines[i] == null)
                {
                    throw new Exception($"Machine at position {i} in the assembly line is not defined.");
                }
                if (Buffers[i] == null && i > 0) // There is no zeroth buffer.
                {
                    throw new Exception($"Buffer at position {i} in the assembly line is not defined.");
                }
            }
        }

        protected override void OnReplicationStart()
        {
            Machines[0].ActualSpeed = Machines[0].MaxSpeed;

            for (int i = 1; i < Length; i++)
            {
                Machines[i].ActualSpeed = Math.Min(Machines[i].MaxSpeed, Machines[i - 1].ActualSpeed);

                Buffers[i].NetSpeed = Machines[i - 1].ActualSpeed - Machines[i].ActualSpeed;
            }

            ScheduleEvent(0, ScheduleFirstEvent);
        }

        public void ScheduleFirstEvent(CSSLEvent e)
        {
            ScheduleEvent(NextEventTime(), HandleEvent);
        }

        private void UpdateBufferContents()
        {
            foreach (Buffer buffer in Buffers.Skip(1))
            {
                buffer.Content += buffer.NetSpeed * (GetTime - GetPreviousEventTime);
                //if (buffer.Content < 0 || buffer.Content > buffer.Capacity)
                //{
                //    Console.WriteLine(buffer.Content);
                //    buffer.Content = Math.Min(Math.Max(0, buffer.Content), buffer.Capacity);
                //}
            }
        }

        private double NextEventTime()
        {
            double nextEventTime = double.MaxValue;

            foreach (Machine machine in Machines)
            {
                if (machine.EventTime < nextEventTime)
                {
                    nextEventTime = machine.EventTime;
                }
            }

            foreach (Buffer buffer in Buffers.Skip(1))
            {
                if (buffer.EventTime < nextEventTime)
                {
                    nextEventTime = buffer.EventTime;
                }
            }

            return nextEventTime;
        }

        public void HandleEvent(CSSLEvent e)
        {
            int machineIndex = 0;

            for (int i = 0; i < Length; i++)
            {
                if (Machines[i].EventTime < Machines[machineIndex].EventTime)
                {
                    machineIndex = i;
                }
            }

            int bufferIndex = 1;

            for (int i = 1; i < Length; i++)
            {
                if (Buffers[i].EventTime < Buffers[bufferIndex].EventTime)
                {
                    bufferIndex = i;
                }
            }

            NotifyObservers(this);

            UpdateBufferContents();

            if (Machines[machineIndex].EventTime < Buffers[bufferIndex].EventTime)
            {
                Machines[machineIndex].HandleMachineEvent();
            }
            else
            {
                Buffers[bufferIndex].HandleBufferEvent();
            }

            ScheduleEvent(NextEventTime(), HandleEvent);
        }
    }
}
