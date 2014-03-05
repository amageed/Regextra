using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Regextra
{
    public static class RegexUtility
    {
        public static string[] Split(string input, 
            string[] delimiters, 
            RegexOptions regexOptions = RegexOptions.None, 
            RegextraSplitOptions splitOptions = RegextraSplitOptions.None)
        {
            if (delimiters == null || delimiters.Length == 0)
                throw new ArgumentException("Delimiters can't be empty", "delimiters");

            var pattern = new StringBuilder(String.Join("|", delimiters.Select(d => Regex.Escape(d))));

            if (splitOptions.HasFlag(RegextraSplitOptions.IncludeDelimiters))
            {
                PrefixSuffix(pattern, "(", ")");
            }
            if (splitOptions.HasFlag(RegextraSplitOptions.MatchWholeWords))
            {
                PrefixSuffix(pattern, @"\b");
            }
            if (splitOptions.HasFlag(RegextraSplitOptions.TrimWhitespace))
            {
                PrefixSuffix(pattern, @"\s*");
            }

            string[] result = Regex.Split(input, pattern.ToString(), regexOptions);
            if (splitOptions.HasFlag(RegextraSplitOptions.RemoveEmptyEntries))
            {
                result = RemoveEmptyEntries(result);
            }
            return result;
        }

        private static void PrefixSuffix(StringBuilder input, string prefixSuffix)
        {
            input.Insert(0, prefixSuffix).Append(prefixSuffix);
        }

        private static void PrefixSuffix(StringBuilder input, string prefix, string suffix)
        {
            input.Insert(0, prefix).Append(suffix);
        }

        private static string[] RemoveEmptyEntries(string[] input)
        {
            var result = input.Where(s => !String.IsNullOrEmpty(s)).ToArray();
            return result;
        }
    }
}
