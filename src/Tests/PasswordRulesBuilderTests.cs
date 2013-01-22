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
    public class PasswordRulesBuilderTests
    {
        [Test]
        public void Can_Initialize()
        {
            var builder = new PasswordRulesBuilder();

            var pattern = builder.ToPattern();
            
            pattern.ShouldBeEmpty();
        }

        [Test]
        public void Can_Add_Range_Rule()
        {
            var builder = new PasswordRulesBuilder().Range('a', 'z');
            
            var pattern = builder.ToPattern();

            pattern.ShouldBe("^(?=.*[a-z]).{1,}$");
        }

        [Test]
        public void Can_Specify_Contained_Characters()
        {
            var builder = new PasswordRulesBuilder().ContainsCharacters("abc");

            var pattern = builder.ToPattern();

            pattern.ShouldBe("^(?=.*[abc]).{1,}$");
        }

        [TestCase("a-z")]
        [TestCase("az-")]
        [TestCase("-az")]
        [TestCase("a--z")]
        public void Dash_In_Contained_Characters_Placed_At_End_To_Avoid_Unintended_Range(string characters)
        {
            var builder = new PasswordRulesBuilder().ContainsCharacters(characters);

            var pattern = builder.ToPattern();

            pattern.ShouldBe("^(?=.*[az-]).{1,}$");
        }

        [Test]
        public void Minimum_Length_Is_One_When_One_Rule_Exists()
        {
            var builder = new PasswordRulesBuilder().ContainsCharacters("a");

            var pattern = builder.ToPattern();

            pattern.ShouldBe("^(?=.*[a]).{1,}$");
        }

        [Test]
        public void Minimum_Length_Is_Two_When_Two_Rules_Exist()
        {
            var builder = new PasswordRulesBuilder().ContainsCharacters("a")
                                                    .Range('1', '9');

            var pattern = builder.ToPattern();

            pattern.ShouldBe("^(?=.*[a])(?=.*[1-9]).{2,}$");
        }

        [Test]
        public void Throws_ArgumentException_When_Assigned_Minimum_Length_Is_Less_Than_Specified_Rules_Count()
        {
            var builder = new PasswordRulesBuilder().MinLength(1)
                                                    .ContainsCharacters("a")
                                                    .Range('1', '9');

            var exception = Should.Throw<ArgumentException>(() => builder.ToPattern());
            exception.Message.ShouldStartWith("Minimum length");
        }

        [Test]
        public void Throws_ArgumentException_When_Assigned_Maximum_Length_Is_Less_Than_Specified_Rules_Count()
        {
            var builder = new PasswordRulesBuilder().MaxLength(1)
                                                    .ContainsCharacters("a")
                                                    .Range('1', '9');

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
            var builder = new PasswordRulesBuilder().Range('a', 'z')
                                                    .Range('0', '9');
            
            var pattern = builder.ToPattern();
            
            Regex.IsMatch(input, pattern).ShouldBe(isValid);
        }

        [TestCase(true, "abc")]
        [TestCase(false, "a")]
        public void Input_Must_Meet_MinLength_Rule(bool isValid, string input)
        {
            var builder = new PasswordRulesBuilder().MinLength(2)
                                                    .Range('a', 'z');

            var pattern = builder.ToPattern();

            Regex.IsMatch(input, pattern).ShouldBe(isValid);
        }

        [TestCase(true, "123")]
        [TestCase(true, "ABC")]
        [TestCase(false, "a1b2c3")]
        [TestCase(false, "abc")]
        public void Input_Must_Not_Contain_LowerCase_Letters(bool isValid, string input)
        {
            var builder = new PasswordRulesBuilder().ExcludesRange('a', 'z');

            var pattern = builder.ToPattern();

            Regex.IsMatch(input, pattern).ShouldBe(isValid);
        }

        [TestCase(true, "abc")]
        [TestCase(false, "a1b2c3")]
        [TestCase(false, "123")]
        public void Input_Must_Not_Contain_Numbers(bool isValid, string input) 
        {
            var builder = new PasswordRulesBuilder().ExcludesRange('0', '9');

            var pattern = builder.ToPattern();

            Regex.IsMatch(input, pattern).ShouldBe(isValid);
        }

        [TestCase(true, "012345")]
        [TestCase(false, "12")]
        [TestCase(false, "6789")]
        public void Input_Of_Min_Length_3_Contains_Numbers_Between_1_to_5(bool isValid, string input)
        {
            var builder = new PasswordRulesBuilder().MinLength(3)
                                                    .ExcludesRange('6', '9');

            var pattern = builder.ToPattern();

            Regex.IsMatch(input, pattern).ShouldBe(isValid);
        }

        [TestCase(true, 'A', 'Z')]
        [TestCase(false, ' ', '/')]
        // Space to forward slash range: 32-47, includes [ !"#$%&'()*+,-./]
        public void Input_Excludes_Range_Of_Space_To_Forward_Slash(bool isValid, char startChar, char endChar)
        {
            var builder = new PasswordRulesBuilder().ExcludesRange(' ', '/');

            var pattern = builder.ToPattern();
            var range = Enumerable.Range((int)startChar, (int)endChar + 1).Select(i => ((char)i).ToString());

            range.All(c => Regex.IsMatch(c, pattern)).ShouldBe(isValid);
        }
    }
}
