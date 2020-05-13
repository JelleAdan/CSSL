using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Linq;

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

        private Statistic[] upTimesStatistics;
        private Statistic[] downTimesStatistics;
        private Statistic[] blockedTimesStatistics;
        private Statistic[] starvedTimesStatistics;

        private Statistic[] upStatistics;
        private Statistic[] downStatistics;
        private Statistic[] blockedStatistics;
        private Statistic[] starvedStatistics;

        private Statistic[] throughPutStatistics;

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

            throughPutStatistics = new Statistic[1];
            for (int i = 0; i < 1; i++)
            {
                throughPutStatistics[i] = new Statistic($"Throughput");
            }

            upStatistics = new Statistic[assemblyLine.Machines.Length];
            downStatistics = new Statistic[assemblyLine.Machines.Length];
            starvedStatistics = new Statistic[assemblyLine.Machines.Length];
            blockedStatistics = new Statistic[assemblyLine.Machines.Length];
            foreach (Machine machine in assemblyLine.Machines)
            {
                upStatistics[machine.Index] = new Statistic($"UpTime_{machine.Name}");
                downStatistics[machine.Index] = new Statistic($"DownTime_{machine.Name}");
                starvedStatistics[machine.Index] = new Statistic($"StarvedTime_{machine.Name}");
                blockedStatistics[machine.Index] = new Statistic($"BlockedTime_{machine.Name}");
            }

        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
            AssemblyLine assemblyLine = (AssemblyLine)modelElement;

            upTimesStatistics = new Statistic[assemblyLine.Length];
            downTimesStatistics = new Statistic[assemblyLine.Length];
            starvedTimesStatistics = new Statistic[assemblyLine.Length];
            blockedTimesStatistics = new Statistic[assemblyLine.Length];
            foreach (Machine machine in assemblyLine.Machines)
            {
                upTimesStatistics[machine.Index] = new Statistic($"{machine.Name}_uptime");
                downTimesStatistics[machine.Index] = new Statistic($"{machine.Name}_downtime");
                starvedTimesStatistics[machine.Index] = new Statistic($"{machine.Name}_starvedtime");
                blockedTimesStatistics[machine.Index] = new Statistic($"{machine.Name}_blockedtime");
            }
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

            // Register uptimes
            foreach (Machine machine in assemblyLine.Machines)
            {
                if (machine.State == MachineState.Up)
                {
                    upTimesStatistics[machine.Index].Collect(assemblyLine.GetTime - assemblyLine.GetPreviousEventTime);
                }
                if (machine.State == MachineState.Down)
                {
                    downTimesStatistics[machine.Index].Collect(assemblyLine.GetTime - assemblyLine.GetPreviousEventTime);
                }
                if (machine.State == MachineState.Starved)
                {
                    starvedTimesStatistics[machine.Index].Collect(assemblyLine.GetTime - assemblyLine.GetPreviousEventTime);
                }
                if (machine.State == MachineState.Blocked)
                {
                    blockedTimesStatistics[machine.Index].Collect(assemblyLine.GetTime - assemblyLine.GetPreviousEventTime);
                }
            }
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
            AssemblyLine assemblyLine = (AssemblyLine)modelElement;

            //Console.WriteLine($"Throughput is {totalProduction / assemblyLine.GetTime}");

            //foreach (Buffer buffer in assemblyLine.Buffers.Skip(1))
            //{
            //    Console.WriteLine($"Mean content {buffer.Name}: {bufferContentStatistics[buffer.Index].Average()}");
            //}

            //Console.WriteLine($"Throughput in (products/second) is {totalProduction / (MySimulation.MyExperiment.LengthOfReplication - MySimulation.MyExperiment.LengthOfWarmUp)}");


            throughPutStatistics[0].Collect(totalProduction / (MySimulation.MyExperiment.LengthOfReplication - MySimulation.MyExperiment.LengthOfWarmUp));

            foreach (Machine machine in assemblyLine.Machines)
            {
                upStatistics[machine.Index].Collect(upTimesStatistics[machine.Index].Sum() / (MySimulation.MyExperiment.LengthOfReplication - MySimulation.MyExperiment.LengthOfWarmUp));
                downStatistics[machine.Index].Collect(downTimesStatistics[machine.Index].Sum() / (MySimulation.MyExperiment.LengthOfReplication - MySimulation.MyExperiment.LengthOfWarmUp));
                starvedStatistics[machine.Index].Collect(starvedTimesStatistics[machine.Index].Sum() / (MySimulation.MyExperiment.LengthOfReplication - MySimulation.MyExperiment.LengthOfWarmUp));
                blockedStatistics[machine.Index].Collect(blockedTimesStatistics[machine.Index].Sum() / (MySimulation.MyExperiment.LengthOfReplication - MySimulation.MyExperiment.LengthOfWarmUp));
            }

        }

        protected override void OnExperimentEnd(ModelElementBase modelElement)
        {
            AssemblyLine assemblyLine = (AssemblyLine)modelElement;

            ExperimentWriter.WriteLine($"Throughput Statistics - Average - stdev - CI");
            ExperimentWriter.WriteLine($"{throughPutStatistics[0].Average()}");
            ExperimentWriter.WriteLine($"{throughPutStatistics[0].StandardDeviation()}");
            ExperimentWriter.WriteLine($"{throughPutStatistics[0].ConfidenceInterval95()}");

            ExperimentWriter.WriteLine($" ");
            ExperimentWriter.WriteLine($"Machine States - Up - Down- Starved - Blocked (1-2-3-4-5-6-7)");
            foreach (Machine machine in assemblyLine.Machines)
            {
                ExperimentWriter.WriteLine($"{upStatistics[machine.Index].Average()}");
                ExperimentWriter.WriteLine($"{downStatistics[machine.Index].Average()}");
                ExperimentWriter.WriteLine($"{starvedStatistics[machine.Index].Average()}");
                ExperimentWriter.WriteLine($"{blockedStatistics[machine.Index].Average()}");
                ExperimentWriter.WriteLine($" ");
            }
        }
    }
}
