using System;

namespace Regextra
{
    [Flags]
    public enum SplitOptions
    {
        None = 0,
        IncludeDelimiters = 1,
        MatchWholeWords = 2,
        TrimWhitespace = 4,
        RemoveEmptyEntries = 8,
        All = IncludeDelimiters | MatchWholeWords | TrimWhitespace | RemoveEmptyEntries
    }
}
