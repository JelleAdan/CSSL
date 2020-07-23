using CSSL.Examples.WaferFab;
using System;
using System.Collections.Generic;
using System.Text;

namespace WaferFabGUI.Models
{
    public class LotStartQty
    {
        public string LotType { get; set; }
        public int Quantity{ get; set; }

        public LotStartQty(string lotType, int quantity)
        {
            LotType = lotType;
            Quantity = quantity;
        }
    }
}
