using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Observer;
using CSSL.Utilities.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CSSL.Examples.WaferFab.Observers
{
    public class WaferFabObserver : ModelElementObserverBase
    {
        public WaferFabObserver(Simulation mySimulation, string name, WaferFab waferFab) : base(mySimulation, name)
        {
            queueLengths = new Dictionary<LotStep, Variable<int>>();
            queueLengthsStatistics = new Dictionary<LotStep, WeightedStatistic>();


            foreach (LotStep step in waferFab.LotSteps.Values.OrderBy(x => x.Id))
            {
                queueLengths.Add(step, new Variable<int>(this));
                queueLengthsStatistics.Add(step, new WeightedStatistic("QueueLength_" + step.Name));
            }

            orderedLotSteps = waferFab.LotSteps.Values.OrderBy(x => x.Id).ToList();
        }

        private Dictionary<LotStep, Variable<int>> queueLengths;

        private Dictionary<LotStep, WeightedStatistic> queueLengthsStatistics;

        private List<LotStep> orderedLotSteps;

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        protected override void OnExperimentStart(ModelElementBase modelElement)
        {
        }

        protected override void OnReplicationStart(ModelElementBase modelElement)
        {
            WaferFab waferFab = (WaferFab)modelElement;

            foreach (var queueLength in queueLengths.Values)
            {
                queueLength.Reset();
            }

            foreach (var queueLengthStatistic in queueLengthsStatistics.Values)
            {
                queueLengthStatistic.Reset();
            }

            headerToFile(waferFab);
        }
        protected override void OnWarmUp(ModelElementBase modelElement)
        {
        }
        protected override void OnInitialized(ModelElementBase modelElement)
        {
        }

        protected override void OnUpdate(ModelElementBase modelElement)
        {
            WaferFab waferFab = (WaferFab)modelElement;

            foreach (var workCenter in waferFab.WorkCenters.Values)
            {
                foreach (var step in workCenter.LotSteps)
                {
                    queueLengths[step].UpdateValue(workCenter.Queues[step].Length);
                    queueLengthsStatistics[step].Collect(queueLengths[step].PreviousValue, queueLengths[step].Weight);
                }
            }

            writeOutputToFile(waferFab);
            //writeOutputToConsole(waferFab);
        }

        protected override void OnReplicationEnd(ModelElementBase modelElement)
        {
        }
        protected override void OnExperimentEnd(ModelElementBase modelElement)
        {
        }


        private void headerToFile(WaferFab waferFab)
        {
            Writer.Write("Simulation Time,Computational Time,");

            foreach (LotStep step in waferFab.LotSteps.Values.OrderBy(x => x.Id))
            {
                Writer.Write($"{step.Name},");
            }

            Writer.Write("\n");
        }

        private void writeOutputToFile(WaferFab waferFab)
        {
            Writer.Write(waferFab.GetTime + "," + waferFab.GetWallClockTime + ",");

            foreach (LotStep step in orderedLotSteps)
            {
                Writer.Write(queueLengths[step].Value + ",");
            }
            Writer.Write("\n");
        }

        private void writeOutputToConsole(WaferFab waferFab)
        {
            Console.Write(waferFab.GetTime + "," + waferFab.GetWallClockTime + ",");

            foreach (LotStep step in orderedLotSteps)
            {
                Console.Write($"{step.Name} " + queueLengths[step].Value + ",");
            }
            Console.Write("\n");
        }
    }
}
