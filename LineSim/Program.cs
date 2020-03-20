using CSSL.Examples.AssemblyLineOld;
using CSSL.Examples.AssemblyLineOld.AssemblyLineObservers;
using CSSL.Modeling;
using CSSL.Utilities.Distributions;
using System;

namespace LineSim
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation sim = new Simulation("AssemblyLineSimulation", @"C:\CSSLtest");

            AssemblyLine assemblyLine = new AssemblyLine(sim.MyModel, "AssemblyLine", 3);

            assemblyLine.AddMachine(0, 5, new NormalDistribution(5, 1), new NormalDistribution(5, 1));
            assemblyLine.AddMachine(1, 4, new NormalDistribution(5, 1), new NormalDistribution(5, 1));
            assemblyLine.AddMachine(2, 1, new NormalDistribution(5, 1), new NormalDistribution(5, 1));

            assemblyLine.AddBuffer(1, 100);
            assemblyLine.AddBuffer(2, 100);

            AssemblyLineObserver observer = new AssemblyLineObserver(sim);
            assemblyLine.Subscribe(observer);

            sim.MyExperiment.LengthOfReplication = 1000;
            sim.MyExperiment.NumberOfReplications = 2;

            sim.Run();
        }
    }
}
