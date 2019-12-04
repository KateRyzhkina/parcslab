using Parcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OddEvenSortingParcs
{
    class OddEvenSorting : IModule
    {
        private double[] values;
        private int size;

        private void Sort()
        {
            for (int i = 0; i < values.Length - 1; i++)
            {
                for (int j = 0; j < values.Length - 1; j++)
                {
                    if (values[j] > values[j + 1])
                    {
                        var t = values[j];
                        values[j] = values[j + 1];
                        values[j + 1] = t;
                    }
                }
            }
        }

        private void MergeAndSave(double[] other, bool old)
        {
            var list = new List<double>();

            int i = 0, j = 0, total = other.Length + values.Length ;
            while (i < values.Length && j < other.Length)
            {
                if (values[i] < other[j])
                {
                    list.Add(values[i]);
                    i++;
                }
                else
                {
                    list.Add(other[j]);
                    j++;
                }
            }

            while (i < values.Length)
            {
                list.Add(values[i++]);
            }

            while (j < other.Length)
            {
                list.Add(other[j++]);
            }
            
            if (old)
            {
                values = list.Skip(total - size).ToArray();
            }
            else
            {
                values = list.Take(size).ToArray();
            }
        }

        void PrintValues()
        {
            foreach (var d in values)
            {
                Console.Write(d + " ");
            }

            Console.WriteLine();
        }

        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            values = info.Parent.ReadObject<double[]>();
            size = values.Length;
            Sort();

            for (int i = 0; i < 100; i++)
            {
                info.Parent.WriteObject(values);
                double[] friend = info.Parent.ReadObject<double[]>();
                bool old = info.Parent.ReadBoolean();
                MergeAndSave(friend, old);
                Sort();
            }
        }
    }
}
