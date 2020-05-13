using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class WaferFab : EventGeneratorBase
    {
        public LotGenerator LotGenerator { get; private set; }

        public Dictionary<string, WorkCenter> WorkCenters { get; private set; }

        public Dictionary<LotType, Sequence> Sequences { get; private set; }

        public Dictionary<string, LotStep> LotSteps { get; set; }

        public Dictionary<LotType, int> LotStarts { get; set; }

        public WaferFab(ModelElementBase parent, string name, ConstantDistribution samplingDistribution) : base(parent, name, samplingDistribution)
        {
            WorkCenters = new Dictionary<string, WorkCenter>();
            Sequences = new Dictionary<LotType, Sequence>();
            LotSteps = new Dictionary<string, LotStep>();
            LotStarts = new Dictionary<LotType, int>();
        }

        public void SetLotGenerator(LotGenerator lotGenerator)
        {
            LotGenerator = lotGenerator;
        }

        public void AddWorkCenter(string name, WorkCenter workCenter)
        {
            WorkCenters.Add(name, workCenter);
        }

        public void AddSequence(LotType lotType, Sequence sequence)
        {
            Sequences.Add(lotType, sequence);
        }

        public void AddLotStart(LotType lotType, int quantity)
        {
            LotStarts.Add(lotType, quantity);
        }

        /// <summary>
        /// This is created to read out data on a sampled interval.
        /// </summary>
        /// <param name="e"></param>
        protected override void HandleGeneration(CSSLEvent e)
        {
            NotifyObservers(this);

            ScheduleEvent(NextEventTime(), HandleGeneration);
        }
    }
}
