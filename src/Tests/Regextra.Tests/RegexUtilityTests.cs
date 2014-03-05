using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests
{
    [TestFixture]
    public class RegexUtilityTests
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
            string[] delimiters = {};

            var ex = Should.Throw<ArgumentException>(() => RegexUtility.Split(input, delimiters));

            ex.ParamName.ShouldBe("delimiters");
        }

        [Test]
        public void Split_Can_Return_Items_And_Delimiters()
        {
            string input = "123xx456yy789";
            string[] delimiters = { "xx", "yy" };
            string[] expected = { "123", "xx", "456", "yy", "789" };
            
            var result = RegexUtility.Split(input, delimiters, splitOptions: RegextraSplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Is_Case_Sensitive()
        {
            string input = "123XYZ456";
            string[] delimiters = { "xyz" };
            string[] expected = { input };

            var result = RegexUtility.Split(input, delimiters, splitOptions: RegextraSplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Case_Can_Be_Ignored()
        {
            string input = "123XYZ456";
            string[] delimiters = { "xyz" };
            string[] expected = { "123", "XYZ", "456" };

            var result = RegexUtility.Split(input, delimiters, RegexOptions.IgnoreCase, RegextraSplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Escapes_Metacharacters()
        {
            string input = "() Hello . World?";
            string[] delimiters = { "()", ".", "?" };
            string[] expected = { "", "()", " Hello ", ".", " World", "?", "" };

            var result = RegexUtility.Split(input, delimiters, splitOptions: RegextraSplitOptions.IncludeDelimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Remove_Empty_Entries()
        {
            string input = "() Hello . World?";
            string[] delimiters = { "()", ".", "?" };
            string[] expected = { "()", " Hello ", ".", " World", "?" };
            var splitOptions = RegextraSplitOptions.IncludeDelimiters | RegextraSplitOptions.RemoveEmptyEntries;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_TrimWhitespace()
        {
            string input = "Hello . World";
            string[] delimiters = { "." };
            string[] expected = { "Hello", ".", "World" };
            var splitOptions = RegextraSplitOptions.IncludeDelimiters | RegextraSplitOptions.TrimWhitespace;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Splits_Partial_Matches()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "Stack", "Overflow ", "Stack", " Over", "Stack" };
            var splitOptions = RegextraSplitOptions.IncludeDelimiters | RegextraSplitOptions.RemoveEmptyEntries;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Match_Whole_Words()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "StackOverflow ", "Stack", " OverStack" };
            var splitOptions = RegextraSplitOptions.IncludeDelimiters | RegextraSplitOptions.MatchWholeWords;

            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Combine_SplitOptions_To_IncludeDelimiters_MatchWholeWords_And_TrimWhitespace()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "StackOverflow", "Stack", "OverStack" };
            var splitOptions = RegextraSplitOptions.IncludeDelimiters | RegextraSplitOptions.MatchWholeWords | RegextraSplitOptions.TrimWhitespace;
            
            var result = RegexUtility.Split(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void Split_Can_Combine_All_SplitOptions()
        {
            string input = "Stack StackOverflow Stack OverStack Stack";
            string[] delimiters = { "Stack" };
            string[] expected = { "Stack", "StackOverflow", "Stack", "OverStack", "Stack" };

            var result = RegexUtility.Split(input, delimiters, splitOptions: RegextraSplitOptions.All);

            result.ShouldBe(expected);
        }
    }
}
