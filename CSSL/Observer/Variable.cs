using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Observer
{
    public class Variable<T>
    {
        public Variable(ObserverBase observer)
        {
            this.observer = observer;
        }

        protected ObserverBase observer;

        /// <summary>
        /// The initial value of the variable. 
        /// </summary>
        public T InitialValue { get; private set; }

        /// <summary>
        /// The current value of the variable. 
        /// At the time of the update it is the new value. 
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// The previou value of the variable.
        /// </summary>
        public T PreviousValue { get; private set; }

        /// <summary>
        /// The time of the update.
        /// At the time of the update it is the current time. 
        /// </summary>
        public double TimeOfUpdate { get; private set; }

        /// <summary>
        /// The time of the previous update.
        /// </summary>
        public double PreviousTimeOfUpdate { get; private set; }

        public double Weight => TimeOfUpdate - PreviousTimeOfUpdate;

        public void UpdateValue(T value)
        {
            PreviousValue = Value;
            Value = value;
            PreviousTimeOfUpdate = TimeOfUpdate;
            TimeOfUpdate = observer.GetTime;
        }

        public void Reset()
        {
            Value = InitialValue;
            PreviousValue = InitialValue;
            TimeOfUpdate = 0;
            PreviousTimeOfUpdate = 0;
        }
    }
}
