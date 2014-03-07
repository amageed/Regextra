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

        [Test]
        public void Trim_Whitespaces_Throws_Exception_For_Null_Input_From_Regex_Class()
        {
            var ex = Should.Throw<ArgumentNullException>(() => RegexUtility.TrimWhitespaces(null));

            ex.ParamName.ShouldBe("input");
        }

        [TestCase("Hello World   ")]
        [TestCase("  Hello World")]
        [TestCase("Hello    World")]
        [TestCase("   Hello    World   ")]
        public void Can_Trim_Whitespaces_In_Given_Inputs(string input)
        {
            var expected = "Hello World";

            var result = RegexUtility.TrimWhitespaces(input);

            result.ShouldBe(expected);
        }

        [TestCase("Pascal")]
        [TestCase("camel")]
        [TestCase("XML")]
        public void CamelCase_Format_Of_Single_Word_Returns_Original_Word_Unmodified(string input)
        {
            string result = RegexUtility.FormatCamelCase(input);

            result.ShouldBe(input);
        }

        [TestCase("PascalCase", "Pascal Case")]
        [TestCase("Blah PascalCase Blah", "Blah Pascal Case Blah")]
        [TestCase("camelCase", "camel Case")]
        [TestCase("Blah camelCase Blah", "Blah camel Case Blah")]
        [TestCase("PickUpXMLInFiveDays", "Pick Up XML In Five Days")]
        public void Can_Format_CamelCase_With_Spaces_By_Default(string input, string expected)
        {
            string result = RegexUtility.FormatCamelCase(input);

            result.ShouldBe(expected);
        }

        [TestCase("PascalCase", "Pascal_Case", "_")]
        [TestCase("PascalCase", "Pascal.Case", ".")]
        [TestCase("PascalCase", "Pascalx123YCase", "x123Y")]
        public void Can_Format_CamelCase_With_Given_Delimiter(string input, string expected, string delimiter)
        {
            string result = RegexUtility.FormatCamelCase(input, delimiter);

            result.ShouldBe(expected);
        }

        [TestCase(null)]
        [TestCase("")]
        public void CamelCase_Format_Throws_Exception_For_Null_Or_Empty_Delimiter(string delimiter)
        {
            var ex = Should.Throw<ArgumentException>(() => RegexUtility.FormatCamelCase("PascalCase", delimiter));

            ex.ParamName.ShouldBe("delimiter");
        }

        [TestCase("PascalCase1", "Pascal Case 1")]
        [TestCase("camel1Case2", "camel 1 Case 2")]
        [TestCase("PascalC1", "Pascal C 1")]
        public void Can_Format_CamelCase_Inputs_With_Numbers(string input, string expected)
        {
            string result = RegexUtility.FormatCamelCase(input);

            result.ShouldBe(expected);
        }

        [TestCase("camelCase", "Camel Case")]
        public void Can_Format_CamelCase_And_Capitalize_First_Character(string input, string expected)
        {
            string result = RegexUtility.FormatCamelCase(input, capitalizeFirstCharacter: true);

            result.ShouldBe(expected);
        }
    }
}
