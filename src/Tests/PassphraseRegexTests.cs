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
    public class PassphraseRegexTests
    {
        [Test]
        public void Verify_Pattern_For_MinLength_Of_1()
        {
            var builder = PassphraseRegex.With.MinLength(1)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[abc]).+$");
        }

        [Test]
        public void Verify_Pattern_For_MinLength_Of_2()
        {
            var builder = PassphraseRegex.With.MinLength(2)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[abc]).{2,}$");
        }

        [Test]
        public void Verify_Pattern_For_MaxLength_Of_1()
        {
            var builder = PassphraseRegex.With.MaxLength(1)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[abc]).$");
        }

        [Test]
        public void Verify_Pattern_For_MaxLength_Of_2()
        {
            var builder = PassphraseRegex.With.MaxLength(2)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[abc]).{1,2}$");
        }

        [Test]
        public void Verify_Pattern_For_MinLength_Of_2_And_MaxLength_Of_5()
        {
            var builder = PassphraseRegex.With.MinLength(2)
                                              .MaxLength(5)
                                              .IncludesAnyCharacters("abc");

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[abc]).{2,5}$");
        }

        [Test]
        public void Can_Specify_Range_Rule()
        {
            var builder = PassphraseRegex.That.IncludesRange('a', 'z');

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[a-z]).+$");
        }

        [Test]
        public void Can_Specify_Excluded_Range_Rule()
        {
            var builder = PassphraseRegex.Which.ExcludesRange('a', 'z');

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?!.*[a-z]).+$");
        }

        [Test]
        public void Invalid_Result_When_Specifying_A_Reversed_Range()
        {
            var builder = PassphraseRegex.That.IncludesRange('z', 'a');

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(false);
            result.Error.ShouldContain("range in reverse order");
        }

        [Test]
        public void Can_Specify_Contained_Characters()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("abc");

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[abc]).+$");
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

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?!.*[abc]).+$");
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

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[az-]).+$");
        }

        [TestCase("[")]
        [TestCase("]")]
        [TestCase("-")]
        [TestCase("\\")]
        public void Included_Characters_Can_Handle_Character_Class_Considerations(string input)
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("[]-\\");

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            result.Pattern.ShouldBe(@"^(?=.*[[\]\\-]).+$");
            Regex.IsMatch(input, result.Pattern).ShouldBe(true);
        }
        
        [Test]
        public void Included_Characters_Will_Ignore_Character_Class_Range()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("[a-z]");

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            result.Pattern.ShouldBe(@"^(?=.*[[az\]-]).+$");
            Regex.IsMatch("b", result.Pattern).ShouldBe(false);
        }

        [Test]
        public void Minimum_Length_Is_One_When_One_Rule_Exists()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("a");

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[a]).+$");
        }

        [Test]
        public void Minimum_Length_Is_Two_When_Two_Rules_Exist()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("a")
                                              .IncludesRange('1', '9');

            var result = builder.ToPattern();

            result.Pattern.ShouldBe("^(?=.*[a])(?=.*[1-9]).{2,}$");
        }

        [Test]
        public void Throws_ArgumentException_When_Assigned_Minimum_Length_Is_Less_Than_Specified_Rules_Count()
        {
            var builder = PassphraseRegex.With.MinLength(1)
                                              .IncludesAnyCharacters("a")
                                              .IncludesRange('1', '9');

            var exception = Should.Throw<ArgumentException>(() => builder.ToPattern());
            exception.Message.ShouldStartWith("Minimum length");
        }

        [Test]
        public void Throws_ArgumentException_When_Assigned_Maximum_Length_Is_Less_Than_Specified_Rules_Count()
        {
            var builder = PassphraseRegex.With.MaxLength(1)
                                              .IncludesAnyCharacters("a")
                                              .IncludesRange('1', '9');

            var exception = Should.Throw<ArgumentException>(() => builder.ToPattern());
            exception.Message.ShouldStartWith("Maximum length");
        }

        [TestCase(true, "abc123")]
        [TestCase(true, "aBc123!@#")]
        [TestCase(false, "ABC0")]
        [TestCase(false, "abcd")]
        [TestCase(false, "1234")]
        public void Input_Must_Contain_A_Lowercase_Letter_And_A_Number(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesRange('a', 'z')
                                              .IncludesRange('0', '9');

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "abc")]
        [TestCase(false, "a")]
        public void Input_Must_Meet_MinLength_Rule(bool isValid, string input)
        {
            var builder = PassphraseRegex.With.MinLength(2)
                                              .IncludesRange('a', 'z');

            var result = builder.ToPattern();

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

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "123")]
        [TestCase(true, "ABC")]
        [TestCase(true, "xYz")]
        [TestCase(false, "a1b2c3")]
        [TestCase(false, "abc")]
        public void Input_Must_Not_Contain_LowerCase_Letters_ABC(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.ExcludesCharacters("abc");

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "abc")]
        [TestCase(false, "a1b2c3")]
        [TestCase(false, "123")]
        public void Input_Must_Not_Contain_Numbers(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.ExcludesRange('0', '9');

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, "012345")]
        [TestCase(false, "12")]
        [TestCase(false, "6789")]
        public void Input_Of_Min_Length_3_Contains_Numbers_Between_1_to_5(bool isValid, string input)
        {
            var builder = PassphraseRegex.With.MinLength(3)
                                              .ExcludesRange('6', '9');

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }

        [TestCase(true, 'A', 'Z')]
        [TestCase(false, ' ', '/')]
        // Space to forward slash range: 32-47, includes [ !"#$%&'()*+,-./]
        public void Input_Excludes_Range_Of_Space_To_Forward_Slash(bool isValid, char startChar, char endChar)
        {
            var builder = PassphraseRegex.That.ExcludesRange(' ', '/');

            var result = builder.ToPattern();
            var range = Enumerable.Range((int)startChar, (int)endChar + 1).Select(i => ((char)i).ToString());

            result.IsValid.ShouldBe(true);
            range.All(c => Regex.IsMatch(c, result.Pattern)).ShouldBe(isValid);
        }

        [Test]
        public void Throws_ArgumentOutOfRangeException_With_Minimum_Occurrence_Less_Than_One()
        {
            var builder = PassphraseRegex.That.IncludesAnyCharacters("abc");

            var exception = Should.Throw<ArgumentOutOfRangeException>(() => builder.WithMinimumOccurrenceOf(0));
            exception.ParamName.ShouldBe("length");
        }

        [TestCase(true, "abc12")]
        [TestCase(true, "abc123")]
        [TestCase(true, "a1b2c3")]
        [TestCase(false, "abc1")]
        [TestCase(false, "abc")]
        public void Input_Contains_At_Least_Two_Digits_In_Specified_Range(bool isValid, string input)
        {
            var builder = PassphraseRegex.That.IncludesRange('0', '9')
                                              .WithMinimumOccurrenceOf(2);

            var result = builder.ToPattern();

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

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
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

            var result = builder.ToPattern();

            result.IsValid.ShouldBe(true);
            Regex.IsMatch(input, result.Pattern).ShouldBe(isValid);
        }
    }
}
