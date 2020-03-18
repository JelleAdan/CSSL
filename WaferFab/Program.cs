using System;
using CSSL.Examples.DataCenterSimulation;
using CSSL.Examples.DataCenterSimulation.DataCenterObservers;
using CSSL.Modeling;
using CSSL.Reporting;
using CSSL.Utilities.Distributions;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CSSL.Examples.WaferFab;

namespace WaferFab
{
    public class Program
    {
        static void Main(string[] args)
        {
            string directory = @"C:\WaferFab";

            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            LoadData(directory);

            int stop = 0;
        }

        public static void LoadData(string directory)
        {
            List<string> IRDs = new List<string>();

            using (StreamReader reader = new StreamReader(Path.Combine(directory, "IRDRawString.txt")))
            {
                IRDs = ConvertData(reader.ReadToEnd());
            }

            List<string> keywords = new List<string> { "Photo", "Etch", "Ox", "Implant", "Nitride", "Anneal", "Alloy", "Evap", "Dep", "putter" };

            List<List<string>> groupedIRDs = GroupStrings(IRDs, keywords);

            PrintGroupedIRDs(directory, groupedIRDs);
        }

        public static List<List<string>> GroupStrings(List<string> sources, List<string> keywords)
        {
            List<List<string>> grouped = new List<List<string>>();

            foreach (string keyword in keywords)
            {
                List<string> filtered = new List<string>();

                foreach (string source in sources)
                {
                    if (source.Contains(keyword))
                    {
                        filtered.Add(source);
                    }
                }

                grouped.Add(filtered);

                List<string> sourcesCopy = sources.ToList();

                foreach (string source in sourcesCopy)
                {
                    if (grouped.SelectMany(x => x).Contains(source))
                    {
                        sources.Remove(source);
                    }
                }
            }

            grouped.Add(sources);

            return grouped;
        }

        public static void PrintGroupedIRDs(string directory, List<List<string>> groupedIRDs)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(directory, "IRDGrouped.txt"), false))
            {
                foreach (var group in groupedIRDs)
                {
                    foreach (var IRD in group)
                    {
                        writer.WriteLine(IRD);
                    }
                    writer.WriteLine();
                }
            }
        }

        public static List<string> ConvertData(string rawData)
        {
            var splitData = rawData.Split(',').ToList();

            List<string> IRDs = new List<string>();

            foreach (string line in splitData)
            {
                IRDs.Add(line.Trim(new char[] { '\"' }));
            }

            return IRDs;
        }

        //public static Individual RouletteWheel(Population population, Random rnd)
        //{
        //    double total = new double();
        //    foreach (Individual individual in population.Individuals)
        //    {
        //        total += individual.Objective();
        //    }
        //    double value = rnd.NextDouble() * total;
        //    // Locate and return individual
        //    for (int i = 0; i < population.Individuals.Count; i++)
        //    {
        //        value -= population.Individuals[i].Objective();
        //        if (value <= 0)
        //        {
        //            return population.Individuals[i];
        //        }
        //    }
        //    // In case rounding errors occur, meaning that after all fitness values are subtracted value != 0, the last individual is selected
        //    return population.Individuals.Last();
        //}
    }

}
