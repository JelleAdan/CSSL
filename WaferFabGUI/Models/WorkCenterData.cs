using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabGUI.Models
{
    public class WorkCenterData
    {
        public string Name { get; set; }

        public double MeanProcessingTime { get; set; }

        public double ExponentialRate => 1 / MeanProcessingTime;

        public WorkCenterData(string name, double mean)
        {
            Name = name;
            MeanProcessingTime = mean;
        }
    }
}
