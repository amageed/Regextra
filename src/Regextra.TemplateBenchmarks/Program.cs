using System;
using System.Collections.Generic;
using System.Linq;
using Regextra;
using Regextra.TemplatePerformanceBenchmarks.Formatters;
using Regextra.TemplatePerformanceBenchmarks.Models;
using SmartFormat;

namespace Regextra.TemplatePerformanceBenchmarks
{
    class Program
    {
        private static Person Person { get; set; }
        private const string sampleTemplate = @"Hi {Name}! Your date of birth is {DateOfBirth:d}, your ID is {Id}.";
        private const string sampleNestedTemplate = @"Hi {Name}! Your date of birth is {DateOfBirth:d}, your zipcode is {Address.ZipCode.Item1:N}, and the address modified date is {Address.DateModified}.";
        private const int ITERATION_COUNT = 1000;

        static void Main(string[] args)
        {
            Person = GetPerson();
            RunBenchmarksForSampleTemplate();
            RunBenchmarksForNestedSampleTemplate();
            Console.ReadLine();
        }

        private static void RunBenchmarksForSampleTemplate()
        {
            var benchmarks = new List<Benchmark>
            {
                new Benchmark("Regextra Template w/FastMember", () => Template.Format(sampleTemplate, Person)),
                new Benchmark("Regextra Template w/Reflection", () => TemplateReflection.Format(sampleTemplate, Person)),
                new Benchmark("SmartFormat.NET", () => sampleTemplate.FormatSmart(Person)),
                new Benchmark("SmartFormat.NET (Cached)", () =>
                {
                    SmartFormat.Core.FormatCache cache = null;
                    sampleTemplate.FormatSmart(ref cache, Person);
                })
            };

            var group = new BenchmarkGroup("Sample Template", ITERATION_COUNT, benchmarks);
            group.Run();
        }

        private static void RunBenchmarksForNestedSampleTemplate()
        {
            var benchmarks = new List<Benchmark>
            {
                new Benchmark("Regextra Template w/FastMember", () => Template.Format(sampleNestedTemplate, Person)),
                new Benchmark("Regextra Template w/Reflection", () => TemplateReflection.Format(sampleNestedTemplate, Person)),
                new Benchmark("SmartFormat.NET", () => sampleNestedTemplate.FormatSmart(Person)),
                new Benchmark("SmartFormat.NET (Cached)", () =>
                {
                    SmartFormat.Core.FormatCache cache = null;
                    sampleNestedTemplate.FormatSmart(ref cache, Person);
                })
            };

            var group = new BenchmarkGroup("Nested Sample Template", ITERATION_COUNT, benchmarks);
            group.Run();
        }

        private static Person GetPerson()
        {
            var person = new Person
            {
                Id = 42,
                Name = "Ahmad",
                DateOfBirth = DateTime.UtcNow,
                Address = new Address
                {
                    ZipCode = Tuple.Create(Guid.NewGuid(), Guid.NewGuid().ToString()),
                    DateModified = DateTime.Now
                }
            };
            return person;
        }
    }
}
