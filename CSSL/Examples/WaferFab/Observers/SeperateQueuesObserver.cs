using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab.Observers
{
    public class SeperateQueuesObserver : ModelElementObserverBase
    {
        public SeperateQueuesObserver(Simulation mySimulation, WorkCenter workCenter, string name) : base(mySimulation, name)
        {
            queueLengths = new Dictionary<LotStep, Variable<int>>();
            queueLengthsStatistic = new Dictionary<LotStep, WeightedStatistic>();

            foreach (LotStep step in workCenter.LotSteps)
            {
                queueLengths.Add(step, new Variable<int>(this));
                queueLengthsStatistic.Add(step, new WeightedStatistic("QueueLength_" + step.Name));
            }
        }

        private Dictionary<LotStep, Variable<int>> queueLengths;

        private Dictionary<LotStep, WeightedStatistic> queueLengthsStatistic;

        private void writeToFile(WorkCenter workCenter)
        {
            Writer.Write(workCenter.GetTime + "," + workCenter.GetWallClockTime + ",");

            foreach (LotStep step in workCenter.LotSteps)
            {
                Writer.Write(queueLengths[step].Value + ",");
            }
            Writer.Write("\n");
        }

        private void writeToConsole(WorkCenter workCenter)
        {
            Console.Write(workCenter.GetTime + "," + workCenter.GetWallClockTime + ",");

            foreach (LotStep step in workCenter.LotSteps)
            {
                Console.Write(queueLengths[step].Value + ",");
            }
            Console.Write("\n");
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            WorkCenter workCenter = (WorkCenter)modelElement;

            try
            {
                LotStep step = new LotStep();

                // Check whether this OnUpdate is triggered by Arrival or Departure event
                if (workCenter.IsArrivalFlag)
                {
                    step = workCenter.LastArrivedLot.GetCurrentStep;
                    //Console.WriteLine($"{GetTime} {this.Name} triggered by arrival of {step.Name} of lot {workCenter.LastArrivedLot.Id}.");
                }

                else
                {
                    step = workCenter.LotStepInService;
                    //Console.WriteLine($"{GetTime} {this.Name} triggered by departure of {step.Name} of lot {workCenter.Queues[step].PeekFirst().Id}");
                }

                queueLengths[step].UpdateValue(workCenter.Queues[step].Length);
                queueLengthsStatistic[step].Collect(queueLengths[step].PreviousValue, queueLengths[step].Weight);

                writeToFile(workCenter);
                //writeToConsole(workCenter);
            }
            catch
            {
                if (workCenter.IsArrivalFlag)
                {
                    Lot lot = workCenter.LastArrivedLot;

                    throw new Exception($"{workCenter.Name} has lot {lot.ProductType} in {lot.GetCurrentStep.Name}, should be in {lot.GetCurrentWorkCenter.Name}");
                }
                else
                {
                    LotStep step = workCenter.LotStepInService;
                    throw new Exception($"{workCenter.Name} has step {step.Name} in service, but this step belongs to workcenter {step.WorkCenter.Name}");
                }
            }
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        private void headerToFile(WorkCenter workCenter)
        {
            Writer.Write("Simulation Time,Computational Time,");

            foreach (LotStep step in workCenter.LotSteps)
            {
                queueLengths[step].UpdateValue(workCenter.Queues[step].Length);

                Writer.Write(step.Name + ",");
            }
            Writer.Write("\n");
        }

        private void headerToConsole(WorkCenter workCenter)
        {
            Console.Write("Simulation Time,Computational Time,");

            foreach (LotStep step in workCenter.LotSteps)
            {
                queueLengths[step].UpdateValue(workCenter.Queues[step].Length);

                Console.Write(step.Name + ",");
            }
            Console.Write("\n");
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            WorkCenter workCenter = (WorkCenter)modelElement;

            //headerToConsole(workCenter);
            headerToFile(workCenter);
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
            WorkCenter workCenter = (WorkCenter)modelElement;

            foreach (LotStep step in workCenter.LotSteps)
            {
                queueLengths[step].Reset();
                // Uncomment below if one want to save across replication statistics
                queueLengthsStatistic[step].Reset();
            }
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected override void OnExperimentStart(ModelElementBase modelElement)
        {
        }

        protected override void OnExperimentEnd(ModelElementBase modelElement)
        {
        }
    }
}

