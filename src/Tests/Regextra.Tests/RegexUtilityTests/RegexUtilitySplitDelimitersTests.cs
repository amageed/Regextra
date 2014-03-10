using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests
{
    [TestFixture]
    public class RegexUtilitySplitDelimitersTests
    {
        [Test]
        public void Split_Throws_ArgumentException_For_Null_Delimiters()
        {
            string input = "123xx456yy789";
            string[] delimiters = null;

            var ex = Should.Throw<ArgumentException>(() => RegexUtility.Split(input, delimiters));

            ex.ParamName.ShouldBe("delimiters");
        }

        [Test]
        public void Split_Throws_ArgumentException_For_Empty_Delimiters()
        {
            string input = "123xx456yy789";
            string[] delimiters = { };

            var ex = Should.Throw<ArgumentException>(() => RegexUtility.Split(input, delimiters));

            ex.ParamName.ShouldBe("delimiters");
        }

        [Test]
        public void Split_Can_Include_Delimiters()
        {
            string input = "123xx456yy789";
            string[] delimiters = { "xx", "yy" };
            string[] expected = { "123", "xx", "456", "yy", "789" };

            var result = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Is_Case_Sensitive()
        {
            string input = "123XYZ456";
            string[] delimiters = { "xyz" };
            string[] expected = { input };

            var result = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Case_Can_Be_Ignored()
        {
            string input = "123XYZ456";
            string[] delimiters = { "xyz" };
            string[] expected = { "123", "XYZ", "456" };

            var result = RegexUtility.Split(input, delimiters, RegexOptions.IgnoreCase, SplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Escapes_Metacharacters()
        {
            string input = "() Hello . World?";
            string[] delimiters = { "()", ".", "?" };
            string[] expected = { "", "()", " Hello ", ".", " World", "?", "" };

            var result = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Remove_Empty_Entries()
        {
            string input = "() Hello . World?";
            string[] delimiters = { "()", ".", "?" };
            string[] expected = { "()", " Hello ", ".", " World", "?" };
            var splitOptions = SplitOptions.IncludeDelimiters | SplitOptions.RemoveEmptyEntries;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Trim_Whitespace()
        {
            string input = "Hello . World";
            string[] delimiters = { "." };
            string[] expected = { "Hello", ".", "World" };
            var splitOptions = SplitOptions.IncludeDelimiters | SplitOptions.TrimWhitespace;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Splits_Partial_Matches()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "Stack", "Overflow ", "Stack", " Over", "Stack" };
            var splitOptions = SplitOptions.IncludeDelimiters | SplitOptions.RemoveEmptyEntries;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Match_Whole_Words()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "StackOverflow ", " OverStack" };

            var result = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.MatchWholeWords);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Combine_SplitOptions_To_IncludeDelimiters_MatchWholeWords_And_TrimWhitespace()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "StackOverflow", "Stack", "OverStack" };
            var splitOptions = SplitOptions.IncludeDelimiters | SplitOptions.MatchWholeWords | SplitOptions.TrimWhitespace;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Combine_All_SplitOptions()
        {
            string input = "Stack StackOverflow Stack OverStack Stack";
            string[] delimiters = { "Stack" };
            string[] expected = { "Stack", "StackOverflow", "Stack", "OverStack", "Stack" };

            var result = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.All);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitIncludeDelimiters_Matches_Core_Split_With_Same_Option()
        {
            string input = "123xx456yy789";
            string[] delimiters = { "xx", "yy" };
            string[] expected = { "123", "xx", "456", "yy", "789" };

            var coreSplit = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.IncludeDelimiters);
            var result = RegexUtility.SplitIncludeDelimiters(input, delimiters);

            result.ShouldBe(coreSplit);
            result.ShouldBe(expected);
        }

        [Test]
        public void SplitMatchWholeWords_Matches_Core_Split_With_Same_Option()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "StackOverflow ", " OverStack" };

            var coreSplit = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.MatchWholeWords);
            var result = RegexUtility.SplitMatchWholeWords(input, delimiters);

            result.ShouldBe(coreSplit);
            result.ShouldBe(expected);
        }

        [Test]
        public void SplitTrimWhitespace_Matches_Core_Split_With_Same_Option()
        {
            string input = "Hello . World";
            string[] delimiters = { "." };
            string[] expected = { "Hello", "World" };

            var coreSplit = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.TrimWhitespace);
            var result = RegexUtility.SplitTrimWhitespace(input, delimiters);

            result.ShouldBe(coreSplit);
            result.ShouldBe(expected);
        }

        [Test]
        public void SplitRemoveEmptyEntries_Matches_Core_Split_With_Same_Option()
        {
            string input = "() Hello . World?";
            string[] delimiters = { "()", ".", "?" };
            string[] expected = { " Hello ", " World" };

            var coreSplit = RegexUtility.Split(input, delimiters, splitOptions: SplitOptions.RemoveEmptyEntries);
            var result = RegexUtility.SplitRemoveEmptyEntries(input, delimiters);

            result.ShouldBe(coreSplit);
            result.ShouldBe(expected);
        }
    }
}
