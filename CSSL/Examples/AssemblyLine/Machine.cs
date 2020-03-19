using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AssemblyLine
{
    public class Machine : SchedulingElementBase
    {
        public Machine(ModelElementBase parent, string name, double maxSpeed, Distribution upTimeDistribution, Distribution downTimeDistribution) : base(parent, name)
        {
            MaxSpeed = maxSpeed;
            this.upTimeDistribution = upTimeDistribution;
            this.downTimeDistribution = downTimeDistribution;
        }

        private Distribution upTimeDistribution;

        private Distribution downTimeDistribution;

        public MachineState State;

        public double ActualSpeed;

        public double MaxSpeed { get; }

        private double residualTime;

        public double EventTime;

        protected override void DoBeforeReplication()
        {
            base.DoBeforeReplication();

            // All machines start in the up state. 
            State = MachineState.Up;
            EventTime = upTimeDistribution.Next();
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
