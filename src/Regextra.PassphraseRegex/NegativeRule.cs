using System;

namespace Regextra
{
    internal class NegativeRule : Rule
    {
        public NegativeRule(string pattern)
            : base(pattern)
        {
        }

        internal override string FormatRequirement(string input)
        {
            return "(?!.*" + input + ")";
        }
    }
}
