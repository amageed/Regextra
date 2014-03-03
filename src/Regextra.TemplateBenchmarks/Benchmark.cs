using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Regextra.TemplatePerformanceBenchmarks
{
    public class Benchmark
    {
        public Stopwatch Stopwatch { get; set; }
        public string Name { get; set; }
        public Action Iteration { get; set; }
        public Benchmark(string name, Action iteration)
        {
            Iteration = iteration;
            Name = name;
            Stopwatch = new Stopwatch();
        }
    }
}