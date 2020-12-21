using CSSL.Examples.WaferFab;
using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        public List<Tuple<DateTime, RealLot>> RealLotStarts { get; set; }

        public List<WorkCenterLotActivities> WorkCenterLotActivities { get; set; }

        public LotTraces LotTraces { get; set; }

        public virtual ExperimentSettings ReadExperimentSettings(string fileName)
        {
            throw new NotImplementedException();
        }

        public virtual WaferFabSettings ReadWaferFabSettings()
        {
            throw new NotImplementedException();
        }

        public virtual WaferFabSettings ReadWaferFabSettings(string serializedFileName)
        {
            throw new NotImplementedException();
        }

        public virtual List<WorkCenterLotActivities> ReadLotActivityHistoriesCSV(string fileName, bool productionLots)
        {
            throw new NotImplementedException();
        }

        protected WaferFabSettings ReadWaferFabSettingsDAT(string fileName)
        {
            //Console.Write($"Reading {fileName} - ");
            WaferFabSettings = Tools.ReadFromBinaryFile<WaferFabSettings>(Path.Combine(OutputDirectory, fileName));
            //Console.Write($"done.\n");
            return WaferFabSettings;
        }


        public List<Tuple<DateTime, RealLot>> ReadRealLotStartsDAT(string fileName)
        {
            Console.Write($"Reading {fileName} - ");
            RealLotStarts = Tools.ReadFromBinaryFile<List<Tuple<DateTime, RealLot>>>(Path.Combine(OutputDirectory, fileName));
            Console.Write($"done.\n");
            return RealLotStarts;
        }

        public List<WorkCenterLotActivities> ReadWorkCenterLotActivitiesDAT(string fileName)
        {
            Console.Write($"Reading {fileName} - ");
            WorkCenterLotActivities = Tools.ReadFromBinaryFile<List<WorkCenterLotActivities>>(Path.Combine(OutputDirectory, fileName));
            Console.Write($"done.\n");
            return WorkCenterLotActivities;
        }

        public LotTraces ReadLotTracesDAT(string fileName)
        {
            Console.Write($"Reading {fileName} - ");
            LotTraces = Tools.ReadFromBinaryFile<LotTraces>(Path.Combine(OutputDirectory, fileName));
            Console.Write($"done.\n");
            return LotTraces;
        }

        public virtual List<RealSnapshot> ReadRealSnapshots(DateTime from, DateTime until, TimeSpan interval, int waferQtyThreshold)
        {
            throw new NotImplementedException();
        }

        public void SaveExperimentSettings(string filename)
        {
            Console.Write($"Saving {filename} - ");
            Tools.WriteToBinaryFile<ExperimentSettings>($@"{OutputDirectory}\ExperimentSettings_{filename}.dat", ExperimentSettings);
            Console.Write($"done.\n");
        }

        public void SaveWaferFabSettings(string filename)
        {
            Console.Write($"Saving {filename} - ");
            Tools.WriteToBinaryFile<WaferFabSettings>($@"{OutputDirectory}\WaferFabSettings_{filename}.dat", WaferFabSettings);
            Console.Write($"done.\n");

        }

        public void SaveRealSnapshots(string filename)
        {
            Console.Write($"Saving {filename} - ");
            Tools.WriteToBinaryFile<List<RealSnapshot>>($@"{OutputDirectory}\RealSnapshots_{filename}.dat", RealSnapshots);
            Console.Write($"done.\n");
        }

        public void SaveWorkCenterLotActivities(string filename)
        {
            Console.Write($"Saving {filename} - ");
            Tools.WriteToBinaryFile<List<WorkCenterLotActivities>>($@"{OutputDirectory}\WorkCenterLotActivities_{filename}.dat", WorkCenterLotActivities);
            Console.Write($"done.\n");
        }

        public void SaveLotTraces(string filename)
        {
            Console.Write($"Saving {filename} - ");
            Tools.WriteToBinaryFile<LotTraces>($@"{OutputDirectory}\LotTraces_{filename}.dat", LotTraces);
            Console.Write($"done.\n");
        }

        public void SaveRealLotStarts(string filename)
        {
            Console.Write($"Saving {filename} - ");
            Tools.WriteToBinaryFile<List<Tuple<DateTime, RealLot>>>($@"{OutputDirectory}\LotStarts_{filename}.dat", RealLotStarts);
            Console.Write($"done.\n");
        }
    }
}
