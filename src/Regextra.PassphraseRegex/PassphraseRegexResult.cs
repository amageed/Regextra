using System;
using System.Text.RegularExpressions;

namespace Regextra
{
    public class PassphraseRegexResult
    {
        public Regex Regex { get; private set; }
        public string Pattern { get; private set; }
        public string Error { get; private set; }
        public bool IsValid
        {
            get { return Error == null && Regex != null; }
        }

        public PassphraseRegexResult(Regex regex, string pattern, string error)
            : this(pattern, error)
        {
            Regex = regex;
        }

        public PassphraseRegexResult(string pattern, string error)
        {
            Pattern = pattern;
            Error = error;
        }
    }
}
