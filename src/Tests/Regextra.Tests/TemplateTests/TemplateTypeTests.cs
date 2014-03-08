using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;
using Regextra.Tests.TemplateTests.Models;
using Shouldly;

namespace Regextra.Tests.TemplateTests
{
    [TestFixture]
    public class TemplateTypeTests
    {
        [Test]
        public void Template_Accepts_Dictionary_Objects()
        {
            var template = "Hello {Name}! The item is {Item}.";
            var item = new Dictionary<string, object>()
            {
                { "Name", "Ahmad" },
                { "Item", true }
            };

            var result = Template.Format(template, item);

            result.ShouldBe("Hello Ahmad! The item is True.");
        }

        [Test]
        public void Template_Dictionary_With_Concrete_TValue_Returns_Expected_Result()
        {
            var template = "The access code is {Code} and the token is {Token:N}.";
            var item = new Dictionary<string, Guid>()
            {
                { "Code", Guid.NewGuid() },
                { "Token", Guid.NewGuid() }
            };

            var result = Template.Format(template, item);

            result.ShouldBe(String.Format("The access code is {0} and the token is {1}.", item["Code"], item["Token"].ToString("N")));
        }

        [Test]
        public void Template_Accepts_ExpandoObject()
        {
            var template = "Hello {Name}! The item is {Item}.";
            dynamic item = new ExpandoObject();
            item.Name = "Ahmad";
            item.Item = true;

            var result = Template.Format(template, item);

            Assert.AreEqual(result, "Hello Ahmad! The item is True.");
        }

        [Test]
        public void Template_Accepts_Dynamic_Object()
        {
            var template = "Hello {Id}:{Name}.";
            dynamic item = new Person { Id = 42, Name = "Ahmad" };

            var result = Template.Format(template, item);

            Assert.AreEqual(result, "Hello 42:Ahmad.");
        }
    }
}
