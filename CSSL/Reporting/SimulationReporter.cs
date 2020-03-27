using CSSL.Modeling;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSSL.Reporting
{
    public class SimulationReporter
    {
        private readonly Simulation simulation;
        private readonly Model model;
        private readonly Experiment experiment;

        private List<string> summary { get; set; }

        public SimulationReporter(Simulation simulation)
        {
            this.simulation = simulation ?? throw new ArgumentNullException("simulation", "Cannot make simulation reporter since simulation is null.")
;
            model = simulation.MyModel;
            experiment = simulation.MyExperiment;
            summary = new List<string>();
        }


        private void BuildSummary()
        {
            summary.Add($"\nSIMULATION REPORT FOR { simulation.Name}\n");

            summary.Add("EXPERIMENT");
            summary.Add($"Maximum number of replications: {experiment.NumberOfReplications}");
            summary.Add($"Maximum computational time per replication: {experiment.LengthOfReplicationWallClock} s");
            summary.Add($"Maximum total computational time: {experiment.LengthOfExperimentWallClock} s");
            summary.Add($"Length of warm-up: {experiment.LengthOfWarmUp} s");
            summary.Add($"Length of replication: {experiment.LengthOfReplication} s\n");

            summary.Add("EXECUTION SUMMARY");
            summary.Add($"Number of replications: {experiment.GetCurrentReplicationNumber()}");
            summary.Add($"Stopped in state: {simulation.GetEndStateIndicator}");
            TimeSpan time = simulation.GetWallClockTimeSpan;
            summary.Add($"Total computational time:{time.Days}d:{time.Hours}h:{time.Minutes}m:{time.Seconds}s:{time.Milliseconds}ms");
        }

        public void PrintSummaryToFile()
        {
            if (!summary.Any()) { BuildSummary(); }

            using (StreamWriter writer = new StreamWriter(experiment.ExperimentOutputDirectory + @"\Summary.txt"))
            {
                foreach(string line in summary)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public void PrintSummaryToConsole()
        {
            if (!summary.Any()) { BuildSummary(); }

            foreach(string line in summary)
            {
                Console.WriteLine(line);
            }
        }

    }
}
