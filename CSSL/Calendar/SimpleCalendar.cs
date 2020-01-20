using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSSL.Modeling;
using CSSL.Modeling.Elements;

namespace CSSL.Calendar
{
    public class SimpleCalendar : ICalendar
    {
        private List<CSSLEvent> fes = new List<CSSLEvent>();

        public void Add(CSSLEvent e)
        {
            if (fes.Count == 0)
            {
                fes.Add(e);
            }
            else
            {
                for (int i = 0; i <= fes.Count; i++)
                {
                    if (i == fes.Count)
                    {
                        fes.Add(e);
                        break;
                    }
                    else if (fes[i].Time > e.Time)
                    {
                        fes.Insert(i, e);
                        break;
                    }
                }
            }
        }

        public void Cancel(CSSLEvent e)
        {
            fes.Remove(e);
        }

        public void CancelAll()
        {
            fes.Clear();
        }

        public bool IsEmpty()
        {
            return !fes.Any();
        }

        public CSSLEvent Next()
        {
            CSSLEvent e = fes.First();
            fes.RemoveAt(0);
            return e;
        }

        public bool HasNext()
        {
            return fes.Any();
        }

        public CSSLEvent PeekNext()
        {
            return fes.First();
        }

        public int Size()
        {
            return fes.Count;
        }
    }
}
