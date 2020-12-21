using CSSL.Examples.WaferFab;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WaferFabSim.SnapshotData;
using static WaferFabSim.InputDataConversion.AutoDataReader;

namespace WaferFabSim.Import
{
    [Serializable]
    public class LotTraces
    {
        public LotTraces(List<LotTrace> lotTraces, DateTime startDate, DateTime endDate)
        {
            All = lotTraces;
            StartDate = startDate;
            EndDate = endDate;
            lotActivities = new List<LotActivity>();

            foreach (LotTrace trace in lotTraces)
            {
                if (trace.ParentTraces != null)
                {
                    throw new Exception("LotTrace is already added to other LotTraces collection. This can only be an 1:m relation.");
                }
                else
                {
                    trace.ParentTraces = this;
                }
            }
        }

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public List<LotTrace> All { get; private set; }
        public List<LotTrace> NoEnd => All.Where(x => x.HasProcessPlan && !x.HasEnd).ToList();
        public List<LotTrace> NoStart => All.Where(x => x.HasProcessPlan && !x.HasStart).ToList();
        public List<LotTrace> UnknownProcessPlan => All.Where(x => !x.HasProcessPlan).ToList();
        public List<LotTrace> Terminated => All.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivitiesRaw.Last().Status == "Terminated").ToList();
        public List<LotTrace> Hold => All.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivitiesRaw.Last().Status == "Hold").ToList();
        public List<LotTrace> Active => All.Where(x => x.HasProcessPlan && !x.HasEnd && x.LotActivitiesRaw.Last().Status == "Active").ToList();

        private List<LotActivity> lotActivities { get; set; }

        public List<LotActivity> LotActivities
        {
            get
            {
                if (!lotActivities.Any())
                {
                    ExtractLotActivities();
                }
                return lotActivities;
            }
        }

        public List<Tuple<DateTime, RealLot>> GetRealLotStarts()
        {
            List<Tuple<DateTime, RealLot>> lotStarts = new List<Tuple<DateTime, RealLot>>();

            foreach (LotTrace trace in All.Where(x => x.HasStart && x.LotActivities.Any() && x.LotActivitiesRaw.Any()))
            {
                LotActivity activity = trace.LotActivities.First();

                LotActivityRaw raw = trace.LotActivitiesRaw.First();

                RealLot newLot = new RealLot(activity, raw, trace.StartDate);

                lotStarts.Add(new Tuple<DateTime, RealLot>(trace.StartDate, newLot));
            }

            return lotStarts;
        }

        private void ExtractLotActivities()
        {
            foreach (LotTrace trace in All)
            {
                List<LotActivityRaw> rawList = trace.LotActivitiesRaw;

                // Create activity
                LotActivity newActivity = new LotActivity(rawList[0]);
                string currentWorkstation = rawList[0].WorkStation;

                // Set arrival time if first step
                if (trace.HasStart)
                {
                    newActivity.Arrival = rawList[0].TrackIn;
                }

                // Loop over lot activities and stop if Workstation changed
                for (int i = 0; i < rawList.Count(); i++)
                {
                    if (currentWorkstation != rawList[i].WorkStation)
                    {
                        // Set time of event to Trackout previous step. If that is null, then to TrackIn of current step.
                        DateTime? timeOfEvent = rawList[i - 1].TrackOut != null ? rawList[i - 1].TrackOut : timeOfEvent = rawList[i].TrackIn;

                        newActivity.Departure = timeOfEvent;
                        lotActivities.Add(newActivity);
                        trace.LotActivities.Add(newActivity);

                        newActivity = new LotActivity(rawList[i]);
                        newActivity.Arrival = timeOfEvent;
                        currentWorkstation = rawList[i].WorkStation;
                    }

                    newActivity.AddLotActivityRaw(rawList[i]);
                }

                // Set departure time if last step on process plan
                if (trace.HasEnd)
                {
                    newActivity.Departure = rawList.Last().TrackOut;
                }

                // Save
                if (trace.Status != "Terminated" && trace.Status != "Hold")
                {
                    lotActivities.Add(newActivity);
                    trace.LotActivities.Add(newActivity);
                }
            }
        }

        public RealSnapshot GetWIPSnapshot(DateTime time, int waferQtyThreshold)
        {
            if (time < StartDate || time > EndDate)
            {
                throw new Exception($"WIP snapshot cannot be generated for time {time}, it is is out of bounds of these LotTraces");
            }
            else
            {
                List<RealLot> lots = new List<RealLot>();

                foreach (LotTrace trace in All)
                {
                    // Make sure that lot activities are ordered on track-in
                    //trace.LotActivitiesRaw = trace.LotActivitiesRaw.OrderBy(x => x.TrackIn).ToList();

                    if (trace.LotActivities.Any() && time >= trace.StartDate && time < trace.EndDate)
                    {
                        // Find current LotActivity and RawActivity
                        LotActivity activity = trace.GetLotActivityAt(time);

                        LotActivityRaw raw = trace.GetLotActivityRawAt(time);

                        if (activity != null && raw != null)
                        {
                            RealLot newLot = new RealLot(activity, raw, time);

                            lots.Add(newLot);
                        }
                    }
                }

                RealSnapshot snapshot = new RealSnapshot(lots, waferQtyThreshold);

                return snapshot;
            }
        }

        public List<string> GetProductTypes()
        {
            return All.Select(x => x.ProductType).Distinct().ToList();
        }

        public List<Tuple<string, int>> GetProductTypesAndCount()
        {
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();

            foreach (string type in GetProductTypes())
            {
                list.Add(new Tuple<string, int>(type, All.Where(x => x.ProductType == type).Count()));
            }

            return list.OrderByDescending(x => x.Item2).ToList();
        }


        public void WriteToCSV(string productType, string fileName)
        {
            List<LotTrace> traces = All.Where(x => x.ProductType == productType).ToList();

            using (StreamWriter writer = new StreamWriter(fileName))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var trace in traces)
                {
                    csv.WriteRecords(trace.LotActivitiesRaw);
                }
            }

            int stop = 0;
        }

        public void WriteToCSV(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var trace in All)
                {
                    csv.WriteRecords(trace.LotActivitiesRaw);
                }
            }
        }

        public void WriteLotActivitiesToCSV(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(LotActivities);
            }
        }

        public sealed class LotActivityRawMap : ClassMap<LotActivityRaw>
        {
            public LotActivityRawMap()
            {
                AutoMap(CultureInfo.InvariantCulture);
                Map(m => m.SplitId).TypeConverter(new StringConverter());
            }
        }

        [Serializable]
        public class LotTrace
        {
            public LotTraces ParentTraces { get; set; }
            public string ProductType { get; set; }
            public string LotId { get; private set; }
            public bool IsComplete => HasProcessPlan && HasEnd && HasStart;
            public DateTime StartDateParent => ParentTraces.StartDate;
            public DateTime EndDateParent => ParentTraces.EndDate;
            public bool HasStart { get; set; }
            public bool HasEnd { get; set; }
            public bool HasProcessPlan { get; set; }

            /// <summary>
            /// First arrival (on workcenter level) time stamp. If null, then first arrival is the StartDate of LotTraces.
            /// </summary>
            public DateTime StartDate => LotActivities.Any() && LotActivities.First().Arrival != null ? (DateTime)LotActivities.First().Arrival : StartDateParent;

            /// <summary>
            /// Last departure (on workcenter level) time stamp. If null, then last departure is the EndDate of LotTraces
            /// </summary>
            public DateTime EndDate => LotActivities.Any() && LotActivities.Last().Departure != null ? (DateTime)LotActivities.Last().Departure : EndDateParent;

            public string Status => LotActivitiesRaw.Any() ? LotActivitiesRaw.Last().Status : "NoLotActivities";

            /// <summary>
            /// LotActivities ordered on arrival time, do not alter this list without making sure ordering stays intact.
            /// </summary>
            public List<LotActivity> LotActivities { get; set; }

            /// <summary>
            /// Ordered by track-in time, do not alter this list without making sure this ordering stays intact.
            /// </summary>
            public List<LotActivityRaw> LotActivitiesRaw { get; set; }

            public ProcessPlan ProcessPlan { get; set; }

            public LotActivity GetLotActivityAt(DateTime time)
            {
                if (time >= StartDate && time <= EndDate)
                {
                    foreach (LotActivity activity in LotActivities)
                    {
                        if ((activity.Arrival == null || time >= activity.Arrival) && (activity.Departure == null || time < activity.Departure))
                        {
                            return activity;
                        }
                    }
                    throw new Exception("LotActivity not found for this time, but time is within start and end date.");
                }
                else
                {
                    throw new Exception($"Time {time} is outside range of known lot activites for lot {LotId}.");
                }
            }

            public LotActivityRaw GetLotActivityRawAt(DateTime time)
            {
                if (time >= StartDate && time <= EndDate)
                {
                    for (int i = 0; i < LotActivitiesRaw.Count; i++)
                    {
                        LotActivityRaw activity = LotActivitiesRaw[i];

                        if (activity.TrackOut > time)
                        {
                            return LotActivitiesRaw[i];
                        }
                    }
                    Console.WriteLine($"WARNING: LotActivityRaw for {LotActivitiesRaw.First().LotId} not found for time {time}, but time is within start and end date.");
                    return null;
                }
                else
                {
                    throw new Exception($"Time {time} is outside range of known lot activites for lot {LotId}.");
                }
            }

            public void MapPPStepSequencesOnActivites(Dictionary<string, ProcessPlan> processPlans)
            {
                if (processPlans.ContainsKey(ProductType))
                {
                    ProcessPlan = processPlans[ProductType];

                    foreach (LotActivityRaw activity in LotActivitiesRaw)
                    {
                        List<SingleStep> steps = ProcessPlan.Steps.Where(x => activity.Stepname == x.Stepname && activity.Techstage == x.Techstage && activity.Subplan == x.Subplan).ToList();

                        // One step of process plans matches the activity
                        if (steps.Count() == 1)
                        {
                            SingleStep step = steps.First();
                            activity.PPStepSequence = step.StepSequence;
                        }
                        // No step of process plan matches the acitivy, so PPStepSequence is set to -1
                        else if (!steps.Any())
                        {
                            activity.PPStepSequence = -1;
                        }
                        // Multiple steps of process plan match the activity, so exception is thrown
                        else if (steps.Count() > 1)
                        {
                            throw new Exception($"Multiple steps found in process plan {steps.First().Productname} with same Stepname {activity.Stepname}, " +
                                $"Techstage {activity.Techstage} and Subplan {activity.Subplan} as activity {activity.LotId}");
                        }
                    }
                }
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

                if (!LotActivitiesRaw.Where(x => x.Stepname == firstStep.Stepname).Any())
                {
                    HasStart = false;
                }

                // Check whether trace contains last step
                SingleStep lastStep = ProcessPlan.Steps.Last();

                if (!LotActivitiesRaw.Where(x => x.Stepname == lastStep.Stepname).Any())
                {
                    HasEnd = false;
                }

                // Passed all checks, so trace is complete
                return;
            }

            public void CalculateTimeInSteps()
            {
                LotActivityRaw lastActivity = null;

                foreach (LotActivityRaw a in LotActivitiesRaw)
                {
                    // Time in step
                    if (a.TrackOut != null)
                    {
                        a.TimeInStep = a.TrackOut - a.TrackIn;
                    }
                    else
                    {
                        a.TimeInStep = null;
                    }

                    // Time before step and after step
                    if (lastActivity != null && lastActivity.TrackOut != null)
                    {
                        a.TimeBeforeStep = a.TrackIn - lastActivity.TrackOut;
                        lastActivity.TimeAfterStep = a.TimeBeforeStep;
                    }

                    lastActivity = a;
                }
            }

            public LotTrace(string lotid)
            {
                LotActivitiesRaw = new List<LotActivityRaw>();
                LotActivities = new List<LotActivity>();
                LotId = lotid;
            }
        }
    }
}
