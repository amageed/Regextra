using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests.PassphraseRegexTests
{
    [TestFixture]
    public class PassphraseRegexGeneralTests
    {
        [Test]
        public void Invalid_Result_When_Rules_Are_Empty()
        {
            var builder = PassphraseRegex.With.MaxLength(1);

            var result = builder.ToRegex();

            result.Error.ShouldBe("No rules were specified");
            result.IsValid.ShouldBe(false);
            result.Regex.ShouldBe(null);
            result.Pattern.ShouldBe(null);
        }

        [Test]
        public void Verify_Pattern_For_MinLength_Of_1()
        {
            var builder = PassphraseRegex.With.MinLength(1)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[abc])(?!^\s|.*\s$).+$");
        }

        [Test]
        public void Verify_Pattern_For_MinLength_Of_2()
        {
            var builder = PassphraseRegex.With.MinLength(2)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[abc])(?!^\s|.*\s$).{2,}$");
        }

        [Test]
        public void Verify_Pattern_For_MaxLength_Of_1()
        {
            var builder = PassphraseRegex.With.MaxLength(1)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[abc])(?!^\s|.*\s$).$");
        }

        [Test]
        public void Verify_Pattern_For_MaxLength_Of_2()
        {
            var builder = PassphraseRegex.With.MaxLength(2)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[abc])(?!^\s|.*\s$).{1,2}$");
        }

        [Test]
        public void Verify_Pattern_For_MinLength_Of_2_And_MaxLength_Of_5()
        {
            var builder = PassphraseRegex.With.MinLength(2)
                                              .MaxLength(5)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[abc])(?!^\s|.*\s$).{2,5}$");
        }

        [Test]
        public void Minimum_Length_Is_One_When_One_Rule_Exists()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("a");

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[a])(?!^\s|.*\s$).+$");
        }

        [Test]
        public void Minimum_Length_Is_Two_When_Two_Rules_Exist()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("a")
                                              .IncludesRange(1, 9);

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[a])(?=.*[1-9])(?!^\s|.*\s$).{2,}$");
        }

        [Test]
        public void Throws_ArgumentException_When_Assigned_Minimum_Length_Is_Less_Than_Specified_Rules_Count()
        {
            var builder = PassphraseRegex.With.MinLength(1)
                                              .IncludesAnyCharacters("a")
                                              .IncludesRange(1, 9);

            var exception = Should.Throw<ArgumentException>(() => builder.ToRegex());
            exception.Message.ShouldStartWith("Minimum length");
        }

        [Test]
        public void Throws_ArgumentException_When_Assigned_Maximum_Length_Is_Less_Than_Specified_Rules_Count()
        {
            var builder = PassphraseRegex.With.MaxLength(1)
                                              .IncludesAnyCharacters("a")
                                              .IncludesRange('1', '9');

            var exception = Should.Throw<ArgumentException>(() => builder.ToRegex());
            exception.Message.ShouldStartWith("Maximum length");
        }

        [TestCase(true, "abc")]
        [TestCase(false, "a")]
        public void Input_Must_Meet_MinLength_Rule(bool isValid, string input)
        {
            var builder = PassphraseRegex.With.MinLength(2)
                                              .IncludesRange('a', 'z');

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [Test]
        public void Throws_ArgumentOutOfRangeException_With_Minimum_Occurrence_Less_Than_One()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("abc");

            var exception = Should.Throw<ArgumentOutOfRangeException>(() => builder.WithMinimumOccurrenceOf(0));
            exception.ParamName.ShouldBe("length");
        }

        [Test]
        public void Throws_ArgumentOutOfRangeException_With_MaximumConsecutiveIdenticalCharacter_Less_Than_2()
        {
            var exception = Should.Throw<ArgumentOutOfRangeException>(() => PassphraseRegex.With.MaxConsecutiveIdenticalCharacterOf(1));
            exception.ParamName.ShouldBe("length");
        }

        [Test]
        public void Does_Not_Throw_Exception_With_MaximumConsecutiveIdenticalCharacter_Of_2()
        {
            Should.NotThrow(() => PassphraseRegex.With.MaxConsecutiveIdenticalCharacterOf(2));
        }

        [TestCase(true, "abc")]
        [TestCase(true, "aabc")]
        [TestCase(true, "aaabc")]
        [TestCase(true, "abbbcb")]
        [TestCase(false, "aaaabc")]
        [TestCase(false, "abbbbc")]
        public void Input_Can_Contain_3_Identical_Consecutive_Characters(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesRange('a', 'z')
                                              .MaxConsecutiveIdenticalCharacterOf(3);

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "ab c")]
        [TestCase(false, " abc")]
        [TestCase(false, "abc ")]
        [TestCase(false, " abc ")]
        public void Leading_And_Trailing_Space_Is_Rejected(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesRange('a', 'z');

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [Test]
        public void RegexOptions_Is_None_By_Default()
        {
            var builder = PassphraseRegex.That.IncludesText("abc");

            var result = builder.ToRegex();

            result.Regex.Options.ShouldBe(RegexOptions.None);
        }

        [TestCase(RegexOptions.Compiled)]
        [TestCase(RegexOptions.IgnoreCase)]
        [TestCase(RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        public void Can_Specify_RegexOptions(RegexOptions options)
        {
            var builder = PassphraseRegex.That.IncludesText("abc")
                                              .Options(options);

            var result = builder.ToRegex();

            result.Regex.Options.ShouldBe(options);
        }
    }
}
