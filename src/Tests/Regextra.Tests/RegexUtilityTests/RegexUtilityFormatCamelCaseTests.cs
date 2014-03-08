using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests.RegexUtilityTests
{
    [TestFixture]
    public class RegexUtilityFormatCamelCaseTests
    {
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
            string result = RegexUtility.FormatCamelCase(input, camelCaseOptions: CamelCaseOptions.CapitalizeFirstCharacter);

            result.ShouldBe(expected);
        }

        [Test]
        public void Turkish_I_Returned_With_Turkish_Culture_And_CapitalizeFirstCharacter_Option()
        {
            string input = "inTheZone";
            string expected = "İn The Zone";
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("tr");

            string result = RegexUtility.FormatCamelCase(input, camelCaseOptions: CamelCaseOptions.CapitalizeFirstCharacter);

            result.ShouldBe(expected);
        }

        [Test]
        public void Regular_I_Returned_With_Turkish_Culture_And_CapitalizeFirstCharacterInvariantCulture_Option()
        {
            string input = "invariantCulture";
            string expected = "Invariant Culture";
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("tr");

            string result = RegexUtility.FormatCamelCase(input, camelCaseOptions: CamelCaseOptions.CapitalizeFirstCharacterInvariantCulture);

            result.ShouldBe(expected);
        }
    }
}
