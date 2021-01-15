using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Examples.AccessController
{
    public class Customer
    {
        public int Type { get; set; }

        public Customer(int type)
        {
            Type = type;
        }

        public int GetReward()
        {
            switch (Type)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 4;
                case 3: return 8;
            }
            throw new Exception();
        }
    }
}
