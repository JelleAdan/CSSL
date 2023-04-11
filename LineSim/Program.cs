using CSSL.Examples.AssemblyLine;
using CSSL.Examples.AssemblyLine.Observers;
using CSSL.Modeling;
using CSSL.Utilities.Distributions;

Settings.FixSeed = true;

Simulation sim = new Simulation("AssemblyLineSimulation", @"C:\CSSLtest");

AssemblyLine assemblyLine = new AssemblyLine(sim.MyModel, "AssemblyLine", 3);

double[] machine1Uptimes = new double[2] { 2, 3 };
double[] machine1Downtimes = new double[3] { 1, 2, 3 };
assemblyLine.AddMachine(0, 1500, new EmpericalDistribution(machine1Uptimes), new EmpericalDistribution(machine1Downtimes));

double[] machine2Uptimes = new double[2] { 20, 30 };
double[] machine2Downtimes = new double[2] { 1, 2 };
assemblyLine.AddMachine(1, 4, new EmpericalDistribution(machine2Uptimes), new EmpericalDistribution(machine2Downtimes));

double[] machine3Uptimes = new double[2] { 2, 3 };
double[] machine3Downtimes = new double[3] { 1, 2, 3 };
assemblyLine.AddMachine(2, 1200, new EmpericalDistribution(machine3Uptimes), new EmpericalDistribution(machine3Downtimes));

assemblyLine.AddBuffer(1, 10); // Position, capacity
assemblyLine.AddBuffer(2, 1);

AssemblyLineObserver observer = new AssemblyLineObserver(sim);
assemblyLine.Subscribe(observer);

sim.MyExperiment.LengthOfReplication = 1E6;
sim.MyExperiment.NumberOfReplications = 2;

sim.Run();