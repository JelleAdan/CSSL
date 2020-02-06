using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Modeling.Elements.Variables
{
    public class Variable<T> : ModelElementBase
    {
        public Variable(ModelElementBase parent, string name) : base(parent, name)
        {
        }

        public T InitialValue { get; private set; }

        public T Value { get; private set; }

        public T PreviousValue { get; private set; }

        public double TimeOfChange { get; private set; }

        public double PreviousTimeOfChange { get; private set; }

        public double Weight => TimeOfChange - PreviousTimeOfChange;

        public void UpdateValue(T value)
        {
            PreviousValue = Value;
            Value = value;
            PreviousTimeOfChange = TimeOfChange;
            TimeOfChange = GetElapsedSimulationClockTime;
            NotifyObservers(this);
        }

        protected override void DoBeforeReplication()
        {
            base.DoBeforeReplication();
            Value = InitialValue;
            PreviousValue = InitialValue;
            TimeOfChange = 0;
        }
    }
}
