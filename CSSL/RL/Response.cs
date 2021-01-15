using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSSL_WebAPI.Models
{
    public class Response
    {
        public double[][] State { get; set; }

        public double Reward { get; set; }

        public bool IsEnded { get; set; }

        public Dictionary<string, string> Info { get; set; }
    }
}
