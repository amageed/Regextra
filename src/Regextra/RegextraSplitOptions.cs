using System;

namespace Regextra
{
    [Flags]
    public enum RegextraSplitOptions
    {
        None = 0,
        IncludeDelimiters = 1,
        MatchWholeWords = 2,
        TrimWhitespace = 4,
        RemoveEmptyEntries = 8,
        All = IncludeDelimiters | MatchWholeWords | TrimWhitespace | RemoveEmptyEntries
    }
}
