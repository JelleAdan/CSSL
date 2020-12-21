using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using WaferFabSim;
using WaferFabSim.Import;
using WaferFabSim.InputDataConversion;
using WaferFabSim.SnapshotData;

namespace WaferFabDataReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\Auto";

            string outputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\SerializedFiles";

            LoadAutoData(inputDir, outputDir);
        }

        public static void LoadAutoData(string inputDir, string outputDir)
        {
            AutoDataReader reader = new AutoDataReader(inputDir, outputDir);

            reader.ReadWaferFabSettings();

            //reader.ReadLotActivityHistoriesCSV("LotActivity2019_2020.csv", true);
            //reader.ReadWorkCenterLotActivitiesDAT("WorkCenterLotActivities_2019_2020.dat");

            //reader.ReadLotActivityHistories("LotActivitySmallTestSet.csv", true);
            //reader.ReadLotActivityHistories("LotActivityJANFEB2020.csv", true);

            //reader.SaveWorkCenterLotActivities("2019_2020");
            //reader.SaveLotTraces("2019_2020");

            //reader.ReadLotTracesDAT("LotTraces_2019_2020.dat");

            //reader.GetRealLotStarts();

            reader.ReadWorkCenterLotActivitiesDAT("WorkCenterLotActivities_2019_2020.dat");

            reader.SaveRealLotStarts("2019_2020");

            //reader.SaveLotActivitiesToCSV($@"C: \Users\nx008314\OneDrive - Nexperia\Work\WaferFabPython\AllLotActivitiesWithEPT_20192020.csv");

            //ReadAndSaveRealSnapshotsPerMonth(reader);

            int stop = 0;
        }
        
        public static void ReadAndSaveRealSnaphotsAll(AutoDataReader reader)
        {
            DateTime start = reader.LotTraces.StartDate;
            DateTime end = reader.LotTraces.EndDate;

            DateTime from = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0, 0);
            DateTime until = new DateTime(end.Year, end.Month, end.Day, end.Hour, 0, 0, 0);
            TimeSpan frequency = new TimeSpan(1, 0, 0);

            reader.ReadRealSnapshots(from, until, frequency, 25);

            reader.SaveRealSnapshots($"{from.Year}-{from.Month}-{from.Day}_{until.Year}-{until.Month}-{until.Day}_{frequency.Hours}h");
        }

        public static void ReadAndSaveRealSnapshotsPerMonth(AutoDataReader reader)
        {
            DateTime start = reader.LotTraces.StartDate;
            DateTime end = reader.LotTraces.EndDate;

            DateTime from = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0, 0);
            DateTime until = new DateTime(start.AddMonths(1).Year, start.AddMonths(1).Month, 1, 0, 0, 0, 0); // Until first of next month
            TimeSpan frequency = new TimeSpan(1, 0, 0);

            while (until < end.AddMonths(1))
            {
                reader.ReadRealSnapshots(from, until, frequency, 25);

                reader.SaveRealSnapshots($"{from.Year}-{from.Month}-{from.Day}_{until.Year}-{until.Month}-{until.Day}_{frequency.Hours}h");

                from = until;
                until = until.AddMonths(1);
            }
        }

        public static void LoadManualData(string inputDir, string outputDir)
        {
            ManualDataReader reader = new ManualDataReader(inputDir + "CSVs");

            reader.ReadWaferFabSettings();

            reader.WaferFabSettings.SampleInterval = 12 * 60 * 60; // 12 hours

            reader.SaveWaferFabSettings("test");

            Tools.WriteToBinaryFile(inputDir + "test.dat", reader.WaferFabSettings);

            WaferFabSettings waferFabSettings2 = Tools.ReadFromBinaryFile<WaferFabSettings>(inputDir + "test.dat");

            int stop = 0;
        }
    }
}
