using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Utilities
{
    public static class LinqExtensions
    {
        public static double Variance(this IEnumerable<double> source)
        {
            var count = source.Count();

            if (count == 0) return 0;

            var mean = source.Average();
            var squaredDifferences = source.Select(d => (d - mean) * (d - mean));
            return squaredDifferences.Average();
        }

        public static double Variance(this IEnumerable<int> source)
        {
            var count = source.Count();

            if (count == 0) return 0;

            var mean = source.Average();
            var squaredDifferences = source.Select(d => (d - mean) * (d - mean));
            return squaredDifferences.Average();
        }
    }
}
