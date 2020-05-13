using CSSL.Modeling;
using OxyPlot.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaferFabGUI.Models
{
    public class WIPSnapshot
    {
        public int Replication { get; private set; }

        public double SimulationTime { get; private set; }

        public TimeSpan SimulationTimeTimeSpan => TimeSpan.FromSeconds(SimulationTime);

        public double WallClockTime { get; private set; }

        public string[] LotSteps { get; private set; }

        public int[] WIPlevels { get; private set; }

        public WIPSnapshot(int replication, string headerLine, string dataLine)
        {
            var header = headerLine.Trim(',').Split(',');

            var data = dataLine.Trim(',').Split(',');

            if (header.Length != data.Length)
            {
                throw new Exception("Snapshot data is incomplete. Number of LotSteps does not match number of WIP levels.");
            }
            else
            {
                Replication = replication;

                LotSteps = header.Skip(2).ToArray();

                SimulationTime = Convert.ToDouble(data[0]);

                WallClockTime = Convert.ToDouble(data[1]);

                WIPlevels = data.Skip(2).Select(x => int.Parse(x)).ToArray();

            }
        }

    }
}
