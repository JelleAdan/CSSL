using CSSL.Modeling;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSSL.Reporting
{
    public class SimulationReporter
    {
        private readonly Simulation simulation;
        private readonly Model model;
        private readonly Experiment experiment;

        public SimulationReporter(Simulation simulation)
        {
            this.simulation = simulation ?? throw new ArgumentNullException("simulation", "Cannot make simulation reporter since simulation is null.")
;
            model = simulation.MyModel;
            experiment = simulation.MyExperiment;
        }

        public void PrintSummaryToConsole()
        {

        }

        public void PrintSummaryToFile()
        {
            string outputDirectory = experiment.ExperimentOutputDirectory + @"\Summary.txt";

            using (StreamWriter writer = new StreamWriter(outputDirectory))
            {
                writer.WriteLine("Summary of this experiment");

            }
        }
    }
}
