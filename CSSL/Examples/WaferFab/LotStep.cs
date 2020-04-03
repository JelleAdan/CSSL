using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class LotStep : IIdentity, IName
    {
        public WorkCenter WorkCenter { get; private set; }

        static int lotStepCount;

        public int Id { get; }

        public string Name { get; }

        public void SetWorkCenter(WorkCenter workCenter)
        {
            WorkCenter = workCenter;
        }

        public LotStep(string name)
        {
            Id = lotStepCount++;
            Name = name;
        }

        public LotStep()
        {
        }
    }
}
