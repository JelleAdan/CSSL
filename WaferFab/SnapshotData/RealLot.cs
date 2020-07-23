using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabSim.SnapshotData
{
    [Serializable]
    public class RealLot
    {
        public DateTime SnapshotTime { get; private set; }
        public string IRDGroup { get; private set; }
        public string StepName { get; private set; }
        public string LotID { get; private set; }
        public int Qty { get; private set; }
        public string Location { get; private set; }
        public string DeviceType { get; private set; }
        public string MasksetID { get; private set; }
        public string Technology { get; private set; }
        public DateTime TrackOutInTime { get; private set; }
        public int? PlanDay { get; private set; }
        public string Status { get; private set; }
        public string Speed { get; private set; }
        public int ClipWeek { get; private set; }

        public static int tmpCounter;

        public RealLot(string headerLine, string dataLine)
        {
            string[] headers = headerLine.Trim(',').Split(',');
            string[] data = dataLine.Trim(',').Split(',');

            for (int i = 0; i < data.Length; i++)
            {
                if (headers[i] == "SNAPSHOTTIME") { SnapshotTime = DateTime.Parse(data[i]); }
                if (headers[i] == "IRDGroup") { IRDGroup = data[i]; }
                if (headers[i] == "StepName") { StepName = data[i]; }
                if (headers[i] == "LotID") { LotID = data[i]; }
                if (headers[i] == "Qty") { Qty = Convert.ToInt32(data[i]); }
                if (headers[i] == "Location") { Location = data[i]; }
                if (headers[i] == "DeviceType") { DeviceType = data[i]; }
                if (headers[i] == "MasketID") { MasksetID = data[i]; }
                if (headers[i] == "Technology") { Technology = data[i]; }
                if (headers[i] == "TrackOut/InTime") { TrackOutInTime = DateTime.Parse(data[i]); }
                if (headers[i] == "PlanDay") { if (data[i] == " " || data[i] == "") { PlanDay = null; } else { PlanDay = int.Parse(data[i]); } }
                if (headers[i] == "MasketID") { MasksetID = data[i]; }
                if (headers[i] == "Status") { Status = data[i]; }
                if (headers[i] == "Speed") { Speed = data[i]; }
                if (headers[i] == "ClipWeek") { ClipWeek = Convert.ToInt32(data[i]); }
                if (headers[i] == "MasketID") { MasksetID = data[i]; }
            }
        }

        public Lot ConvertToLot(double creationTime, Dictionary<string, Sequence> sequences)
        {
            if (sequences.ContainsKey(DeviceType))
            {
                Sequence sequence = sequences[DeviceType];

                Lot lot = new Lot(creationTime, sequence);

                lot.PlanDay = PlanDay;
                lot.ClipWeek = ClipWeek;
                lot.LotID = LotID;

                for (int i = 0; i < lot.Sequence.stepCount; i++)
                {
                    if (lot.Sequence.GetCurrentStep(i).Name == IRDGroup)
                    {
                        lot.SetCurrentStepCount(i);
                        break;
                    }
                    if (i == lot.Sequence.stepCount - 1)
                    {
                        lot.SetCurrentStepCount(0);
                        tmpCounter++;
                    }
                }
                return lot;
            }
            else
            {
                throw new Exception($"Process plans does not contain {DeviceType}");
            }
        }
    }
}
