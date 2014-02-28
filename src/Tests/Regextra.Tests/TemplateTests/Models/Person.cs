using System;
using System.Collections.Generic;
using System.Linq;

namespace Regextra.Tests.TemplateTests.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Address Address { get; set; }
    }
}
