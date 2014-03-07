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
    }
}
