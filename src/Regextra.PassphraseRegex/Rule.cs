using System;
using System.Linq;

namespace Regextra
{
    internal class Rule : IRule
    {
        private readonly string _pattern;
        public int MinimumOccurrence { get; set; }
        public string Requirement
        {
            get
            {
                return FormatRequirement(_pattern);
            }
        }

        public Rule(string pattern)
        {
            _pattern = pattern;
            MinimumOccurrence = 1;
        }

        internal virtual string FormatRequirement(string input)
        {
            return "(?=" + RepeatPattern(input) + ")";
        }

        private string RepeatPattern(string input)
        {
            var pattern = ".*" + input;
            if (MinimumOccurrence > 1)
            {
                return String.Join("", Enumerable.Repeat(pattern, MinimumOccurrence));
            }
            return pattern;
        }
    }
}
