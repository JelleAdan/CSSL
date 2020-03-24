using System;
using CSSL.Examples.DataCenterSimulation;
using CSSL.Examples.DataCenterSimulation.DataCenterObservers;
using CSSL.Modeling;
using CSSL.Reporting;
using CSSL.Utilities.Distributions;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CSSL.Examples.WaferFab;
using WaferFab.DataConversion;

namespace WaferFab
{
    public class Program
    {
        static void Main(string[] args)
        {
            string directory = @"C:\Users\nx008314\OneDrive - Nexperia\Work\WaferFab";

            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            LotStepGrouping.LoadData(directory);

            int stop = 0;
        }
    }
}
