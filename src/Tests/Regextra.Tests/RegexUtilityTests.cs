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
        public void SplitKeepDelimiters_Throws_ArgumentException_For_Null_Delimiters()
        {
            string input = "123xx456yy789";
            string[] delimiters = null;

            var ex = Should.Throw<ArgumentException>(() => RegexUtility.SplitKeepDelimiters(input, delimiters));

            ex.ParamName.ShouldBe("delimiters");
        }

        [Test]
        public void SplitKeepDelimiters_Throws_ArgumentException_For_Empty_Delimiters()
        {
            string input = "123xx456yy789";
            string[] delimiters = {};

            var ex = Should.Throw<ArgumentException>(() => RegexUtility.SplitKeepDelimiters(input, delimiters));

            ex.ParamName.ShouldBe("delimiters");
        }

        [Test]
        public void SplitKeepDelimiters_Returns_Items_And_Delimiters()
        {
            string input = "123xx456yy789";
            string[] delimiters = { "xx", "yy" };
            string[] expected = { "123", "xx", "456", "yy", "789" };
            
            var result = RegexUtility.SplitKeepDelimiters(input, delimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Is_Case_Sensitive()
        {
            string input = "123XYZ456";
            string[] delimiters = { "xyz" };
            string[] expected = { input };
            
            var result = RegexUtility.SplitKeepDelimiters(input, delimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Case_Can_Be_Ignored()
        {
            string input = "123XYZ456";
            string[] delimiters = { "xyz" };
            string[] expected = { "123", "XYZ", "456" };

            var result = RegexUtility.SplitKeepDelimiters(input, delimiters, RegexOptions.IgnoreCase);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Escapes_Metacharacters()
        {
            string input = "() Hello . World?";
            string[] delimiters = { "()", ".", "?" };
            string[] expected = { "", "()", " Hello ", ".", " World", "?", "" };

            var result = RegexUtility.SplitKeepDelimiters(input, delimiters);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Can_Remove_Empty_Entries()
        {
            string input = "() Hello . World?";
            string[] delimiters = { "()", ".", "?" };
            string[] expected = { "()", " Hello ", ".", " World", "?" };

            var result = RegexUtility.SplitKeepDelimiters(input, delimiters, splitOptions: RegextraSplitOptions.RemoveEmptyEntries);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Can_TrimWhitespace()
        {
            string input = "Hello . World";
            string[] delimiters = { "." };
            string[] expected = { "Hello", ".", "World" };

            var result = RegexUtility.SplitKeepDelimiters(input, delimiters, splitOptions: RegextraSplitOptions.TrimWhitespace);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Splits_Partial_Matches()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "Stack", "Overflow ", "Stack", " Over", "Stack" };

            var result = RegexUtility.SplitKeepDelimiters(input, delimiters, splitOptions: RegextraSplitOptions.RemoveEmptyEntries);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Can_Split_Whole_Words()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            string[] expected = { "StackOverflow ", "Stack", " OverStack" };

            var result = RegexUtility.SplitKeepDelimiters(input, delimiters, splitOptions: RegextraSplitOptions.MatchWholeWords);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Can_Combine_SplitOptions_For_MatchWholeWords_And_TrimWhitespace()
        {
            string input = "StackOverflow Stack OverStack";
            string[] delimiters = { "Stack" };
            var splitOptions = RegextraSplitOptions.MatchWholeWords | RegextraSplitOptions.TrimWhitespace;
            string[] expected = { "StackOverflow", "Stack", "OverStack" };
            
            var result = RegexUtility.SplitKeepDelimiters(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }

        [Test]
        public void SplitKeepDelimiters_Can_Combine_All_SplitOptions()
        {
            string input = "Stack StackOverflow Stack OverStack Stack";
            string[] delimiters = { "Stack" };
            var splitOptions = RegextraSplitOptions.MatchWholeWords | RegextraSplitOptions.TrimWhitespace | RegextraSplitOptions.RemoveEmptyEntries;
            string[] expected = { "Stack", "StackOverflow", "Stack", "OverStack", "Stack" };

            var result = RegexUtility.SplitKeepDelimiters(input, delimiters, splitOptions: splitOptions);

            result.ShouldBe(expected);
        }
    }
}
