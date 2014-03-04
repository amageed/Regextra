using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests.PassphraseRegexTests
{
    [TestFixture]
    public class PassphraseRegexCharactersTests
    {
        [Test]
        public void Can_Specify_Contained_Characters()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[abc])(?!^\s|.*\s$).+$");
        }

        [TestCase(null)]
        [TestCase("")]
        public void Throws_ArgumentException_With_Null_Or_Empty_Contained_Characters(string input)
        {
            var exception = Should.Throw<ArgumentException>(() => PassphraseRegex.That.IncludesAnyCharacters(input));
            exception.ParamName.ShouldBe("characters");
        }

        [Test]
        public void Can_Specify_Excluded_Characters()
        {
            var builder = PassphraseRegex.That.ExcludesCharacters("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?!.*[abc])(?!^\s|.*\s$).+$");
        }

        [TestCase(null)]
        [TestCase("")]
        public void Throws_ArgumentException_With_Null_Or_Empty_Excluded_Characters(string input)
        {
            var exception = Should.Throw<ArgumentException>(() => PassphraseRegex.That.ExcludesCharacters(input));
            exception.ParamName.ShouldBe("characters");
        }

        [TestCase("a-z")]
        [TestCase("az-")]
        [TestCase("-az")]
        [TestCase("a--z")]
        [TestCase(@"a\-z")]
        public void Dash_In_Contained_Characters_Placed_At_End_To_Avoid_Unintended_Range(string characters)
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters(characters);

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[az-])(?!^\s|.*\s$).+$");
        }

        [TestCase("a-z")]
        [TestCase("az-")]
        [TestCase("-az")]
        [TestCase("a--z")]
        [TestCase(@"a\-z")]
        public void Dash_In_Excluded_Characters_Placed_At_End_To_Avoid_Unintended_Range(string characters)
        {
            var builder = PassphraseRegex.That.ExcludesCharacters(characters);

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?!.*[az-])(?!^\s|.*\s$).+$");
        }

        [TestCase("a^z")]
        [TestCase("az^")]
        [TestCase("^az")]
        [TestCase("a^z")]
        [TestCase(@"a\^z")]
        public void Caret_In_Contained_Characters_Placed_At_End_To_Avoid_Unintended_Negation(string characters)
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters(characters);

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[az^])(?!^\s|.*\s$).+$");
        }

        [TestCase("a^z")]
        [TestCase("az^")]
        [TestCase("^az")]
        [TestCase("a^z")]
        [TestCase(@"a\^z")]
        public void Caret_In_Excluded_Characters_Placed_At_End_To_Avoid_Unintended_Negation(string characters)
        {
            var builder = PassphraseRegex.That.ExcludesCharacters(characters);

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?!.*[az^])(?!^\s|.*\s$).+$");
        }

        [TestCase("[")]
        [TestCase("]")]
        [TestCase("-")]
        [TestCase("^")]
        [TestCase("\\")]
        public void Included_Characters_Can_Handle_Character_Class_Considerations(string input)
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("[-]^\\");

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            result.Pattern.ShouldBe(@"^(?=.*[[\]\\^-])(?!^\s|.*\s$).+$");
            Regex.IsMatch(input, result.Pattern).ShouldBe(true);
        }

        [TestCase("[")]
        [TestCase("]")]
        [TestCase("-")]
        [TestCase("^")]
        [TestCase("\\")]
        public void Excluded_Characters_Can_Handle_Character_Class_Considerations(string input)
        {
            var builder = PassphraseRegex.That.ExcludesCharacters("[-]^\\");

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            result.Pattern.ShouldBe(@"^(?!.*[[\]\\^-])(?!^\s|.*\s$).+$");
            Regex.IsMatch(input, result.Pattern).ShouldBe(false);
        }

        [Test]
        public void Included_Characters_Will_Ignore_Character_Class_Range()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("[a-z]");

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            result.Pattern.ShouldBe(@"^(?=.*[[az\]-])(?!^\s|.*\s$).+$");
            Regex.IsMatch("b", result.Pattern).ShouldBe(false);
        }

        [TestCase(true, "123")]
        [TestCase(true, "ABC")]
        [TestCase(true, "xYz")]
        [TestCase(false, "a1b2c3")]
        [TestCase(false, "abc")]
        public void Input_Must_Not_Contain_LowerCase_Letters_ABC(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.ExcludesCharacters("abc");

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "abc12")]
        [TestCase(true, "abc123")]
        [TestCase(true, "a1b2c3")]
        [TestCase(false, "abc1")]
        [TestCase(false, "abc")]
        public void Input_Contains_At_Least_Two_Digits_In_Specified_Characters(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("0123456789")
                                              .WithMinimumOccurrenceOf(2);

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase("abcdefg")]
        [TestCase("AbCdEfG")]
        public void Included_Characters_With_RegexOptions_IgnoreCase_Accepts_Any_Casing(string input)
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("cDe")
                                              .Options(RegexOptions.IgnoreCase);

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(true);
            result.Regex.Options.ShouldBe(RegexOptions.IgnoreCase);
        }

        [TestCase("abcdefg")]
        [TestCase("AbCdEfG")]
        public void Excluded_Characters_With_RegexOptions_IgnoreCase_Rejects_Any_Casing(string input)
        {
            var builder = PassphraseRegex.That.ExcludesCharacters("cDe")
                                              .Options(RegexOptions.IgnoreCase);

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(false);
            result.Regex.Options.ShouldBe(RegexOptions.IgnoreCase);
        }
    }
}
