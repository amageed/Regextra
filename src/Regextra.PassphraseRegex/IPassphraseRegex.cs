using System;
using System.Text.RegularExpressions;

namespace Regextra
{
    public interface IPassphraseRegex
    {
        IPassphraseRegex IncludesText(string text);
        IPassphraseRegex ExcludesText(string text);
        IPassphraseRegexOptions IncludesAnyCharacters(string characters);
        IPassphraseRegex ExcludesCharacters(string characters);
        IPassphraseRegexOptions IncludesRange(char start, char end);
        IPassphraseRegexOptions IncludesRange(int start, int end);
        IPassphraseRegex ExcludesRange(char start, char end);
        IPassphraseRegex ExcludesRange(int start, int end);
        IPassphraseRegex MinLength(int length);
        IPassphraseRegex MaxLength(int length);
        IPassphraseRegex MaxConsecutiveIdenticalCharacterOf(int length);
        IPassphraseRegex Options(RegexOptions options);
        PassphraseRegexResult ToRegex();
    }
}