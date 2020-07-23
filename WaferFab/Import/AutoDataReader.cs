using CSSL.Examples.WaferFab;
using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WaferFabSim.Import;

namespace WaferFabSim.InputDataConversion
{
    public class AutoDataReader : DataReaderBase
    {
        public AutoDataReader(string directory) : base(directory)
        {
            lotStepsRaw = new List<SingleStep>();
            irdMappings = new List<IRDMapping>();
            irdNumbering = new Dictionary<string, int>();
            lotActivitiesRaw = new List<LotActivityRaw>();
        }

        public AutoDataReader(string inputDirectory, string outputDirectory) : base(inputDirectory, outputDirectory)
        {
            lotStepsRaw = new List<SingleStep>();
            irdMappings = new List<IRDMapping>();
            irdNumbering = new Dictionary<string, int>();
            lotActivitiesRaw = new List<LotActivityRaw>();
        }


        public override WaferFabSettings ReadWaferFabSettings()
        {
            readWaferFabSettingsData();
            lotSteps = fillStepsWithIRDs();

            WaferFabSettings = new WaferFabSettings();

            WaferFabSettings.LotStartsFrequency = 12;

            WaferFabSettings.LotTypes = getProductTypes();

            WaferFabSettings.LotStartQtys = WaferFabSettings.LotTypes.ToDictionary(x => x, x => 0);

            WaferFabSettings.LotSteps = lotSteps;

            WaferFabSettings.WorkCenters = irdMappings.Select(x => x.WorkStation).Distinct().ToList();

            WaferFabSettings.WorkCenterDistributions = getDistributions();

            WaferFabSettings.WorkCenterDispatchers = getDispatchers();

            WaferFabSettings.LotStepsPerWorkStation = getLotStepsPerWorkstation();

            WaferFabSettings.Sequences = getSequencesPerIRDGroup();

            processPlans = getProcessPlans();

            return WaferFabSettings;
        }

        public override List<LotActivityHistory> ReadLotActivityHistories(string fileName, bool onlyProductionLots)
        {
            LotActivityHistories = new List<LotActivityHistory>();

            using (StreamReader reader = new StreamReader(Path.Combine(InputDirectory, fileName)))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string dataLine = reader.ReadLine();

                    lotActivitiesRaw.Add(new LotActivityRaw(headerLine, dataLine));
                }

                // Filter only production lots, order by track-in time and group activities from same lot
                if (onlyProductionLots)
                {
                    lotActivitiesRaw = lotActivitiesRaw.Where(x => x.LotId.StartsWith("M1")).OrderBy(x => x.TrackIn).OrderBy(x => x.LotId).ToList();
                }

                // Map IRD groups on LotActivities
                Dictionary<string, IRDMapping> irdDict = irdMappings.ToDictionary(x => $"{x.Techstage} {x.Subplan}", x => x);

                List<string> lotids = lotActivitiesRaw.Select(x => x.LotId).ToList();

                foreach (var lotActivity in lotActivitiesRaw)
                {
                    if (irdDict.ContainsKey($"{lotActivity.Techstage} {lotActivity.Subplan}"))
                    {
                        lotActivity.IRDGroup = irdDict[$"{lotActivity.Techstage} {lotActivity.Subplan}"].IRDGroup;
                    }
                }
            }

            // Group lotactivities per Lot into LotTraces
            List<LotTrace> lotTraces = new List<LotTrace>();

            string currentId = "";
            LotTrace trace = new LotTrace("");

            foreach (LotActivityRaw activity in lotActivitiesRaw)
            {
                if (activity.LotId != currentId)
                {
                    if (currentId != "")
                        lotTraces.Add(trace);

                    currentId = activity.LotId;

                    trace = new LotTrace(activity.LotId);
                    trace.ProductType = activity.ProductType;
                }

                trace.LotActivities.Add(activity);

                trace.AddProcessPlan(processPlans);
                trace.CheckCompleteness();

                lotTraces.Add(trace);
            }

            int complete = lotTraces.Where(x => x.IsComplete).Count();
            int noprocessplan = lotTraces.Where(x => !x.HasProcessPlan).Count();
            int nostart = lotTraces.Where(x => x.HasProcessPlan && !x.HasStart).Count();
            int noend = lotTraces.Where(x => x.HasProcessPlan && !x.HasEnd).Count();

            int stop = 0;

            var noendlist = lotTraces.Where(x => x.HasProcessPlan && !x.HasEnd).ToList();
            var noprocessplanlist = lotTraces.Where(x => !x.HasProcessPlan).Select(x => x.ProductType).Distinct().ToList();

            int stop2 = 0;

