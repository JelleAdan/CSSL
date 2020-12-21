using CSSL.Utilities;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Examples.WaferFab.Utilities
{
    [Serializable]
    /// <summary>
    /// Lotstep-dependent overtaking distribution. This distribution contains one WIP-dependent OvertakingDistribution per LotStep
    /// </summary>
    public class LotStepOvertakingDistribution : OvertakingDistributionBase
    {
        public LotStepOvertakingDistribution(List<OvertakingRecord> records, OvertakingDistributionParameters parameters, List<LotStep> lotSteps) : base(records)
        {
            this.records = records;
            this.parameters = parameters;
            this.lotSteps = lotSteps;

            distributions = new Dictionary<string, OvertakingDistribution>();

            buildDistribution();
        }

        private WorkCenter _workcenter;
        public override WorkCenter WorkCenter
        {
            get
            {
                return _workcenter;
            }
            set
            {
                _workcenter = value;

                foreach (OvertakingDistribution distr in distributions.Values)
                {
                    distr.WorkCenter = value;
                }
            }
        }

        private Dictionary<string, OvertakingDistribution> distributions { get; set; }

        /// <summary>
        /// Records ordered on WIPIn 
        /// </summary>
        private List<OvertakingRecord> records { get; }

        private List<string> lotStepsInRecords { get; set; }

        private OvertakingDistributionParameters parameters { get; }

        private List<LotStep> lotSteps { get; }

        public static int LotStepNotFoundCount;

        public static int LotStepFoundCount;

        public override double Next()
        {
            string lotStep = WorkCenter.LastArrivedLot.GetCurrentStep.Name;

            double next;

            if (distributions.ContainsKey(lotStep))
            {
                LotStepFoundCount++;
                next = distributions[lotStep].Next();
            }
            else
            {   // get random other lotstep if this lotstep is unknown
                LotStepNotFoundCount++;
                next = distributions[lotStepsInRecords[rnd.Next(0, lotStepsInRecords.Count())]].Next();
            }

            return next;
        }

        /// <summary>
        /// Used for initialization
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="queueLength"></param>
        /// <returns></returns>
        public override double Next(Lot lot, int WIP)
        {
            string lotStep = lot.GetCurrentStep.Name;

            double next;

            if (distributions.ContainsKey(lotStep))
            {
                LotStepFoundCount++;
                next = distributions[lotStep].Next(lot, WIP);
            }
            else
            {   // get random other lotstep, this lotstep is unknown
                LotStepNotFoundCount++;
                next = distributions[lotStepsInRecords[rnd.Next(0, lotStepsInRecords.Count())]].Next(lot, WIP);
            }

            return next;
        }

        private void buildDistribution()
        {
            foreach (string step in lotSteps.Select(x => x.Name))
            {
                List<OvertakingRecord> selectedRecords = records.Where(x => x.LotStep == step).ToList();

                if (selectedRecords.Any())
                {
                    distributions.Add(step, new OvertakingDistribution(selectedRecords, parameters));
                }
            }

            lotStepsInRecords = distributions.Keys.ToList();
        }
    }
}
