using AoC24.Days;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Benchmark
{
    [MemoryDiagnoser]
    public class Benchmarker
    {
        string inputCh1;
        string inputCh2;

        Day4 Day4_Ch1;
        Day4 Day4_Ch2;

        [GlobalSetup]
        public void Setup()
        {
            inputCh1 = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "input.txt"));
            inputCh2 = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "input.txt"));

            Day4_Ch1 = new Day4();
            Day4_Ch2 = new Day4();
        }

        [Benchmark]
        public void Ch1()
        {
            Day4_Ch1.Ch1(inputCh1);
        }

        [Benchmark]
        public void Ch2()
        {
            Day4_Ch2.Ch2(inputCh2);
        }
    }
}
