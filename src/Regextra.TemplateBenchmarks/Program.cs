using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Regextra;
using Regextra.TemplateBenchmarks.Formatters;
using Regextra.TemplateBenchmarks.Models;
using SmartFormat;

namespace Regextra.TemplateBenchmarks
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
            RunBenchmarksForDictionaryInput();
            RunBenchmarksForExpandoObject();
            RunBenchmarksForDynamicPerson();
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

        private static void RunBenchmarksForDictionaryInput()
        {
            var template = "The access code is {Code} and the token is {Token:N}.";
            var item = new Dictionary<string, Guid>()
            {
                { "Code", Guid.NewGuid() },
                { "Token", Guid.NewGuid() }
            };

            var benchmarks = new List<Benchmark>
            {
                new Benchmark("Regextra Template", () => Template.Format(template, item)),
                new Benchmark("SmartFormat.NET", () => template.FormatSmart(item))
            };

            var group = new BenchmarkGroup("Dictionary Input Template", ITERATION_COUNT, benchmarks);
            group.Run();
        }

        public static void RunBenchmarksForExpandoObject()
        {
            dynamic item = new ExpandoObject();
            item.Id = Person.Id;
            item.Name = Person.Name;
            item.DateOfBirth = Person.DateOfBirth;

            var benchmarks = new List<Benchmark>
            {
                new Benchmark("Regextra Template", () => Template.Format(sampleTemplate, item)),
                new Benchmark("SmartFormat.NET", () => Smart.Format(sampleTemplate, item))
            };

            var group = new BenchmarkGroup("ExpandoObject Template", ITERATION_COUNT, benchmarks);
            group.Run();
        }

        private static void RunBenchmarksForDynamicPerson()
        {
            dynamic item = Person;

            var benchmarks = new List<Benchmark>
            {
                new Benchmark("Regextra Template w/FastMember", () => Template.Format(sampleNestedTemplate, item)),
                new Benchmark("SmartFormat.NET", () => Smart.Format(sampleNestedTemplate, item))
            };

            var group = new BenchmarkGroup("Dynamic Person Template", ITERATION_COUNT, benchmarks);
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
