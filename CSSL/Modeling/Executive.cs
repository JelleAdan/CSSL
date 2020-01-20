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
        public double Time { get; private set; }

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

            public EventExecutionProcess(Executive executive)
            {
                this.executive = executive;
            }

            protected override double maxComputationalTimeMiliseconds => executive.MySimulation.MyExperiment.MaxComputationalTimePerReplication;

            protected override bool HasNext => executive.calendar.HasNext();

            protected sealed override void DoInitialize()
            {
                base.DoInitialize();
                executive.calendar.CancelAll();
                executive.Time = 0;
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

            }
        }
    }
}
