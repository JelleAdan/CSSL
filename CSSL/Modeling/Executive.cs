using CSSL.Calendar;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSSL.Modeling.Elements.CSSLEvent;

namespace CSSL.Modeling
{
    public class Executive
    {
        public double SimulationClockTime { get; private set; }
        public double WallClockTime => eventExecutionProcess.GetElapsedWallClockTimeSeconds;

        private ICalendar calendar;

        private EventExecutionProcess eventExecutionProcess;

        public Executive(Simulation simulation)
        {
            MySimulation = simulation;
            calendar = new SimpleCalendar();
            eventExecutionProcess = new EventExecutionProcess(this);
        }

        public Executive(Simulation simulation, ICalendar calendar)
        {
            MySimulation = simulation;
            this.calendar = calendar;
            eventExecutionProcess = new EventExecutionProcess(this);
        }

        public Simulation MySimulation { get; }

        public void TryInitialize()
        {
            eventExecutionProcess.TryInitialize();
        }

        public void TryRunAll()
        {
            eventExecutionProcess.TryRunAll();
        }

        public void Execute(CSSLEvent e)
        {
            SimulationClockTime = e.Time;
            e.Execute();
        }

        internal void HandleEndEvent(CSSLEvent csslevent)
        {
            eventExecutionProcess.Stop();
        }

        internal void ScheduleEvent(double time, CSSLEventAction action)
        {
            CSSLEvent e = new CSSLEvent(time, action);
            calendar.Add(e);
        }

        private class EventExecutionProcess : IterativeProcess<CSSLEvent>
        {
            private Executive executive;

            public EventExecutionProcess(Executive executive)
            {
                this.executive = executive;
            }


            protected override double maxWallClockTime
            {
                get
                {
                    double maxCompTimePerReplication = executive.MySimulation.MyExperiment.LengthOfReplicationWallClock;

                    return maxCompTimePerReplication == double.PositiveInfinity ? executive.MySimulation.MyExperiment.MaxWallClockTimeTotal : maxCompTimePerReplication;
                }
            }

            protected override bool HasNext => executive.calendar.HasNext();

            protected sealed override void DoInitialize()
            {
                base.DoInitialize();
                executive.calendar.CancelAll();
                executive.SimulationClockTime = 0;
                executive.MySimulation.MyExperiment.CreateReplicationOutputDirectory();

                if(executive.MySimulation.MyExperiment.LengthOfReplicationSimulationClock != double.PositiveInfinity)
                {
                    ScheduleEndEvent();
                }
            }

            private void ScheduleEndEvent()
            {
                executive.ScheduleEvent(executive.MySimulation.MyExperiment.LengthOfReplicationSimulationClock, executive.HandleEndEvent);
            }

            protected sealed override CSSLEvent NextIteration()
            {
                return executive.calendar.Next();
            }

            protected sealed override void RunIteration()
            {
                CSSLEvent e = NextIteration();
                executive.Execute(e);
            }

            protected sealed override void DoEnd()
            {
                base.DoEnd();

                Console.WriteLine("Ended replication in state: " + MyEndStateIndicator.ToString());
            }
        }
    }
}
