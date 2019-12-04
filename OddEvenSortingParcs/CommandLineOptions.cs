using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Parcs.Module.CommandLine;

namespace OddEvenSortingParcs
{
    using CommandLine;

    public class CommandLineOptions : BaseModuleOptions
    {
        [Option("input", Required = true, HelpText = "File path to the input array.")]
        public string InputFile { get; set; }
        [Option("output", Required = true, HelpText = "File path to the sorted array.")]
        public string OutputFile { get; set; }
        [Option("p", Required = true, HelpText = "Number of points.")]
        public int PointsNum { get; set; }
    }
}
