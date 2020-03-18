using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class LotStep : IIdentity, IName
    {
        public WorkCenter WorkCenter { get; }

        public int Id { get; }

        public string Name { get; }

        public LotStep(WorkCenter workCenter, int id, string name)
        {
            WorkCenter = workCenter;
            Id = id;
            Name = name;
        }
    }
}
