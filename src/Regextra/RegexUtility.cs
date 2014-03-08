using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Regextra
{
    public static class RegexUtility
    {
        private static readonly Regex _trimWhitespacesRegex = new Regex(@"^\s+|\s+$|(\s)\1+", RegexOptions.Compiled);
        private static readonly Regex _formatCamelCaseRegex = new Regex(@"\s*(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]|(?<=[A-Za-z])[0-9])\s*", RegexOptions.Compiled);
        private static readonly Regex _formatCamelCaseCapitalizeRegex = new Regex(@"\b(?<LowerCaseChar>[a-z])(?=[a-z]*[A-Z])|\s*(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]|(?<=[a-zA-Z])[0-9])\s*", RegexOptions.Compiled);

        public static string[] Split(string input,
            string[] delimiters,
            RegexOptions regexOptions = RegexOptions.None,
            SplitOptions splitOptions = SplitOptions.None)
        {
            if (delimiters == null || delimiters.Length == 0)
            {
                throw new ArgumentException("Delimiters can't be empty", "delimiters");
            }

            var pattern = new StringBuilder(String.Join("|", delimiters.Select(d => Regex.Escape(d))));

            // pattern building order matters: IncludeDelimiters must occur first if selected
            if (splitOptions.HasFlag(SplitOptions.IncludeDelimiters))
            {
                PrefixSuffix(pattern, "(", ")");
            }
            if (splitOptions.HasFlag(SplitOptions.MatchWholeWords))
            {
                PrefixSuffix(pattern, @"\b");
            }
            if (splitOptions.HasFlag(SplitOptions.TrimWhitespace))
            {
                PrefixSuffix(pattern, @"\s*");
            }

            string[] result = Regex.Split(input, pattern.ToString(), regexOptions);
            if (splitOptions.HasFlag(SplitOptions.RemoveEmptyEntries))
            {
                result = RemoveEmptyEntries(result);
            }
            return result;
        }

        public static string[] SplitIncludeDelimiters(string input, string[] delimiters, RegexOptions regexOptions = RegexOptions.None)
        {
            return Split(input, delimiters, regexOptions, SplitOptions.IncludeDelimiters);
        }

        public static string[] SplitMatchWholeWords(string input, string[] delimiters, RegexOptions regexOptions = RegexOptions.None)
        {
            return Split(input, delimiters, regexOptions, SplitOptions.MatchWholeWords);
        }

        public static string[] SplitTrimWhitespace(string input, string[] delimiters, RegexOptions regexOptions = RegexOptions.None)
        {
            return Split(input, delimiters, regexOptions, SplitOptions.TrimWhitespace);
        }

        public static string[] SplitRemoveEmptyEntries(string input, string[] delimiters, RegexOptions regexOptions = RegexOptions.None)
        {
            return Split(input, delimiters, regexOptions, SplitOptions.RemoveEmptyEntries);
        }

        public static object SplitRemoveEmptyEntries(string input, string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            var split = Regex.Split(input, pattern, regexOptions);
            var result = RemoveEmptyEntries(split);
            return result;
        }

        public static string TrimWhitespaces(string input)
        {
            var result = _trimWhitespacesRegex.Replace(input, "$1");
            return result;
        }

        /// <summary>
        /// Formats PascalCase (upper CamelCase) and (lower) camelCase words to a friendly format separated by the given delimiter (space by default).
        /// </summary>
        /// <param name="input">CamelCase input to format</param>
        /// <param name="delimiter">Delimiter to use for formatting (space by default)</param>
        /// <param name="capitalizeFirstCharacter">Capitalize the first character for (lower) camelCase words (false by default)</param>
        public static string FormatCamelCase(string input, string delimiter = " ", CamelCaseOptions camelCaseOptions = CamelCaseOptions.None)
        {
            if (String.IsNullOrEmpty(delimiter))
            {
                throw new ArgumentException("Delimiter can't be null or empty", "delimiter");
            }

            string result;
            if (camelCaseOptions.HasFlag(CamelCaseOptions.CapitalizeFirstCharacter) || camelCaseOptions.HasFlag(CamelCaseOptions.CapitalizeFirstCharacterInvariantCulture))
            {
                result = _formatCamelCaseCapitalizeRegex.Replace(input, m => EvaluateCamelCaseMatchWithCapitalization(m, delimiter, camelCaseOptions));
            }
            else
            {
                var replacement = delimiter + "$1";
                result = _formatCamelCaseRegex.Replace(input, replacement);
            }

            return result;
        }

        private static string EvaluateCamelCaseMatchWithCapitalization(Match m, string delimiter, CamelCaseOptions options)
        {
            string result;

            if (m.Groups["LowerCaseChar"].Value != String.Empty)
            {
                if (options.HasFlag(CamelCaseOptions.CapitalizeFirstCharacterInvariantCulture))
                {
                    result = m.Groups["LowerCaseChar"].Value.ToUpperInvariant();
                }
                else
                {
                    result = m.Groups["LowerCaseChar"].Value.ToUpper();
                }
            }
            else
            {
                result = delimiter + m.Groups[1].Value;
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
