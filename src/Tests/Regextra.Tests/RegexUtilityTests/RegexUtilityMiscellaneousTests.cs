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
    }
}
