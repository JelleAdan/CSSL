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

        public Executive()
        {
            calendar = new SimpleCalendar();
        }

        public Executive(ICalendar calendar)
        {
            this.calendar = calendar;
        }

        public void Initialize()
        {
            eventExecutionProcess.TryInitialize();
        }


        internal void ScheduleEvent(double time, CSSLEventAction action)
        {
            CSSLEvent e = new CSSLEvent(time, action);
            calendar.Add(e);
        }

        private class EventExecutionProcess : IterativeProcess<CSSLEvent>
        {
            private ICalendar Calendar;

            public EventExecutionProcess(ICalendar calendar) : base()
            {
                Calendar = calendar;
            }

            protected sealed override void DoInitialize()
            {
                base.DoInitialize();
            }

            protected sealed override void DoRunAll()
            {
                base.DoRunAll();
            }

            protected sealed override void DoRunNext()
            {
                base.DoRunNext();
            }

            protected sealed override void DoEnd()
            {
                base.DoEnd();
            }
        }
    }
}
