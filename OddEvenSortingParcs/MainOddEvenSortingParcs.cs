using Parcs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OddEvenSortingParcs
{
    class MainOddEvenSortingParcs: MainModule
    {
        private static CommandLineOptions options;

        static void Main(string[] args)
        {
            options = new CommandLineOptions();

            if (args != null)
            {
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    throw new ArgumentException($@"Cannot parse the arguments. Possible usages: {options.GetUsage()}");
                }
            }

            if (!File.Exists(options.InputFile))
            {
                throw new ArgumentException("Input file doesn't exist");
            }

            (new MainOddEvenSortingParcs()).RunModule(options);
        }

        public double[] ReadArray(string fileName)
        {
            return File.ReadLines(fileName)
                    .SelectMany(line => line.Split()
                    .Select(s => (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double d), d)))
                    .Where(n => n.Item1)
                    .Select(n => n.Item2)
                    .ToArray();
        }

        public override void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            var sw = new Stopwatch();
            double[] A = ReadArray(options.InputFile);
            Console.WriteLine($"Processing array of {A.Length} elements");
            sw.Start();
            int pointsNum = options.PointsNum;

            int chunkSize = A.Length / pointsNum + 1;

            var points = new IPoint[pointsNum];
            var channels = new IChannel[pointsNum];
            for (int i = 0; i < pointsNum; ++i)
            {
                points[i] = info.CreatePoint();
                channels[i] = points[i].CreateChannel();
                points[i].ExecuteClass("OddEvenSortingParcs.OddEvenSorting");
            }

            for (int i = 0; i < channels.Length; i++)
            {
                Console.WriteLine($"Sent to channel: {i}");
                double[] chunk = A.Skip(i * chunkSize).Take(Math.Min(chunkSize, A.Length - chunkSize * i)).ToArray();
                channels[i].WriteObject(chunk);
            }

            for (int i = 0; i < channels.Length; i++)
            {
                if (i % 2 == 1)
                {
                    for (int prId = 0; prId < channels.Length - 1; prId += 2)
                    {
                        var arr1 = channels[prId].ReadObject<double[]>();
                        var arr2 = channels[prId+1].ReadObject<double[]>();
                        channels[prId].WriteObject(arr2);
                        channels[prId].WriteData(false);
                        channels[prId + 1].WriteObject(arr1);
                        channels[prId + 1].WriteData(true);
                    }
                }
                else
                {
                    for (int prId = 1; prId < channels.Length - 1; prId += 2)
                    {
                        var arr1 = channels[prId].ReadObject<double[]>();
                        var arr2 = channels[prId + 1].ReadObject<double[]>();
                        channels[prId].WriteObject(arr2);
                        channels[prId].WriteData(false);
                        channels[prId + 1].WriteObject(arr1);
                        channels[prId + 1].WriteData(true);
                    }
                }                
            }

            var result = new List<double>();

            foreach (var channel in channels)
            {
                result.AddRange(channel.ReadObject<double[]>());
            }
            sw.Stop();

            var sb = new StringBuilder();
            foreach (var d in result)
            {
                sb.Append(d.ToString(CultureInfo.InvariantCulture) + " ");
            }

            File.WriteAllText(options.OutputFile, sb.ToString());
            Console.WriteLine("Done");
            Console.WriteLine($"Total time {sw.ElapsedMilliseconds} ms ({sw.ElapsedTicks} ticks)");
        }
    }
}
