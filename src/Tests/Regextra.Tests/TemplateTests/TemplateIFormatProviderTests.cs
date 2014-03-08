using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Shouldly;

namespace Regextra.Tests.TemplateTests
{
    [TestFixture]
    public class TemplateIFormatProviderTests
    {
        [Test]
        public void Template_With_IFormatProvider_And_EN_US_Culture_Returns_Expected_Currency_Format()
        {
            var template = "This item costs {Price:C2}";
            var item = new { Price = 1234.567m };
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            var result = Template.Format(template, item, NumberFormatInfo.CurrentInfo);

            result.ShouldBe("This item costs $1,234.57");
        }

        [Test]
        public void Template_With_IFormatProvider_And_EN_GB_Culture_Returns_Expected_Currency_Format()
        {
            var template = "This item costs {Price:C1}";
            var item = new { Price = 1234.567m };
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            var result = Template.Format(template, item, NumberFormatInfo.CurrentInfo);

            result.ShouldBe("This item costs " + item.Price.ToString("C1", NumberFormatInfo.CurrentInfo));
        }

        [Test]
        public void Template_Nested_Properties_With_IFormatProvider_And_EN_GB_Culture_Returns_Expected_Currency_Format()
        {
            var template = "This item costs {Details.Item1:C2}";
            var item = new { Details = Tuple.Create(1234.567m) };
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            var result = Template.Format(template, item, NumberFormatInfo.CurrentInfo);

            result.ShouldBe("This item costs " + item.Details.Item1.ToString("C2", NumberFormatInfo.CurrentInfo));
        }
    }
}
