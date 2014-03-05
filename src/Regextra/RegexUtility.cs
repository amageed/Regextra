using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regextra
{
    public static class RegexUtility
    {
        public static string[] SplitIncludeDelimiters(string input,
            string[] delimiters,
            RegexOptions regexOptions = RegexOptions.None,
            RegextraSplitOptions splitOptions = RegextraSplitOptions.None)
        {
            if (delimiters == null || delimiters.Length == 0)
                throw new ArgumentException("Delimiters can't be empty", "delimiters");

            var pattern = "(" + String.Join("|", delimiters.Select(d => Regex.Escape(d))) + ")";

            if (splitOptions.HasFlag(RegextraSplitOptions.MatchWholeWords))
            {
                pattern = PrefixSuffixString(pattern, @"\b");
            }
            if (splitOptions.HasFlag(RegextraSplitOptions.TrimWhitespace))
            {
                pattern = PrefixSuffixString(pattern, @"\s*");
            }

            string[] result = Regex.Split(input, pattern, regexOptions);
            if (splitOptions.HasFlag(RegextraSplitOptions.RemoveEmptyEntries))
            {
                result = RemoveEmptyEntries(result);
            }
            return result;
        }

        private static string PrefixSuffixString(string input, string prefixSuffix)
        {
            return prefixSuffix + input + prefixSuffix;
        }

        private static string[] RemoveEmptyEntries(string[] input)
        {
            var result = input.Where(s => !String.IsNullOrEmpty(s)).ToArray();
            return result;
        }
    }
}
