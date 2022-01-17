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

        internal void TryEnd()
        {
            eventExecutionProcess.TryEnd();
        }

        internal void Execute(CSSLEvent e)
        {
            if (e.Time < Time)
            {
                throw new Exception("Attempted to execute an event in the past.");
            }
            PreviousEventTime = Time;
            Time = e.Time;
            e.Execute();
        }

        internal void HandleEndEvent(CSSLEvent csslevent)
        {
            eventExecutionProcess.Stop();
        }

        internal void HandlePauseEvent(CSSLEvent csslevent)
        {
            eventExecutionProcess.Pause();
        }

        internal void ScheduleEvent(double time, CSSLEventAction action)
        {
            if (time < Time)
            {
                throw new Exception("Attempted to schedule an event in the past.");
            }
            CSSLEvent e = new CSSLEvent(time, action);
            calendar.Add(e);
        }

        internal void ScheduleEventNow(CSSLEventAction action)
        {
            CSSLEvent e = new CSSLEvent(Time, action);
            calendar.AddNow(e);
        }

        internal void ScheduleEndEvent(double time)
        {
            ScheduleEvent(time, HandleEndEvent);
        }

        internal void ScheduleEndEventNow()
        {
            ScheduleEventNow(HandleEndEvent);
        }

        internal void SchedulePauseEventNow()
        {
            ScheduleEventNow(HandlePauseEvent);
        }

        public bool IsCreated => eventExecutionProcess.IsCreated;

        public bool IsInitialized => eventExecutionProcess.IsInitialized;

        public bool IsRunning => eventExecutionProcess.IsRunning;

        public bool IsPaused => eventExecutionProcess.IsPaused;

        public bool IsEnded => eventExecutionProcess.IsEnded;

        private class EventExecutionProcess : IterativeProcess<CSSLEvent>
        {
            private Executive executive;

            internal EventExecutionProcess(Executive executive)
            {
                this.executive = executive;
            }

            protected override double maxWallClockTime => executive.MySimulation.MyExperiment.LengthOfReplicationWallClock;

            protected override bool HasNext => executive.calendar.HasNext();

            protected sealed override void DoInitialize()
            {
                base.DoInitialize();

                executive.calendar.CancelAll();
                executive.PreviousEventTime = 0;
                executive.Time = 0;
                executive.MySimulation.MyExperiment.StrictlyOnReplicationStart();

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
