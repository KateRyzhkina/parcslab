using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddEvenSorting
{
    class Program
    {
        static CommandLineOptions options;
        
        static double[] ReadArray(string fileName)
        {
            return File.ReadLines(fileName)
                    .SelectMany(line => line.Split()
                    .Select(s => (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double d), d)))
                    .Where(n => n.Item1)
                    .Select(n => n.Item2)
                    .ToArray();
        }

        static void OddEvenSort(double[] A, int sp)
        {
            int n = A.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = i % sp; j < n - 1; j += sp)
                {
                    if (A[j] > A[j + 1])
                    {
                        double t = A[j];
                        A[j] = A[j + 1];
                        A[j + 1] = t;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            options = new CommandLineOptions();

            if (args != null)
            {
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    throw new ArgumentException($@"Cannot parse the arguments.");
                }
            }

            if (!File.Exists(options.InputFile))
            {
                throw new ArgumentException("Input file doesn't exist");
            }

            double[] A = ReadArray(options.InputFile);
            Console.WriteLine($"Processing array of {A.Length} elements");
            var sw = new Stopwatch();

            sw.Start();
            OddEvenSort(A, 2);
            sw.Stop();

            var sb = new StringBuilder();
            foreach (var d in A)
            {
                sb.Append(d.ToString(CultureInfo.InvariantCulture) + " ");
            }

            File.WriteAllText(options.OutputFile, sb.ToString());
            Console.WriteLine("Done");
            Console.WriteLine($"Total time {sw.ElapsedMilliseconds} ms ({sw.ElapsedTicks} ticks)");
        }
    }
}
