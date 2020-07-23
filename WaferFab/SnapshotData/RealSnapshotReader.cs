using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WaferFabSim.SnapshotData
{
    public class RealSnapshotReader
    {
        public RealSnapshotReader()
        {
            RealLots = new List<RealLot>();
        }
        public string fileDirectory { get; set; }

        public int WaferQtyThreshold { get; set; }

        public List<RealLot> RealLots { get; set; }

        public List<RealSnapshot> RealSnapshots { get; set; }

        public List<RealSnapshot> Read(string fileToRead, int waferQtyThreshold)
        {
            WaferQtyThreshold = waferQtyThreshold;
            fileDirectory = fileToRead;
            RealLots = fillAllLots();
            RealSnapshots = constructRealSnapshots();

            return RealSnapshots;
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

            using (StreamReader reader = new StreamReader(fileDirectory))
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
