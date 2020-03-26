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

            foreach(LotStep lotStep in lotSteps)
            {
                Queues.Add(lotStep, new CSSLQueue<Lot>(this, name + "_" + lotStep.Name + "_Queue"));
            }
        }

        public Distribution ServiceTimeDistribution { get; }

        private DispatcherBase dispatcher { get; set; }

        public List<LotStep> LotSteps { get; set; }

        public Lot LastArrivedLot { get; set; }

        public LotStep LotStepInService { get; set; }

        public Dictionary<LotStep, CSSLQueue<Lot>> Queues { get; set; }

        public int TotalQueueLength { get; private set; }

        public void SetDispatcher(DispatcherBase dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void ConnectLotSteps()
        {
            foreach(LotStep lot in LotSteps)
            {
                lot.SetWorkCenter(this);
            }
        }

        public void HandleArrival(Lot lot)
        {
            LastArrivedLot = lot;

            NotifyObservers(this);

            TotalQueueLength++;

            dispatcher.HandleArrival(lot);
        }

        public void HandleDeparture(CSSLEvent e)
        {
            if (LotStepInService == null)
            {
                throw new Exception($"Tried to send lot from {Name}, but there is no type (LotType) in service.");
            }
            else
            {
                NotifyObservers(this);

                TotalQueueLength--;

                dispatcher.HandleDeparture();
            }
        }
    }
}
