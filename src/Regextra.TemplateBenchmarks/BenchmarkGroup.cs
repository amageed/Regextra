using System;
using System.Collections.Generic;
using System.Linq;

namespace Regextra.TemplateBenchmarks
{
    public class BenchmarkGroup
    {
        private readonly int IterationCount;
        public List<Benchmark> Benchmarks { get; set; }
        public string Title { get; set; }
        public BenchmarkGroup(string title, int iterationCount, List<Benchmark> benchmarks)
        {
            Title = title;
            IterationCount = iterationCount;
            Benchmarks = benchmarks;
        }

        public void Run()
        {
            Console.WriteLine("Benchmarking group: {0}", Title);
            Warmup();
            RunBenchmarks();
            DisplayResults();
        }

        private void Warmup()
        {
            foreach (var benchmark in Benchmarks)
            {
                benchmark.Iteration();
            }
        }

        private void RunBenchmarks()
        {
            var rand = new Random();
            for (int i = 1; i <= IterationCount; i++)
            {
                foreach (var benchmark in Benchmarks.OrderBy(x => rand.Next()))
                {
                    benchmark.Stopwatch.Start();
                    benchmark.Iteration();
                    benchmark.Stopwatch.Stop();
                }
            }
        }

        private void DisplayResults()
        {
            foreach (var benchmark in Benchmarks.OrderBy(b => b.Stopwatch.ElapsedMilliseconds))
            {
                Console.WriteLine("{0}: {1} ms -- {2} ms/iteration", benchmark.Name, benchmark.Stopwatch.ElapsedMilliseconds, ((double)benchmark.Stopwatch.ElapsedMilliseconds / (double)IterationCount));
            }
            Console.WriteLine();
        }
    }
}
