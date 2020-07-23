using CSSL.Examples.WaferFab;
using CSSL.Examples.WaferFab.Dispatchers;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WaferFabSim.InputDataConversion;
using WaferFabSim.SnapshotData;

namespace WaferFabSim
{
    [Serializable]
    public class WaferFabSettings
    {
        public WaferFabSettings()
        {
            SampleInterval = 10 * 60;

            LotStartQtys = new Dictionary<string, int>();
            InitialLots = new List<RealLot>();
            LotTypes = new List<string>();
            WorkCenters = new List<string>();
            LotSteps = new Dictionary<string, LotStep>();
            WorkCenterDistributions = new Dictionary<string, Distribution>();
            WorkCenterDispatchers = new Dictionary<string, string>();
            LotStepsPerWorkStation = new Dictionary<string, List<LotStep>>();
            Sequences = new Dictionary<string, Sequence>();

            CheckData();
        }

        // Observers
        public double SampleInterval { get; set; }

        // Lot starts
        public int LotStartsFrequency { get; set; }
        public Dictionary<string, int> LotStartQtys { get; set; }

        // Initial Lots
        public List<RealLot> InitialLots { get; set; }

        // Model
        public List<string> LotTypes { get; set; }
        public Dictionary<string, LotStep> LotSteps { get; set; }
        public List<string> WorkCenters { get; set; }
        public Dictionary<string, Distribution> WorkCenterDistributions { get; set; }
        public Dictionary<string, string> WorkCenterDispatchers { get; set; }
        public Dictionary<string, List<LotStep>> LotStepsPerWorkStation { get; set; }
        public Dictionary<string, Sequence> Sequences { get; set; }

        // Processing Time History
        public Dictionary<string, LotActivityHistory> ProcessingTimeHistories { get; set; }


        public WaferFab GetWaferFab()
        {

            CheckData();

            // To implement
            return null;
        }

        private void CheckData()
        {
            if (LotSteps.Count != LotSteps.Select(x => x.Value.Id).Distinct().Count())
            {
                throw new Exception("LotStep IDs are not unique.");
            }
        }
    }

}
