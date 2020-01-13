using CSSL.Modeling;
using CSSL.Modeling.Elements;
using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Observer
{
    public abstract class ObserverBase : IObserver<object>
    {
        public abstract void OnCompleted();
        public abstract void OnError(Exception error);
        public abstract void OnNext(object value);

        private IDisposable cancellation;

        public void Subscribe(ModelElement modelElement)
        {
            cancellation = modelElement.Subscribe(this);
        }

        public void Unsubscribe()
        {
            cancellation.Dispose();
        }
    }
}
