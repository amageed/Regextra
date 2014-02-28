using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests.TemplateTests
{
    [TestFixture]
    public class TemplateTests
    {
        [Test]
        public void Input_Without_Template_Tokens_Returns_Original_Input()
        {
            var template = "The quick brown fox jumped over the lazy dog.";

            var result = Template.Format(template, new { });

            result.ShouldBe(template);
        }

        [Test]
        public void Template_With_Tokens_Is_Populated_With_Matching_Items()
        {
            var template = "The quick {Color} {Animal1} jumped over the lazy {Animal2}.";
            var item = new { Color = "brown", Animal1 = "fox", Animal2 = "dog" };

            var result = Template.Format(template, item);

            var expected = "The quick brown fox jumped over the lazy dog.";
            result.ShouldBe(expected);
        }

        [Test]
        public void Template_Throws_MissingFieldException_When_Object_Is_Missing_Any_Token()
        {
            var template = "The quick {Color} {Animal1} jumped over the lazy {Animal2}.";
            var item = new { Color = "brown", Animal2 = "dog" };

            var ex = Should.Throw<MissingFieldException>(() => Template.Format(template, item));
            ex.Message.ShouldContain(".Animal1");
        }

        [TestCase("{Name", "{Name")]
        [TestCase("{{Name", "{{Name")]
        [TestCase("Name}", "Name}")]
        [TestCase("Name}}", "Name}}")]
        public void Template_Delimiters_Are_Ignored_When_One_Sided(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }

        [TestCase("{{Name}}", "{Name}")]
        [TestCase("{{{{Name}}}}", "{{Name}}")]
        public void Balanced_Even_Delimiters_Are_Escaped(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }

        [TestCase("{Name}", "Ahmad")]
        [TestCase("{{{Name}}}", "{Ahmad}")]
        public void Balanced_Odd_Delimiters_Yield_Trimmed_Delimiters_And_Property_Value(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }

        [TestCase("{{{Name}", "{Ahmad")]
        [TestCase("{Name}}}", "Ahmad}")]
        public void Template_Is_Formatted_With_Odd_Delimiter_Pairs(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }

        [TestCase("{{Name}", "{Name}")]
        [TestCase("{Name}}", "{Name}")]
        public void Template_Is_Escaped_With_Even_And_Odd_Delimiter_Pairs(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }

        [Test]
        public void Can_Apply_Custom_Format_To_Guid()
        {
            Guid guid = Guid.NewGuid();
            var template = "{Item:N}";
            var expected = guid.ToString("N");

            var result = template.TemplateFormat(new { Item = guid });

            result.ShouldBe(expected);
        }

        [TestCase("d")]
        [TestCase("F")]
        [TestCase("g")]
        [TestCase("M")]
        [TestCase("R")]
        [TestCase("t")]
        [TestCase("MM-dd-yy")]
        [TestCase("dd/MM/yyyy gg")]
        [TestCase("MM/dd/yy H:mm:ss zzz")]
        public void Can_Apply_Standard_And_Custom_Formats_To_DateTime(string format)
        {
            DateTime date = DateTime.UtcNow;
            var template = "{Item:" + format + "}";
            var expected = date.ToString(format);

            var result = Template.Format(template, new { Item = date });

            result.ShouldBe(expected);
        }

        [TestCase("C")]
        [TestCase("C3")]
        [TestCase("e")]
        [TestCase("E4")]
        [TestCase("P")]
        public void Can_Apply_Standard_And_Custom_Formats_To_Numbers(string format)
        {
            var number = 1234.56789;
            var template = "{Item:" + format + "}";
            var expected = number.ToString(format);

            var result = Template.Format(template, new { Item = number });

            result.ShouldBe(expected);
        }

        [TestCase(255, "x")]
        [TestCase(10, "X4")]
        public void Can_Apply_Hexadecimal_Formatting(int number, string format)
        {
            var template = "{Item:" + format + "}";
            var expected = number.ToString(format);

            var result = Template.Format(template, new { Item = number });

            result.ShouldBe(expected);
        }
    }
}
