using CSSL.Examples.WaferFab.Utilities;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    [Serializable]
    public class WaferFab : EventGeneratorBase, IGetDateTime
    {
        public DateTime InitialDateTime { get; }

        public LotGenerator LotGenerator { get; private set; }

        public Dictionary<string, WorkCenter> WorkCenters { get; private set; }

        public Dictionary<string, Sequence> Sequences { get; private set; }

        public Dictionary<string, LotStep> LotSteps { get; set; }

        public Dictionary<string, int> ManualLotStarts { get; set; }

        public List<Tuple<DateTime, Lot>> LotStarts { get; set; }

        /// <summary>
        /// Note: the ordering of these lists is used by some dispatchers to determine initial queue
        /// </summary>
        public List<Lot> InitialLots { get; set; }

        public WaferFab(ModelElementBase parent, string name, ConstantDistribution samplingDistribution, DateTime? initialTime = null) : base(parent, name, samplingDistribution)
        {
            WorkCenters = new Dictionary<string, WorkCenter>();
            Sequences = new Dictionary<string, Sequence>();
            LotSteps = new Dictionary<string, LotStep>();
            ManualLotStarts = new Dictionary<string, int>();
            InitialLots = new List<Lot>();
            InitialDateTime = initialTime == null ? DateTime.Now : (DateTime)initialTime;
        }

        public DateTime GetDateTime => InitialDateTime + new TimeSpan (0,0,(int)GetTime);

        public void SetLotGenerator(LotGenerator lotGenerator)
        {
            LotGenerator = lotGenerator;
        }

        public void AddWorkCenter(string name, WorkCenter workCenter)
        {
            WorkCenters.Add(name, workCenter);
        }

        public void AddSequence(string lotType, Sequence sequence)
        {
            Sequences.Add(lotType, sequence);
        }

        public void AddLotStart(string lotType, int quantity)
        {
            ManualLotStarts.Add(lotType, quantity);
        }

        /// <summary>
        /// This is created to read out data on a sampled interval.
        /// </summary>
        /// <param name="e"></param>
        protected override void HandleGeneration(CSSLEvent e)
        {
            Console.WriteLine($"Date: {GetDateTime}");

            NotifyObservers(this);

            ScheduleEvent(NextEventTime(), HandleGeneration);
        }

        /// <summary>
        /// This reads out the data on t = 0;
        /// </summary>
        /// <param name="e"></param>
        private void HandleFirstSnapshot(CSSLEvent e)
        {
              NotifyObservers(this);
        }

        protected override void OnReplicationStart()
        {
            base.OnReplicationStart();

            ScheduleEvent(0, HandleFirstSnapshot);
        }
    }
}
