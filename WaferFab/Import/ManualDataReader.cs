using CSSL.Examples.WaferFab;
using CSSL.Examples.WaferFab.Dispatchers;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WaferFabSim.Import;

namespace WaferFabSim.InputDataConversion
{
    public class ManualDataReader : DataReaderBase
    {
        public ManualDataReader(string directory) : base(directory)
        {
            WorkCenterLotStepMapping = new Dictionary<LotStep, string>();
        }

        private Dictionary<LotStep, string> WorkCenterLotStepMapping { get; set; }

        public override WaferFabSettings ReadWaferFabSettings()
        {
            WaferFabSettings = new WaferFabSettings();

            read();

            return WaferFabSettings;
        }

        private void read()
        {
            readLotStarts();
            readWorkCenters();
            readLotSteps();
            readSequences();

            mapLotStepsPerWorkStation();
        }

        private void mapLotStepsPerWorkStation()
        {
            foreach (string wc in WaferFabSettings.WorkCenters)
            {
                WaferFabSettings.LotStepsPerWorkStation.Add(wc, new List<LotStep>());
            }

            foreach (var mapping in WorkCenterLotStepMapping)
            {
                WaferFabSettings.LotStepsPerWorkStation[mapping.Value].Add(mapping.Key);
            }
        }

        private void readLotStarts()
        {
            using (var reader = new StreamReader(Path.Combine(InputDirectory, "LotStarts.csv")))
            {
                int row = 1;

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    if (row == 1)
                    {
                        WaferFabSettings.LotStartsFrequency = Convert.ToInt32(values[2]);
                    }
                    else
                    {
                        WaferFabSettings.ManualLotStartQtys.Add(values[0], Convert.ToInt32(values[1]));
                    }

                    row++;
                }
            }
        }

        public void readWorkCenters()
        {
            using (var reader = new StreamReader(Path.Combine(InputDirectory, "WorkCenters.csv")))
            {
                int row = 1;

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    if (row != 1)
                    {
                        WaferFabSettings.WorkCenters.Add(values[0]);
                        WaferFabSettings.WCServiceTimeDistributions.Add(values[0], new ExponentialDistribution(Convert.ToDouble(values[1])));
                        WaferFabSettings.WCDispatchers.Add(values[0], "BQF");
                    }
                    row++;
                }
            }
        }

        public void readLotSteps()
        {
            using (var reader = new StreamReader(Path.Combine(InputDirectory, "LotSteps.csv")))
            {
                int row = 1;

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    if (row != 1 && values[0] != "")
                    {
                        LotStep step = new LotStep(Convert.ToInt32(values[0]), values[1]);

                        WaferFabSettings.LotSteps.Add(values[1], step);

                        WorkCenterLotStepMapping.Add(step, values[2]);
                    }
                    row++;
                }
            }
        }

        public void readSequences()
        {
            using (var reader = new StreamReader(Path.Combine(InputDirectory, "Sequences.csv")))
            {
                var header = reader.ReadLine().Split(',');

                string[] lotTypes = header.ToArray();

                int count = lotTypes.Length;

                List<LotStep>[] sequences = new List<LotStep>[count];

                for (int i = 0; i < count; i++)
                {
                    sequences[i] = new List<LotStep>();
                }

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    for (int i = 0; i < count; i++)
                    {
                        if (values[i] != "")
                        {
                            sequences[i].Add(WaferFabSettings.LotSteps[values[i]]);
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    WaferFabSettings.Sequences.Add(lotTypes[i], new Sequence(lotTypes[i], lotTypes[i], sequences[i]));
                }
            }
        }


    }
}
