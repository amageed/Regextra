using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests.PassphraseRegexTests
{
    [TestFixture]
    public class TextTests
    {
        [Test]
        public void Can_Specify_Includes_Text_Rule()
        {
            var builder = PassphraseRegex.That.IncludesText("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*abc)(?!^\s|.*\s$).+$");
        }

        [Test]
        public void Can_Specify_Excludes_Text_Rule()
        {
            var builder = PassphraseRegex.Which.ExcludesText("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?!.*abc)(?!^\s|.*\s$).+$");
        }

        [TestCase(true, "abc")]
        [TestCase(true, "xabcx")]
        [TestCase(true, "abcx")]
        [TestCase(true, "xabc")]
        [TestCase(false, "a_b_c")]
        public void Input_Must_Contain_ABC(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesText("abc");

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(isValid);
        }

        [TestCase(true, "a_b_c")]
        [TestCase(false, "abc")]
        [TestCase(false, "xabcx")]
        [TestCase(false, "abcx")]
        [TestCase(false, "xabc")]
        public void Input_Must_Not_Contain_ABC(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.ExcludesText("abc");

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(isValid);
        }

        [TestCase(".", ".", "a")]
        [TestCase("[a-z]", "x[a-z]x", "b")]
        [TestCase("^a", "x^ax", "abc")]
        [TestCase("d$", "food$bar", "food")]
        public void Metacharacters_Are_Escaped_For_Includes_Text_Rule(string input, string passingText, string failingText)
        {
            var builder = PassphraseRegex.That.IncludesText(input);

            var result = builder.ToRegex();

            result.Pattern.ShouldContain(Regex.Escape(input));
            result.Regex.IsMatch(passingText).ShouldBe(true);
            result.Regex.IsMatch(failingText).ShouldBe(false);
        }

        [TestCase(".", "a", "x.x")]
        [TestCase("[a-z]", "[a-z", "x[a-z]x")]
        [TestCase("^a", "abc", "x^ax")]
        [TestCase("d$", "food_$", "food$bar")]
        public void Metacharacters_Are_Escaped_For_Excludes_Text_Rule(string input, string passingText, string failingText)
        {
            var builder = PassphraseRegex.That.ExcludesText(input);

            var result = builder.ToRegex();

            result.Pattern.ShouldContain(Regex.Escape(input));
            result.Regex.IsMatch(passingText).ShouldBe(true);
            result.Regex.IsMatch(failingText).ShouldBe(false);
        }

        [TestCase("abc")]
        [TestCase("AbC")]
        public void Included_Text_With_RegexOptions_IgnoreCase_Accepts_Any_Casing(string input)
        {
            var builder = PassphraseRegex.That.IncludesText("abc")
                                              .Options(RegexOptions.IgnoreCase);

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(true);
            result.Regex.Options.ShouldBe(RegexOptions.IgnoreCase);
        }

        [TestCase("abc")]
        [TestCase("AbC")]
        public void Excluded_Text_With_RegexOptions_IgnoreCase_Rejects_Any_Casing(string input)
        {
            var builder = PassphraseRegex.That.ExcludesText("abc")
                                              .Options(RegexOptions.IgnoreCase);

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(false);
            result.Regex.Options.ShouldBe(RegexOptions.IgnoreCase);
        }
    }
}
