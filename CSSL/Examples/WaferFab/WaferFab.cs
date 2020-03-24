using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.WaferFab
{
    public class WaferFab : ModelElementBase
    {
        public LotGenerator LotGenerator { get; set; }

        public List<WorkCenter> WorkCenters { get; private set; }

        public Dictionary<LotType, Sequence> Sequences { get; set; }

        public Dictionary<LotType, int> LotStarts { get; set; }

        public WaferFab(ModelElementBase parent, string name) : base(parent, name)
        {
        }

        public void SetLotGenerator(LotGenerator lotGenerator)
        {
            LotGenerator = lotGenerator;
        }

        public void AddWorkCenter(WorkCenter workCenter)
        {
            WorkCenters.Add(workCenter);
        }

        public void AddSequence(LotType lotType, Sequence sequence)
        {
            Sequences.Add(lotType, sequence);
        }

        public void AddLotStart(LotType lotType, int quantity)
        {
            LotStarts.Add(lotType, quantity);
        }

    }
}
