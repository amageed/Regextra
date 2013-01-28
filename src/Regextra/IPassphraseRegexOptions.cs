using System;

namespace Regextra
{
    public interface IPassphraseRegexOptions : IPassphraseRegex
    {
        IPassphraseRegex WithMinimumOccurrenceOf(int length);
    }
}
