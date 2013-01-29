using System;

namespace Regextra
{
    public class PatternResult
    {
        public string Pattern { get; private set; }
        public string Error { get; private set; }
        public bool IsValid
        {
            get { return Error == null; }
        }

        public PatternResult(string pattern, string error)
        {
            Pattern = pattern;
            Error = error;
        }
    }
}
