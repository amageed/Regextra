using System;

namespace Regextra
{
    [Flags]
    public enum CamelCaseOptions
    {
        None = 0,
        CapitalizeFirstCharacter = 1,
        CapitalizeFirstCharacterInvariantCulture = 2
    }
}