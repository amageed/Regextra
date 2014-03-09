using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests.RegexUtilityTests
{
    [TestFixture]
    public class RegexUtilityMiscellaneousTests
    {
        [Test]
        public void Can_Split_And_Remove_Empty_Entries()
        {
            var input = "x hello x world x goodbye !x world!";
            var pattern = @"\s*[x!]\s*";
            string[] expected = { "hello", "world", "goodbye", "world" };

            var result = RegexUtility.SplitRemoveEmptyEntries(input, pattern);

            result.ShouldBe(expected);
        }

        [Test]
        public void Can_Split_And_Remove_Empty_Entries_And_Use_RegexOptions_To_Ignore_Case()
        {
            var input = "x hello X world x goodbye !X world!";
            var pattern = @"\s*[X!]\s*";
            string[] expected = { "hello", "world", "goodbye", "world" };

            var result = RegexUtility.SplitRemoveEmptyEntries(input, pattern, RegexOptions.IgnoreCase);

            result.ShouldBe(expected);
        }

        [Test]
        public void Trim_Whitespaces_Throws_Exception_For_Null_Input_From_Regex_Class()
        {
            var ex = Should.Throw<ArgumentNullException>(() => RegexUtility.TrimWhitespaces(null));

            ex.ParamName.ShouldBe("input");
        }

        [TestCase("Hello World   ")]
        [TestCase("  Hello World")]
        [TestCase("Hello    World")]
        [TestCase("   Hello    World   ")]
        public void Can_Trim_Whitespaces_In_Given_Inputs(string input)
        {
            var expected = "Hello World";

            var result = RegexUtility.TrimWhitespaces(input);

            result.ShouldBe(expected);
        }

        [Test]
        public void MatchesToNamedGroupsDictionaries_Throws_FormatException_When_Named_Groups_Are_Absent()
        {
            var input = "Hello";
            var pattern = ".";

            var ex = Should.Throw<FormatException>(() => RegexUtility.MatchesToNamedGroupsDictionaries(input, pattern));
        }

        [Test]
        public void MatchesToNamedGroupsDictionaries_Returns_Expected_Result()
        {
            var input = "123-456-7890 hello 098-765-4321";
            var pattern = @"(?<AreaCode>\d{3})-(?<First>\d{3})-(?<Last>\d{4})";
            string[] groupNames = new[] { "AreaCode", "First", "Last" };

            Dictionary<string, string>[] results = RegexUtility.MatchesToNamedGroupsDictionaries(input, pattern);

            results.Length.ShouldBe(2);
            results[0].Keys.Count.ShouldBe(groupNames.Length);
            results[1][groupNames[0]].ShouldBe("098");
            results[1][groupNames[1]].ShouldBe("765");
            results[1][groupNames[2]].ShouldBe("4321");
        }

        [Test]
        public void MatchesToNamedGroupsDictionaries_Accepts_RegexOptions_To_Ignore_Case()
        {
            var input = "Hello, World!";
            var pattern = @"(?<Greeting>heLlO), (?<Subject>\w+)!";

            Dictionary<string, string> result = RegexUtility.MatchesToNamedGroupsDictionaries(input, pattern, RegexOptions.IgnoreCase).Single();

            result["Greeting"].ShouldBe("Hello");
            result["Subject"].ShouldBe("World");
        }

        [Test]
        public void MatchesToNamedGroupsLookup_Throws_FormatException_When_Named_Groups_Are_Absent()
        {
            var input = "Hello";
            var pattern = ".";

            var ex = Should.Throw<FormatException>(() => RegexUtility.MatchesToNamedGroupsLookup(input, pattern));
        }

        [Test]
        public void MatchesToNamedGroupsLookup_Returns_Expected_Result()
        {
            var input = "123-456-7890 hello 098-765-4321";
            var pattern = @"(?<AreaCode>\d{3})-(?<First>\d{3})-(?<Last>\d{4})";
            string[] groupNames = new[] { "AreaCode", "First", "Last" };

            ILookup<string, string> result = RegexUtility.MatchesToNamedGroupsLookup(input, pattern);

            result.Count.ShouldBe(groupNames.Length);
            result[groupNames[0]].Count().ShouldBe(2);
            result[groupNames[1]].ShouldContain("456");
            result[groupNames[2]].ShouldContain("4321");
        }

        [Test]
        public void MatchesToNamedGroupsLookup_Accepts_RegexOptions_To_Ignore_Case()
        {
            var input = "Hello, World! hi, Universe!";
            var pattern = @"(?<Greeting>h\w+), (?<Subject>\w+)!";

            ILookup<string, string> result = RegexUtility.MatchesToNamedGroupsLookup(input, pattern, RegexOptions.IgnoreCase);

            result["Greeting"].ShouldContain("Hello");
            result["Greeting"].ShouldContain("hi");
        }
    }
}
