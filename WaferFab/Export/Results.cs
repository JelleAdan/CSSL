using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WaferFabSim.SnapshotData;

namespace WaferFabSim.OutputDataConversion
{
    public class Results
    {
        public Results(string directory)
        {
            this.directory = directory; 
        }

        private string directory { get; set; }
        public List<SimulationSnapshot> SimulationSnapshots { get; private set; }


        public void ReadResults()
        {
            SimulationSnapshots = new List<SimulationSnapshot>();

            // Get to replication directories
            foreach (string replicationDir in Directory.GetDirectories(directory))
            {
                var test = replicationDir.ToCharArray();

                int replicationNumber = Convert.ToInt32(replicationDir.Substring(replicationDir.Length - 1));

                // Get to waferFabObserver
                var waferFabObserverDir = Directory.GetFiles(replicationDir).Where(x => x.Contains("WaferFabObserver")).First();

                // Read data
                using (var reader = new StreamReader(waferFabObserverDir))
                {
                    string header = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string data = reader.ReadLine();

                        SimulationSnapshots.Add(new SimulationSnapshot(replicationNumber, header, data));
                    }
                }
            }
        }
    }
}
