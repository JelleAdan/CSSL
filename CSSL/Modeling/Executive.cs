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
        public double PreviousEventTime { get; private set; }

        public double Time { get; private set; }

        public double WallClockTime => eventExecutionProcess.GetWallClockTime;

        private ICalendar calendar;

        public void SetCalendar(ICalendar calendar)
        {
            this.calendar = calendar;
        }

        private EventExecutionProcess eventExecutionProcess;

        internal Executive(Simulation simulation) 
        {
            MySimulation = simulation;
            calendar = new SimpleCalendar();
            eventExecutionProcess = new EventExecutionProcess(this);
        }

        internal Simulation MySimulation { get; set; }

        internal void TryInitialize()
        {
            eventExecutionProcess.TryInitialize();
        }

        internal void TryRunAll()
        {
            eventExecutionProcess.TryRunAll();
        }

        internal void Execute(CSSLEvent e)
        {
            PreviousEventTime = Time;
            Time = e.Time;
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

            internal EventExecutionProcess(Executive executive)
            {
                this.executive = executive;
            }

            protected override double maxWallClockTime
            {
                get
                {
                    double maxCompTimePerReplication = executive.MySimulation.MyExperiment.LengthOfReplicationWallClock;

                    return maxCompTimePerReplication == double.PositiveInfinity ? executive.MySimulation.MyExperiment.LengthOfExperimentWallClock : maxCompTimePerReplication;
                }
            }

            protected override bool HasNext => executive.calendar.HasNext();

            protected sealed override void DoInitialize()
            {
                base.DoInitialize();
                executive.calendar.CancelAll();
                executive.PreviousEventTime = 0;
                executive.Time = 0;
                executive.MySimulation.MyExperiment.CreateReplicationOutputDirectory();

                if (executive.MySimulation.MyExperiment.LengthOfReplication != double.PositiveInfinity)
                {
                    ScheduleEndEvent();
                }
            }

            private void ScheduleEndEvent()
            {
                executive.ScheduleEvent(executive.MySimulation.MyExperiment.LengthOfReplication, executive.HandleEndEvent);
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
