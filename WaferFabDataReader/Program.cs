using System;
using WaferFabSim;
using WaferFabSim.InputDataConversion;

namespace WaferFabDataReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputDir = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\Auto";

            string outputDir = @"C:\CSSLWaferFab\";

            LoadAutoData(inputDir, outputDir);


        }

        public static void LoadAutoData(string inputDir, string outputDir)
        {
            AutoDataReader reader = new AutoDataReader(inputDir, outputDir);

            reader.ReadWaferFabSettings();

            reader.ReadLotActivityHistories("LotActivity2019_2020.csv", true);

            int stop = 0;
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
