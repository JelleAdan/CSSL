using CSSL.Examples.WaferFab.Dispatchers;
using CSSL.Modeling.CSSLQueue;
using CSSL.Modeling.Elements;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class WorkCenter : SchedulingElementBase
    {
        public WorkCenter(ModelElementBase parent, string name, Distribution serviceTimeDistribution, List<LotStep> lotSteps) : base(parent, name)
        {
            LotSteps = lotSteps;
            ServiceTimeDistribution = serviceTimeDistribution;
            LotStepInService = null;
            Queues = new Dictionary<LotStep, CSSLQueue<Lot>>();

            foreach (LotStep lotStep in lotSteps)
            {
                Queues.Add(lotStep, new CSSLQueue<Lot>(this, name + "_" + lotStep.Name + "_Queue"));
            }

            SetWorkStationInLotSteps();
        }

        public Distribution ServiceTimeDistribution { get; }

        private DispatcherBase dispatcher { get; set; }

        public List<LotStep> LotSteps { get; set; }

        public List<Lot> InitialLots
        {
            get
            {
                WaferFab waferFab = (WaferFab)Parent;

                return waferFab.InitialLots.Where(x => x.GetCurrentWorkCenter == this).ToList();
            }
        }

        public Lot LastArrivedLot { get; set; }
        
        /// <summary>
        /// Flag for observers to indicate whether NotifyObservers is triggered by arrival or departure event. True = arrival, false = departure.
        /// </summary>
        public bool IsArrivalFlag { get; set; }

        public LotStep LotStepInService { get; set; }

        public Dictionary<LotStep, CSSLQueue<Lot>> Queues { get; set; }

        public int TotalQueueLength { get; private set; }

        public void SetDispatcher(DispatcherBase dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void SetWorkStationInLotSteps()
        {
            foreach (LotStep step in LotSteps)
            {
                step.SetWorkCenter(this);
            }
        }

        public void HandleArrival(Lot lot)
        {
            LastArrivedLot = lot;

            IsArrivalFlag = true;

            NotifyObservers(this);

            TotalQueueLength++;

            dispatcher.HandleArrival(lot);
        }

        public void HandleDeparture(CSSLEvent e)
        {
            if (LotStepInService == null)
            {
                dispatcher.HandleFirstDeparture();
            }
            else
            {
                IsArrivalFlag = false;

                NotifyObservers(this);

                TotalQueueLength--;

                dispatcher.HandleDeparture();
            }
        }

        protected override void OnReplicationStart()
        {
            LotStepInService = null;

            // Initialize queues with deep copy of initial lots
            if (InitialLots.Any())
            {
                List<Lot> initialLotsDeepCopy = InitialLots.ConvertAll(x => new Lot(x));

                foreach(var lot in initialLotsDeepCopy)
                {
                    HandleArrival(lot);
                }
            }
        }

    }
}
