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

        [TestCase("{Name}", "Ahmad")]
        [TestCase("{{Name}}", "{Name}")]
        [TestCase("{{{Name}}}", "{Ahmad}")]
        [TestCase("{{{{Name}}}}", "{{Name}}")]
        [TestCase("{{{{{Name}}}}}", "{{Ahmad}}")]
        public void Template_Delimiters_Are_Escaped_When_Doubled_Up(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }

        [TestCase("{Name", "{Name")]
        [TestCase("{{Name", "{{Name")]
        [TestCase("Name}", "Name}")]
        [TestCase("Name}}", "Name}}")]
        public void Template_Delimiters_Are_Ignored_When_Unbalanced(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }

        [TestCase("{{Name}", "{Ahmad")]
        [TestCase("{Name}}", "Ahmad}")]
        public void Template_Is_Formatted_When_Tokens_Are_Partially_Balanced(string template, string expected)
        {
            var result = Template.Format(template, new { Name = "Ahmad" });

            result.ShouldBe(expected);
        }
    }
}
