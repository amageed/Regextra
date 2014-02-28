using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Regextra.Tests.TemplateTests.Models;
using Shouldly;

namespace Regextra.Tests.TemplateTests
{
    [TestFixture]
    public class TemplateNestedPropertyTests
    {
        [Test]
        public void Template_Can_Access_Nested_Address1_Property()
        {
            var person = GeneratePersonMock();
            var template = "{Address.Address1}";

            var result = Template.Format(template, person);

            result.ShouldBe(person.Address.Address1);
        }

        [Test]
        public void Template_Can_Access_Nested_ZipCode_Item1_Property()
        {
            var person = GeneratePersonMock();
            var template = "{Address.ZipCode.Item1}";

            var result = Template.Format(template, person);

            result.ShouldBe(person.Address.ZipCode.Item1);
        }

        [Test]
        public void Template_Can_Format_Nested_DateModified_Property()
        {
            var person = GeneratePersonMock();
            var template = "{Address.DateModified:t}";

            var result = Template.Format(template, person);

            result.ShouldBe(person.Address.DateModified.ToString("t"));
        }

        private Person GeneratePersonMock()
        {
            var fixture = new Fixture();
            var person = fixture.CreateAnonymous<Person>();
            return person;
        }
    }
}