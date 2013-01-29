using System;

namespace Regextra
{
    public interface IPassphraseRegex
    {
        IPassphraseRegexOptions ContainsCharacters(string characters);
        IPassphraseRegex ExcludesCharacters(string characters);
        IPassphraseRegexOptions IncludesRange(char start, char end);
        IPassphraseRegex ExcludesRange(char start, char end);
        IPassphraseRegex MinLength(int length);
        IPassphraseRegex MaxLength(int length);
        PatternResult ToPattern();
    }
}