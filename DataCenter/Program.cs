using CSSL.Examples.DataCenterSimulation;
using CSSL.Examples.DataCenterSimulation.DataCenterObservers;
using CSSL.Modeling;
using CSSL.Reporting;
using CSSL.Utilities.Distributions;

Simulation sim = new Simulation("SomeSimulation", @"C:\CSSL");

Settings.WriteOutput = false;

Settings.NotifyObservers = true;

// Parameters...

double dispatchTime = 1E-3;
double lambda = 100;
int numberServerpools = 10;
int numberServerpoolsToChooseFrom = 10;

// The experiment part...

sim.MyExperiment.NumberOfReplications = 3;
sim.MyExperiment.LengthOfReplication = 10;
sim.MyExperiment.LengthOfWarmUp = 2;

// The model part...

DataCenter dataCenter = new DataCenter(sim.MyModel, "DataCenter");

for (int i = 0; i < numberServerpools; i++)
{
    dataCenter.AddServerpool(new ServerPool(dataCenter, $"Serverpool_{i}"));
}

Dispatcher dispatcher = new Dispatcher(dataCenter, "Dispatcher", new ExponentialDistribution(1), double.PositiveInfinity, dataCenter.ServerPools, dispatchTime, numberServerpoolsToChooseFrom);
dataCenter.SetDispatcher(dispatcher);

JobGenerator jobGenerator = new JobGenerator(dataCenter, "JobGenerator", new ExponentialDistribution(lambda), dispatcher);
dataCenter.SetJobGenerator(jobGenerator);

// The observer part...

DispatcherObserver dispatcherObserver = new DispatcherObserver(sim);
dispatcher.Subscribe(dispatcherObserver);

DataCenterObserver dataCenterObserver = new DataCenterObserver(sim);
dataCenter.Subscribe(dataCenterObserver);

foreach (ServerPool serverpool in dataCenter.ServerPools)
{
    ServerPoolObserver serverpoolObserver = new ServerPoolObserver(sim);
    serverpool.Subscribe(serverpoolObserver);
}

// Run...

sim.Run();

// The reporting part...

SimulationReporter reporter = sim.MakeSimulationReporter();

reporter.PrintSummaryToFile();
reporter.PrintSummaryToConsole();