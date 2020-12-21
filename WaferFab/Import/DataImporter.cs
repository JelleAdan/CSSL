using System;
using System.Collections.Generic;
using System.Text;
using WaferFabSim.InputDataConversion;
using WaferFabSim.SnapshotData;

namespace WaferFabSim.Import
{
    public class DataImporter
    {
        public WaferFabSettings ImportWaferFabSettings(string directory)
        {
            return Tools.ReadFromBinaryFile<WaferFabSettings>(directory);
        }

        public ExperimentSettings ImportExperimentSettings(string directory)
        {
            return Tools.ReadFromBinaryFile<ExperimentSettings>(directory);
        }

        public List<RealSnapshot> ImportRealSnapshots(string directory)
        {
            return Tools.ReadFromBinaryFile<List<RealSnapshot>>(directory);
        }

        // Still to implement
        public List<RealLot> ImportLotStarts(string directory)
        {
            return Tools.ReadFromBinaryFile<List<RealLot>>(directory);
        }

    }
}
