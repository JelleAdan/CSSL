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
            Writer.Write(workCenter.GetTime + "\t" + workCenter.GetWallClockTime + "\t");

            foreach (LotStep step in workCenter.LotSteps)
            {
                Writer.Write(queueLengths[step].Value + "\t");
            }
            Writer.Write("\n");
        }

        private void writeToConsole(WorkCenter workCenter)
        {
            Console.Write(workCenter.GetTime + "\t" + workCenter.GetWallClockTime + "\t");

            foreach (LotStep step in workCenter.LotSteps)
            {
                Console.Write(queueLengths[step].Value + "\t");
            }
            Console.Write("\n");
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            WorkCenter workCenter = (WorkCenter)modelElement;
            LotStep step = workCenter.LastArrivedLot.GetCurrentStep;

            queueLengths[step].UpdateValue(workCenter.Queues[step].Length);
            queueLengthsStatistic[step].Collect(queueLengths[step].PreviousValue, queueLengths[step].Weight);

            writeToFile(workCenter);
            //writeToConsole(workCenter);
        }

        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }

        protected override void OnInitialized(ModelElementBase modelElement)
        {
            //Writer.Write("Simulation Time\tComputational Time\t");
            //Console.Write("Simulation Time\tComputational Time\t");

            //WorkCenter workCenter = (WorkCenter)modelElement;

            //foreach (LotStep step in workCenter.LotSteps)
            //{
            //    queueLengths[step].UpdateValue(workCenter.Queues[step].Length);

            //    Writer.Write(step.Name + "\t");
            //    Console.Write(step.Name + "\t");
            //}

            //Writer.Write("\n");
            //Console.Write("\n");
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

