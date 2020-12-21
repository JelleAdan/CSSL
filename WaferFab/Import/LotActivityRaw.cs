using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabSim.Import
{
    [Serializable]
    /// <summary>
    /// Raw Lot Activity per single step as recorded in factory
    /// </summary>
    public class LotActivityRaw
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
        public DateTime? TrackOut { get; set; }
        public int QtyIn { get; set; }
        public int QtyOut { get; set; }
        public string Status { get; set; }
        public string IRDGroup { get; set; }
        public string WorkStation { get; set; }
        public int PPStepSequence { get; set; }
        public TimeSpan? TimeInStep { get; set; }
        public TimeSpan? TimeBeforeStep { get; set; }
        public TimeSpan? TimeAfterStep { get; set; }

        public LotActivity LotActivity { get; set; }

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
                if (headers[i] == "RECIPE") { Recipe = data[i]; }
                if (headers[i] == "FW_STEPNAME") { Stepname = data[i]; }
                if (headers[i] == "FW_LOCATION") { Location = data[i]; }
                if (headers[i] == "FW_TRACKINTIME") { TrackIn = Convert.ToDateTime(data[i]); }
                if (headers[i] == "FW_TRACKOUTTIME")
                {
                    //if (data[i] != "1/0/1900 0:00:00" && data[i] != "1/0/1900 0:00")
                    if (data[i] != "1/0/1900 0:00:00")
                    {
                        TrackOut = Convert.ToDateTime(data[i]);
                    }
                    else
                    {
                        TrackOut = null;
                    }
                }
                if (headers[i] == "FW_STEPQTYIN") { QtyIn = Convert.ToInt32(data[i]); }
                if (headers[i] == "FW_STEPQTYOUT") { QtyOut = Convert.ToInt32(data[i]); }
                if (headers[i] == "FW_CURRENTSTATUS") { Status = data[i]; }
            }
        }
    }

}
