using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WaferFabSim.InputDataConversion;

namespace WaferFabSim.SnapshotData
{
    public class RealSnapshotReader
    {
        public RealSnapshotReader()
        {
            RealLots = new List<RealLot>();
            RealSnapshots = new List<RealSnapshot>();
        }
        public string Filename { get; set; }

        public int WaferQtyThreshold { get; set; }

        public List<RealLot> RealLots { get; set; }

        public List<RealSnapshot> RealSnapshots { get; set; }

        public List<RealSnapshot> Read(string filename, int waferQtyThreshold)
        {
            RealLots.Clear();
            RealSnapshots.Clear();

            WaferQtyThreshold = waferQtyThreshold;
            Filename = filename;

            string type = Path.GetExtension(filename).ToLower();

            if (type == ".csv")
            {
                readCSV();
            }
            else if (type == ".dat")
            {
                ReadDAT();
            }
            else
            {
                throw new Exception($"Cannot read file type {type}");
            }

            return RealSnapshots;
        }

        private void readCSV()
        {
            RealLots = fillAllLots();
            RealSnapshots = constructRealSnapshots();
        }

        private void ReadDAT()
        {
            RealSnapshots = Tools.ReadFromBinaryFile<List<RealSnapshot>>(Filename);
            RealLots = RealSnapshots.SelectMany(x => x.RealLots).ToList();
        }

        private List<RealSnapshot> constructRealSnapshots()
        {
            List<RealSnapshot> allSnapshots = new List<RealSnapshot>();

            foreach (var snapshotTime in RealLots.Select(x => x.SnapshotTime).Distinct())
            {
                allSnapshots.Add(new RealSnapshot(RealLots.Where(x => x.SnapshotTime == snapshotTime).ToList(), WaferQtyThreshold));
            }

            return allSnapshots.OrderBy(x => x.Time).ToList();
        }

        private List<RealLot> fillAllLots()
        {
            List<RealLot> allLots = new List<RealLot>();

            using (StreamReader reader = new StreamReader(Filename))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string dataLine = reader.ReadLine();

                    allLots.Add(new RealLot(headerLine, dataLine));
                }
            }

            return allLots;
        }
    }

}
