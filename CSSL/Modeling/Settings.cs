using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Modeling
{
    public static class Settings
    {


        public static bool FixSeed
        {
            get
            {
                return FixSeed;
            }
            set
            {
                if (value)
                {
                    SeedGenerator = new Random(200844210);
                }
                else
                {
                    SeedGenerator = new Random();
                }
                FixSeed = value;
            }
        }

        public static bool Verbose { get; set; }

        public static bool WriteOutput { get; set; } = true;

        public static bool NotifyObservers { get; set; } = true;

        public static Random SeedGenerator { get; set; } = new Random();
    }
}
