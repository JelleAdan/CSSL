using CSSL.Examples.WaferFab;
using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WaferFabSim.InputDataConversion;
using WaferFabSim.SnapshotData;

namespace WaferFabSim.Import
{
    /// <summary>
    /// This is the base class for reading data from CSV files. This data can either directly be used in the ShellModel or be saved it into serialized .dat files.
    /// Serialized .dat files can be read by the DataImporter class.
    /// </summary>
    public abstract class DataReaderBase
    {

        public DataReaderBase(string directory)
        {
            InputDirectory = directory;
            OutputDirectory = directory;
        }

        public DataReaderBase(string inputDirectory, string outputDirectory)
        {
            InputDirectory = inputDirectory;
            OutputDirectory = outputDirectory;
        }

        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }

        public ExperimentSettings ExperimentSettings { get; set; }

        public WaferFabSettings WaferFabSettings { get; set; }

        public List<RealSnapshot> RealSnapshots { get; set; }

        public List<LotActivityHistory> LotActivityHistories { get; set; }

        public virtual ExperimentSettings ReadExperimentSettings(string fileName)
        {
            throw new NotImplementedException();
        }

        public virtual WaferFabSettings ReadWaferFabSettings()
        {
            throw new NotImplementedException();
        }

        public virtual List<LotActivityHistory> ReadLotActivityHistories(string fileName, bool productionLots)
        {
            throw new NotImplementedException();
        }

        public virtual List<RealSnapshot> ReadRealSnapshots(string fileName)
        {
            throw new NotImplementedException();
        }

        public void SaveExperimentSettings(string filename)
        {
            Tools.WriteToBinaryFile<ExperimentSettings>($@"{OutputDirectory}\ExperimentSettings_{filename}.dat", ExperimentSettings);
        }

        public void SaveWaferFabSettings(string filename)
        {
            Tools.WriteToBinaryFile<WaferFabSettings>($@"{OutputDirectory}\WaferFabSettings_{filename}.dat", WaferFabSettings);

        }

        public void SaveRealSnapshots(string filename)
        {
            Tools.WriteToBinaryFile<List<RealSnapshot>>($@"{OutputDirectory}\RealSnapshots_{filename}.dat", RealSnapshots);

        }

        public void SaveLotActivityHistories(string filename)
        {
            Tools.WriteToBinaryFile<List<LotActivityHistory>>($@"{OutputDirectory}\LotActivityHistories_{filename}.dat", LotActivityHistories);
        }
    }
}
