using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Examples.AssemblyLine.Observers
{
    public class AssemblyLineObserver : ModelElementObserverBase
    {
        public AssemblyLineObserver(Simulation mySimulation) : base(mySimulation)
        {
        }

        private double totalProduction;

        private Variable<double>[] bufferContents;

        private WeightedStatistic[] bufferContentStatistics;

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected override void OnExperimentStart(ModelElementBase modelElement)
        {
            AssemblyLine assemblyLine = (AssemblyLine)modelElement;
            bufferContentStatistics = new WeightedStatistic[assemblyLine.Length];
            bufferContents = new Variable<double>[assemblyLine.Length];
            foreach (Buffer buffer in assemblyLine.Buffers.Skip(1))
            {
                bufferContentStatistics[buffer.Index] = new WeightedStatistic($"{buffer.Name}_content");
                bufferContents[buffer.Index] = new Variable<double>(this);
            }
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            totalProduction = 0;

            foreach (var bufferContent in bufferContents.Skip(1))
            {
                bufferContent.Reset();
            }
            foreach (var bufferContentStatistic in bufferContentStatistics.Skip(1))
            {
                bufferContentStatistic.Reset();
            }
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            AssemblyLine assemblyLine = (AssemblyLine)modelElement;

            // Accumulate the total production

            Machine lastMachine = assemblyLine.Machines[assemblyLine.Length - 1];

            if (lastMachine.State == MachineState.Up)
            {
                totalProduction += lastMachine.ActualSpeed * (assemblyLine.GetTime - assemblyLine.GetPreviousEventTime);
            }

            // Register buffer contents

            foreach (Buffer buffer in assemblyLine.Buffers.Skip(1))
            {
                bufferContents[buffer.Index].UpdateValue(buffer.Content);
                double weight = bufferContents[buffer.Index].TimeOfUpdate - bufferContents[buffer.Index].PreviousTimeOfUpdate;
                bufferContentStatistics[buffer.Index].Collect(buffer.Content, weight);
            }
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
            AssemblyLine assemblyLine = (AssemblyLine)modelElement;

            Console.WriteLine($"Throughput is {totalProduction / assemblyLine.GetTime}");

            foreach (Buffer buffer in assemblyLine.Buffers.Skip(1))
            {
                Console.WriteLine($"Mean content {buffer.Name}: {bufferContentStatistics[buffer.Index].Average()}");
            }
        }

        protected override void OnExperimentEnd(ModelElementBase modelElement)
        {
        }
    }
}
