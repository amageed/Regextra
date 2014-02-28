using System;
using System.Collections.Generic;
using System.Linq;

namespace Regextra.Tests.TemplateTests.Models
{
    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        // Purposely convoluted data type for Zip+4 format to get extra nesting
        public Tuple<string, string> ZipCode { get; set; }
        public DateTime DateModified { get; set; }
    }
}
