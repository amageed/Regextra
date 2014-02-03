using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Regextra;
using Shouldly;

namespace Tests
{
    [TestFixture]
    public class PassphraseRegexRangeTests
    {
        [Test]
        public void Can_Specify_Range_Rule()
        {
            var builder = PassphraseRegex.That.IncludesRange('a', 'z');

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[a-z])(?!^\s|.*\s$).+$");
        }

        [Test]
        public void Can_Specify_Range_Rule_Using_Numbers()
        {
            var builder = PassphraseRegex.That.IncludesRange(0, 9);

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?=.*[0-9])(?!^\s|.*\s$).+$");
        }

        [Test]
        public void Invalid_Result_When_Specifying_A_Reversed_Range()
        {
            var builder = PassphraseRegex.That.IncludesRange('z', 'a');

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(false);
            result.Regex.ShouldBe(null);
            result.Error.ShouldContain("range in reverse order");
            result.Pattern.ShouldNotBe(null);
        }

        [Test]
        public void Can_Specify_Excluded_Range_Rule()
        {
            var builder = PassphraseRegex.Which.ExcludesRange('a', 'z');

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?!.*[a-z])(?!^\s|.*\s$).+$");
        }

        [Test]
        public void Can_Specify_Excluded_Range_Rule_Using_Numbers()
        {
            var builder = PassphraseRegex.That.ExcludesRange(0, 9);

            var result = builder.ToRegex();

            result.Pattern.ShouldBe(@"^(?!.*[0-9])(?!^\s|.*\s$).+$");
        }

        [TestCase(true, 0, 1, 2, 3, 4, 5)]
        [TestCase(false, 6, 7, 8, 9)]
        public void Numeric_Range_0_5_Accepts_Valid_Numbers(bool isValid, params int[] inputs)
        {
            var builder = PassphraseRegex.That.IncludesRange(0, 5);

            var result = builder.ToRegex();

            inputs.All(i => result.Regex.IsMatch(i.ToString())).ShouldBe(isValid);
        }

        [TestCase(true, 5, 6, 7, 8, 9)]
        [TestCase(false, 0, 1, 2, 3, 4)]
        public void Numeric_Range_0_4_Rejects_Invalid_Numbers(bool isValid, params int[] inputs)
        {
            var builder = PassphraseRegex.That.ExcludesRange(0, 4);

            var result = builder.ToRegex();

            inputs.All(i => result.Regex.IsMatch(i.ToString())).ShouldBe(isValid);
        }

        [Test]
        public void Negative_Numeric_Range_Start_Value_Throws_ArgumentOutOfRangeException()
        {
            var exception = Should.Throw<ArgumentOutOfRangeException>(() => PassphraseRegex.That.IncludesRange(-1, 9));
            exception.ParamName.ShouldBe("start");
        }

        [Test]
        public void Negative_Numeric_Range_End_Value_Throws_ArgumentOutOfRangeException()
        {
            var exception = Should.Throw<ArgumentOutOfRangeException>(() => PassphraseRegex.That.ExcludesRange(0, -9));
            exception.ParamName.ShouldBe("end");
        }

        [Test]
        public void Numeric_Range_Start_Value_Greater_Than_Ten_Throws_FormatException()
        {
            var exception = Should.Throw<ArgumentOutOfRangeException>(() => PassphraseRegex.That.ExcludesRange(10, 1));
            exception.ParamName.ShouldBe("start");
        }

        [Test]
        public void Numeric_Range_End_Value_Greater_Than_Ten_Throws_FormatException()
        {
            var exception = Should.Throw<ArgumentOutOfRangeException>(() => PassphraseRegex.That.IncludesRange(0, 10));
            exception.ParamName.ShouldBe("end");
        }

        [TestCase(true, "abc123")]
        [TestCase(true, "aBc123!@#")]
        [TestCase(false, "ABC0")]
        [TestCase(false, "abcd")]
        [TestCase(false, "1234")]
        public void Input_Must_Contain_A_Lowercase_Letter_And_A_Number(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesRange('a', 'z')
                                              .IncludesRange(0, 9);

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "123")]
        [TestCase(true, "ABC")]
        [TestCase(false, "a1b2c3")]
        [TestCase(false, "abc")]
        public void Input_Excludes_LowerCase_Letters_From_A_To_Z(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.ExcludesRange('a', 'z');

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "abc")]
        [TestCase(false, "a1b2c3")]
        [TestCase(false, "123")]
        public void Input_Must_Not_Contain_Numbers(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.ExcludesRange(0, 9);

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "012345")]
        [TestCase(false, "12")]
        [TestCase(false, "6789")]
        public void Input_Of_Min_Length_3_Contains_Numbers_Between_1_to_5(bool isValid, string input)
        {
            var builder = PassphraseRegex.With.MinLength(3)
                                              .ExcludesRange(6, 9);

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, 'A', 'Z')]
        [TestCase(false, ' ', '/')]
        // Space to forward slash range: 32-47, includes [ !"#$%&'()*+,-./]
        public void Input_Excludes_Range_Of_Space_To_Forward_Slash(bool isValid, char startChar, char endChar)
        {
            var builder = PassphraseRegex.That.ExcludesRange(' ', '/');

            var result = builder.ToRegex();
            var range = Enumerable.Range((int)startChar, (int)endChar + 1 - (int)startChar).Select(i => ((char)i).ToString());

            result.IsValid.ShouldBe(true);
            range.All(c => Regex.IsMatch(c, result.Pattern)).ShouldBe(isValid);
        }

        [TestCase(true, "abc12")]
        [TestCase(true, "abc123")]
        [TestCase(true, "a1b2c3")]
        [TestCase(false, "abc1")]
        [TestCase(false, "abc")]
        public void Input_Contains_At_Least_Two_Digits_In_Specified_Range(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesRange(0, 9)
                                              .WithMinimumOccurrenceOf(2);

            var result = builder.ToRegex();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase("abcdefg")]
        [TestCase("abCDEfg")]
        public void Input_Includes_Letters_From_C_To_E_With_RegexOptions_IgnoreCase_Accepts_Any_Casing(string input)
        {
            var builder = PassphraseRegex.That.IncludesRange('c', 'e')
                                              .Options(RegexOptions.IgnoreCase);

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(true);
            result.Regex.Options.ShouldBe(RegexOptions.IgnoreCase);
        }

        [TestCase("abcdefg")]
        [TestCase("abCDEfg")]
        public void Input_Excludes_Letters_From_C_To_E_With_RegexOptions_IgnoreCase_Rejects_Any_Casing(string input)
        {
            var builder = PassphraseRegex.That.ExcludesRange('C', 'E')
                                              .Options(RegexOptions.IgnoreCase);

            var result = builder.ToRegex();

            result.Regex.IsMatch(input).ShouldBe(false);
            result.Regex.Options.ShouldBe(RegexOptions.IgnoreCase);
        }
    }
}
