using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Regextra
{
    public static class RegexUtility
    {
        public static string[] SplitKeepDelimiters(string input,
            string[] delimiters,
            RegexOptions regexOptions = RegexOptions.None,
            RegextraSplitOptions splitOptions = RegextraSplitOptions.None)
        {
            if (delimiters == null || delimiters.Length == 0)
                throw new ArgumentException("Delimiters can't be empty", "delimiters");

            var delimiterPattern = "(" + String.Join("|", delimiters.Select(d => Regex.Escape(d))) + ")";

            string[] result;
            if (splitOptions == RegextraSplitOptions.None || splitOptions == RegextraSplitOptions.RemoveEmptyEntries)
            {
                result = Regex.Split(input, delimiterPattern, regexOptions);
            }
            else
            {
                StringBuilder pattern = new StringBuilder(delimiterPattern);

                if (splitOptions.HasFlag(RegextraSplitOptions.MatchWholeWords))
                {
                    pattern.Insert(0, @"\b").Append(@"\b");
                }
                if (splitOptions.HasFlag(RegextraSplitOptions.TrimWhitespace))
                {
                    pattern.Insert(0, @"\s*").Append(@"\s*");
                }

                result = Regex.Split(input, pattern.ToString(), regexOptions);
            }

            if (splitOptions.HasFlag(RegextraSplitOptions.RemoveEmptyEntries))
            {
                result = RemoveEmptyEntries(result);
            }
            return result;
        }

        private static string[] RemoveEmptyEntries(string[] input)
        {
            var result = input.Where(s => !String.IsNullOrEmpty(s)).ToArray();
            return result;
        }
    }
}
