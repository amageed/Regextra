using System;

namespace Regextra
{
    internal class Rule : IRule
    {
        private readonly string _pattern;
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
        }

        internal virtual string FormatRequirement(string input)
        {
            return "(?=.*" + input + ")";
        }
    }
}