            var noprocessplanlis2t = lotTraces.Where(x => !x.HasProcessPlan && x.ProductType == "CVCheck1").ToList();

            int asokd = 0;

            var terminated = lotTraces.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivities.Last().Status == "Terminated").ToList();
            var onhold = lotTraces.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivities.Last().Status == "Hold").ToList();
            var active = lotTraces.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivities.Last().Status == "Active").ToList();
            var Finished = lotTraces.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivities.Last().Status == "Finished").ToList();

            int asd = 0;

            var None = lotTraces.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivities.Last().Status != "Terminated" && x.LotActivities.Last().Status != "Active" && x.LotActivities.Last().Status != "Hold").ToList();

            int stoppp = 0;


            //List<LotActivityRaw> temp = lotActivitiesRaw.Where(x => x.IRDGroup == null).ToList();
            //List<LotActivityRaw> temp2 = lotActivitiesRaw.Where(x => x.IRDGroup != null).ToList();

            //List<string> IDs = temp.Select(x => x.Id).OrderBy(x => x).ToList();
            //List<string> IDs2 = temp2.Select(x => x.Id).OrderBy(x => x).ToList();


            return LotActivityHistories;
        }

        // To read LotActivityHistories
        private List<LotActivityRaw> lotActivitiesRaw { get; set; }

        // To read WaferFabSettings
        private List<SingleStep> lotStepsRaw { get; set; }
        private Dictionary<string, LotStep> lotSteps { get; set; }
        private List<IRDMapping> irdMappings { get; set; }
        private Dictionary<string, int> irdNumbering { get; set; }
        private Dictionary<string, ProcessPlan> processPlans { get; set; }

        private void readWaferFabSettingsData()
        {
            lotStepsRaw.Clear();
            irdMappings.Clear();

            // Read steps from process plans
            using (StreamReader reader = new StreamReader(Path.Combine(InputDirectory, "ProcessPlans.csv")))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string dataLine = reader.ReadLine();

                    lotStepsRaw.Add(new SingleStep(headerLine, dataLine));
                }
            }

            // Read IRD Mappings
            using (StreamReader reader = new StreamReader(Path.Combine(InputDirectory, "IRDMapping.csv")))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string dataLine = reader.ReadLine();

                    irdMappings.Add(new IRDMapping(headerLine, dataLine));
                }
            }



            // Map IRDs on Steps
            //List<string> missingLines = new List<string>();

            foreach (var step in lotStepsRaw)
            {
                for (int i = 0; i < irdMappings.Count; i++)
                {
                    var mapping = irdMappings[i];

                    if (step.Techstage == mapping.Techstage && step.Subplan == mapping.Subplan)
                    {
                        step.IRDGroup = mapping.IRDGroup;
                        step.ToolGroup = mapping.WorkStation;
                        break;
                    }
                    else if (i == irdMappings.Count - 1)
                    {
                        throw new Exception($"Did not find IRDGroup for {step.Productname} in {step.Techstage} {step.Subplan}");
                        //missingLines.Add($"{step.Techstage},{step.Subplan}");
                    }
                }
            }

            //using (StreamWriter writer = new StreamWriter(@"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab\MissingIRDs.csv", false))
            //{
            //    foreach (var line in missingLines.Distinct())
            //    {
            //        writer.WriteLine(line);
            //    }
            //}

            // Read IRD Numbering
            using (StreamReader reader = new StreamReader(Path.Combine(InputDirectory, "IRDNumbering.csv")))
            {
                string headerLine = reader.ReadLine(); // not used

                while (!reader.EndOfStream)
                {
                    string[] dataLine = reader.ReadLine().Split(',');

                    irdNumbering.Add(dataLine[0], Convert.ToInt32(dataLine[1]));
                }

                // Order the lists on IDs
                irdNumbering = irdNumbering.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }
        private Dictionary<string, LotStep> fillStepsWithIRDs()
        {
            Dictionary<string, LotStep> steps = new Dictionary<string, LotStep>();

            foreach (var IRD in irdNumbering)
            {
                steps.Add(IRD.Key, new LotStep(IRD.Value, IRD.Key));
            }

            return steps;
        }
        private Dictionary<string, List<LotStep>> getLotStepsPerWorkstation()
        {
            Dictionary<string, List<LotStep>> lotStepsPerWorkstation = new Dictionary<string, List<LotStep>>();

            // Initialize Lists for each workcenter
            foreach (string wc in WaferFabSettings.WorkCenters)
            {
                lotStepsPerWorkstation.Add(wc, new List<LotStep>());
            }

            // Add all steps to corresponding lists
            foreach (var irdMapping in irdMappings)
            {
                // TO DO: REMOVE THIS if SAOP Process plans is up to date

                if (WaferFabSettings.LotSteps.ContainsKey(irdMapping.IRDGroup))
                {
                    lotStepsPerWorkstation[irdMapping.WorkStation].Add(WaferFabSettings.LotSteps[irdMapping.IRDGroup]);
                }
            }

            // Order lists per workcenter
            foreach (string wc in WaferFabSettings.WorkCenters)
            {
                lotStepsPerWorkstation[wc] = lotStepsPerWorkstation[wc].Distinct().OrderBy(x => x.Id).ToList();
            }

            return lotStepsPerWorkstation;
        }
        private Dictionary<string, Sequence> getSequencesPerIRDGroup()
        {
            List<Sequence> sequences = new List<Sequence>();

            foreach (string product in lotStepsRaw.Select(x => x.Productname).Distinct())
            {
                List<SingleStep> stepsThisProduct = lotStepsRaw.Where(x => x.Productname == product).OrderBy(x => x.StepSequence).ToList();

                Sequence seq = new Sequence(stepsThisProduct.First().Productname, stepsThisProduct.First().Plangroup);

                string currentIRD = "";

                // NOTE. Property of lists: foreach loop on List loops in correct order (from first to last index)
                foreach (var step in stepsThisProduct)
                {
                    if (currentIRD != step.IRDGroup)
                    {
                        currentIRD = step.IRDGroup;

                        seq.AddStep(lotSteps[currentIRD]);
                    }
                }
                sequences.Add(seq);
            }

            return sequences.ToDictionary(x => x.ProductType);
        }
        private Dictionary<string, ProcessPlan> getProcessPlans()
        {
            Dictionary<string, ProcessPlan> ProcessPlans = new Dictionary<string, ProcessPlan>();

            foreach (string product in getProductTypes())
            {
                ProcessPlan plan = new ProcessPlan(product, lotStepsRaw.Where(x => x.Productname == product).OrderBy(x => x.StepSequence).ToList());

                ProcessPlans.Add(product, plan);
            }

            return ProcessPlans;
        }


        private Dictionary<string, Distribution> getDistributions()
        {
            Dictionary<string, Distribution> dict = new Dictionary<string, Distribution>();

            foreach (string wc in WaferFabSettings.WorkCenters)
            {
                dict.Add(wc, new ExponentialDistribution(0.01));
            }

            return dict;
        }
        private Dictionary<string, string> getDispatchers()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (string wc in WaferFabSettings.WorkCenters)
            {
                dict.Add(wc, "BQF");
            }

            return dict;
        }
        private List<string> getProductTypes()
        {
            return lotStepsRaw.Select(x => x.Productname).Distinct().ToList();
        }
        private List<string> getProductGroups()
        {
            return lotStepsRaw.Select(x => x.Plangroup).Distinct().ToList();
        }

        private class LotTrace
        {
            public string ProductType { get; set; }
            public string LotId { get; private set; }
            public bool IsComplete => HasProcessPlan && HasEnd && HasStart;
            public bool HasStart { get; set; }
            public bool HasEnd { get; set; }
            public bool HasProcessPlan { get; set; }

            public string Status => LotActivities.Any() ? LotActivities.Last().Status : "NoLotActivities";


            public List<LotActivityRaw> LotActivities { get; set; }

            public ProcessPlan ProcessPlan { get; set; }

            public void AddProcessPlan(Dictionary<string, ProcessPlan> processPlans)
            {
                if (processPlans.ContainsKey(ProductType))
                    ProcessPlan = processPlans[ProductType];
            }

            public void CheckCompleteness()
            {
                HasStart = true;
                HasEnd = true;
                HasProcessPlan = true;


                // Check whether trace has process plan
                if (ProcessPlan == null)
                {
                    HasProcessPlan = false;
                    return;
                }


                // Check whether trace contains first step
                SingleStep firstStep = ProcessPlan.Steps.First();

                if (!LotActivities.Where(x => x.Stepname == firstStep.Stepname).Any())
                {
                    HasStart = false;
                    return;
                }

                // Check whether trace contains last step
                SingleStep lastStep = ProcessPlan.Steps.Last();

                if (!LotActivities.Where(x => x.Stepname == lastStep.Stepname).Any())
                {
                    HasEnd = false;

                    return;
                }

                // Passed all checks, so trace is complete
                return;
            }

            public LotTrace(string lotid)
            {
                LotActivities = new List<LotActivityRaw>();
                LotId = lotid;
            }
        }

        private class LotActivityRaw
        {
            public string LotId { get; set; }
            public string SplitId { get; set; }        // If null no split, if some string is split
            public string PlanName { get; set; }
            public string ProductType { get; set; }
            public string Techstage { get; set; }
            public string Subplan { get; set; }
            public string Stepname { get; set; }
            public string Location { get; set; }
            public string Recipe { get; set; }
            public DateTime TrackIn { get; set; }
            public DateTime TrackOut { get; set; }
            public int QtyIn { get; set; }
            public int QtyOut { get; set; }
            public string Status { get; set; }
            public string IRDGroup { get; set; }

            public LotActivityRaw(string headerLine, string dataLine)
            {
                string[] headers = headerLine.Trim(',').Split(',');
                string[] data = dataLine.Trim(',').Split(',');

                for (int i = 0; i < data.Length; i++)
                {
                    if (headers[i] == "FW_LOTID")
                    {
                        if (data[i].Length > 7)
                        {
                            LotId = data[i].Substring(0, 7);
                            SplitId = data[i].Substring(7);
                        }
                        else
                        {
                            LotId = data[i];
                        }
                    }
                    if (headers[i] == "FW_CURRENTPLANNAME") { PlanName = data[i]; }
                    if (headers[i] == "FW_CURRENTPRODUCTNAME") { ProductType = data[i]; }
                    if (headers[i] == "TECHSTAGE") { Techstage = data[i]; }
                    if (headers[i] == "SUBPLAN") { Subplan = data[i]; }
                    if (headers[i] == "FW_STEPNAME") { Stepname = data[i]; }
                    if (headers[i] == "FW_LOCATION") { Location = data[i]; }
                    if (headers[i] == "FW_TRACKINTIME") { TrackIn = Convert.ToDateTime(data[i]); }
                    if (headers[i] == "FW_TRACKOUTTIME")
                    {
                        if (data[i] != "1/0/1900 0:00")
                        {
                            TrackOut = Convert.ToDateTime(data[i]);
                        }
                        else
                        {
                            TrackOut = Convert.ToDateTime("1/1/1900 0:00");
                        }
                    }
                    if (headers[i] == "FW_STEPQTYIN") { QtyIn = Convert.ToInt32(data[i]); }
                    if (headers[i] == "FW_STEPQTYOUT") { QtyOut = Convert.ToInt32(data[i]); }
                    if (headers[i] == "FW_CURRENTSTATUS") { Status = data[i]; }
                }
            }
        }

        private class ProcessPlan
        {
            public string Productname { get; private set; }
            public List<SingleStep> Steps { get; set; }

            public ProcessPlan(string productName, List<SingleStep> steps)
            {
                Productname = productName;
                Steps = steps;
            }
        }

        private class SingleStep
        {
            public string Productname { get; private set; }
            public string Plangroup { get; private set; }
            public string Techstage { get; private set; }
            public string Subplan { get; private set; }
            public string Stepname { get; private set; }
            public string Recipe { get; private set; }
            public int StepSequence { get; private set; }
            public string IRDGroup { get; set; }

            public string ToolGroup { get; set; }

            private readonly char[] digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            public string IRDStep => Techstage.TrimEnd(digits) + " " + Subplan.TrimEnd(digits);

            public SingleStep(string headerLine, string dataLine)
            {
                string[] headers = headerLine.Trim(',').Split(',');
                string[] data = dataLine.Trim(',').Split(',');

                for (int i = 0; i < data.Length; i++)
                {
                    if (headers[i] == "PRODUCTNAME") { Productname = data[i]; }
                    if (headers[i] == "PLANGROUP") { Plangroup = data[i]; }
                    if (headers[i] == "TECHSTAGE") { Techstage = data[i]; }
                    if (headers[i] == "SUBPLAN") { Subplan = data[i]; }
                    if (headers[i] == "STEPNAME") { Stepname = data[i]; }
                    if (headers[i] == "RECIPE") { Recipe = data[i]; }
                    if (headers[i] == "STEPSEQUENCE") { StepSequence = int.Parse(data[i]); }
                }
            }
        }

        public class IRDMapping
        {
            public string Techstage { get; set; }
            public string Subplan { get; set; }
            public string IRDGroup { get; set; }
            public string WorkStation { get; set; }

            public IRDMapping(string headerLine, string dataLine)
            {
                string[] headers = headerLine.Trim(',').Split(',');
                string[] data = dataLine.Trim(',').Split(',');

                for (int i = 0; i < data.Length; i++)
                {
                    if (headers[i] == "TECHSTAGE") { Techstage = data[i]; }
                    if (headers[i] == "SUBPLAN") { Subplan = data[i]; }
                    if (headers[i] == "IRDNAME") { IRDGroup = data[i]; }
                    if (headers[i] == "SUMMARYGROUP") { WorkStation = data[i]; }
                }
            }
        }
    }
}
