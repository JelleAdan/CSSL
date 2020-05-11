using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabGUI.Models
{
    public class LotStartQty
    {
        public LotType LotType { get; set; }
        public int Quantity{ get; set; }

        public LotStartQty(LotType lotType, int quantity)
        {
            LotType = lotType;
            Quantity = quantity;
        }
    }
}
