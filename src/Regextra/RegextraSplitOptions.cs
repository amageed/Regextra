using System;

namespace Regextra
{
    [Flags]
    public enum RegextraSplitOptions
    {
        None = 0,
        RemoveEmptyEntries = 1,
        MatchWholeWords = 2,
        TrimWhitespace = 4
    }
}
