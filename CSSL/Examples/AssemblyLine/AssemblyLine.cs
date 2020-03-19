using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AssemblyLine
{
    public class AssemblyLine : ModelElementBase
    {
        public AssemblyLine(ModelElementBase parent, string name, int length) : base(parent, name)
        {
            machines = new Machine[length];
            buffers = new Buffer[length];
        }

        private int length => machines.Length;

        private Machine[] machines;

        private Buffer[] buffers;

        public void AddMachine(int index, double maxSpeed, Distribution upTimeDistribution, Distribution downTimeDistribution)
        {
            machines[index] = new Machine(this, $"Machine_{index}", maxSpeed, upTimeDistribution, downTimeDistribution);
        }

        public void AddBuffer(int index, double capacity)
        {
            if (index == 0)
            {
                throw new Exception("There is no zeroth buffer.");
            }
            else
            {
                buffers[index] = new Buffer(this, $"Buffer_{index}") { Capacity = capacity };
            }
        }

        protected override void DoBeforeExperiment()
        {
            base.DoBeforeExperiment();
            
            // Check if all machines and buffers are defined. 
            for (int i = 0; i < length; i++)
            {
                if (machines[i] == null)
                {
                    throw new Exception($"Machine at position {i} in the assembly line is not defined.");
                }
                if (buffers[i] == null && i > 0) // There is no zeroth buffer.
                {
                    throw new Exception($"Buffer at position {i} in the assembly line is not defined.");
                }
            }
        }

        protected override void DoBeforeReplication()
        {
            base.DoBeforeReplication();
            
            // The first machine starts at maximum speed.
            machines[0].ActualSpeed = machines[0].MaxSpeed;

            for (int i = 1; i < length; i++)
            {
                // Downstream machines start running at the speed of the upstream machine, or its maximum speed.
                machines[i].ActualSpeed = Math.Min(machines[i].MaxSpeed, machines[i - 1].ActualSpeed);

                // Start with empty buffers.
                buffers[i].Content = 0;

                buffers[i].NetSpeed = machines[i - 1].ActualSpeed - machines[i].ActualSpeed;

            }
        }
    }
}
