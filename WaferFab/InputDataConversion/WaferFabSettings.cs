using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WaferFabSim.InputDataConversion
{
    public class WaferFabSettings
    {
        public WaferFabSettings(string directory)
        {
            this.directory = directory;
            LotStartQtys = new Dictionary<LotType, int>();
            WorkCentersData = new Dictionary<string, double>();
            lotStepsData = new List<Tuple<int, string, string>>();
            LotSteps = new Dictionary<string, LotStep>();
            SequencesData = new Dictionary<LotType, List<LotStep>>();
            LotStepsPerWorkStation = new Dictionary<string, List<LotStep>>();

            readLotStarts();
            readWorkCenters();
            readLotSteps();
            readSequences();

            CheckData();
        }

        private string directory { get; set; }

        public double sampleInterval { get; set; }

        public int LotStartsFrequency { get; set; }

        public Dictionary<LotType, int> LotStartQtys { get; private set; }

        public Dictionary<string, double> WorkCentersData { get; private set; }

        private List<Tuple<int, string, string>> lotStepsData { get; set; }

        public Dictionary<string, LotStep> LotSteps { get; set; }

        public Dictionary<string, List<LotStep>> LotStepsPerWorkStation { get; private set; }

        public Dictionary<LotType, List<LotStep>> SequencesData { get; private set; }

        public void readLotStarts()
        {
            using (var reader = new StreamReader(Path.Combine(directory, "LotStarts.csv")))
            {
                int row = 1;

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    if (row == 1)
                    {
                        LotStartsFrequency = Convert.ToInt32(values[2]);
                    }
                    else
                    {
                        LotStartQtys.Add((LotType)Enum.Parse(typeof(LotType), values[0]), Convert.ToInt32(values[1]));
                    }

                    row++;
                }
            }
        }

        public void readWorkCenters()
        {
            using (var reader = new StreamReader(Path.Combine(directory, "WorkCenters.csv")))
            {
                int row = 1;

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    if (row != 1)
                    {
                        WorkCentersData.Add(values[0], Convert.ToDouble(values[1]));
                    }
                    row++;
                }
            }
        }

        public void readLotSteps()
        {
            using (var reader = new StreamReader(Path.Combine(directory, "LotSteps.csv")))
            {
                int row = 1;

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    if (row != 1 && values[0] != "")
                    {
                        lotStepsData.Add(new Tuple<int, string, string>(Convert.ToInt32(values[0]), values[1], values[2]));

                        LotSteps.Add(values[1], new LotStep(values[1]));
                    }
                    row++;
                }
            }

            foreach (var wc in WorkCentersData.Keys)
            {
                LotStepsPerWorkStation.Add(wc, lotStepsData.Where(x => x.Item3 == wc).Select(x => x.Item2).Select(x => LotSteps[x]).ToList());
            }
        }

        public void readSequences()
        {
            using (var reader = new StreamReader(Path.Combine(directory, "Sequences.csv")))
            {
                var header = reader.ReadLine().Split(',');

                LotType[] lotTypes = header.Select(x => (LotType)Enum.Parse(typeof(LotType), x)).ToArray();

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
                            sequences[i].Add(LotSteps[values[i]]);
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    SequencesData.Add(lotTypes[i], sequences[i]);
                }
            }
        }

        public void CheckData()
        {
            // To be implemented.
        }
    }

}
