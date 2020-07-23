using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    [Serializable]
    public class LotStep : IIdentity, IName
    {
        public WorkCenter WorkCenter { get; private set; }

        public int Id { get; }

        public string Name { get; }

        public void SetWorkCenter(WorkCenter workCenter)
        {
            WorkCenter = workCenter;
        }

        public LotStep(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public LotStep()
        {
        }
    }
}
