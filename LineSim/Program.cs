//using CSSL.Examples.AssemblyLineOld;
//using CSSL.Examples.AssemblyLineOld.AssemblyLineObservers;
using CSSL.Calendar;
using CSSL.Examples.AssemblyLine;
using CSSL.Examples.AssemblyLine.Observers;
using CSSL.Modeling;
using CSSL.Modeling.CSSLQueue;
using CSSL.Utilities.Distributions;
using System;

namespace LineSim
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings.FixSeed = true;

            Simulation sim = new Simulation("AssemblyLineSimulation", @"C:\CSSLtest");

            SimpleCalendar calendar = new SimpleCalendar();
            sim.MyExecutive.SetCalendar(calendar);

            AssemblyLine assemblyLine = new AssemblyLine(sim.MyModel, "AssemblyLine", 3);

            assemblyLine.AddMachine(0, 5, new NormalDistribution(5, 1), new NormalDistribution(5, 1));
            assemblyLine.AddMachine(1, 2, new NormalDistribution(5, 1), new NormalDistribution(5, 1));
            assemblyLine.AddMachine(2, 3, new NormalDistribution(5, 1), new NormalDistribution(5, 1));

            assemblyLine.AddBuffer(1, 1);
            assemblyLine.AddBuffer(2, 10);

            AssemblyLineObserver observer = new AssemblyLineObserver(sim);
            assemblyLine.Subscribe(observer);

            sim.MyExperiment.LengthOfReplication = 1E5;
            sim.MyExperiment.NumberOfReplications = 2;

            sim.Run();
        }
    }
}
