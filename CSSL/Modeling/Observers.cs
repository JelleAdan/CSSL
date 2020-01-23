using CSSL.Modeling.Elements;
using CSSL.Observer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Modeling
{
    internal class Observers : IDisposable
    {
        internal List<ObserverBase<ModelElementBase>> ModelElementObservers { get; set; }
        internal List<ObserverBase<Executive>> ExecutiveObservers { get; set; }


        void IDisposable.Dispose()
        {
            foreach (var observer in ModelElementObservers)
            {
                observer.Dispose();
            }

            foreach (var observer in ExecutiveObservers)
            {
                observer.Dispose();
            }

            // Append this in case of more observer lists
        }

        internal void Add(ObserverBase<T> observer)
        {

        }
    }
}
